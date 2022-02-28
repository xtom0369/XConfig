using System.IO;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace XConfig.Editor 
{
    //配置表主键的类型
    public enum EnumTableMainKeyType
    {
        SINGLE, // 单组件
        DOUBLE, // 双主键
        OTHER, // 其它情况
    }

    public class ConfigFileImporter
    {
        public static char[] SEPARATOR = { '\t' };

        public ConfigFileContext context;

        // 路径
        public string fileFullPath;

        // 命名
        public string fileName;//文件名
        public string fullFileName;//文件名
        public bool isReadRow;//是否读取表内容
        public string tableClassName;//表类名
        public string lowerTableClassName;//首字母小写表类名
        public string rowClassName;//表行类名
        public string lowerRowClassName;//首字母小写表行类名

        // 表头解析数据
        public string[] keys;
        public string[] lowerKeys;
        public string[] types;
        public string[] defaults;
        public IConfigType[] configTypes;
        public Flag[] flags;
        public EnumTableMainKeyType mainKeyType;
        public List<int> mainKeyIndexs = new List<int>();
        public List<string> mainKeys = new List<string>();
        public List<string> mainTypes = new List<string>();

        // 表解析数据
        public List<string[]> rowDatas = new List<string[]>();//表内容的所有单位格，注意只有isReadContentRow=true才会有内容
        public List<int> rowIndexs = new List<int>();//表内容每一行的行号

        // 表继承数据
        public Dictionary<string, string[]> mainKey2RowData = new Dictionary<string, string[]>();
        public bool isParent; // 是否为父表
        public bool isChild; // 是否为子表
        public string parentFileName;//父表文件名
        public ConfigFileImporter parentImporter;//父表
        public List<ConfigFileImporter> childImporters = new List<ConfigFileImporter>();//子表数组

        public ConfigFileImporter(string fileFullPath, string fileName, ConfigFileContext context, bool isReadContentRow = false)
        {
            this.fileFullPath = fileFullPath;
            this.fileName = fileName;
            this.fullFileName = Path.GetFileName(fileFullPath);
            this.context = context;
            this.isReadRow = isReadContentRow;
            this.tableClassName = StringUtil.FileNameToTableName(fileName);
            this.lowerTableClassName = StringUtil.ToFirstCharLower(tableClassName);
            this.rowClassName = StringUtil.FileNameToRowName(fileName);
            this.lowerRowClassName = StringUtil.ToFirstCharLower(rowClassName);
            this.isParent = ConfigInherit.Inst.IsParent(fileName);
            this.isChild = ConfigInherit.Inst.TryGetParent(fileName, out this.parentFileName);
        }

        public void Import(StreamReader reader)
        {
            #region key
            keys = reader.ReadLine().Split(SEPARATOR);//字段名
            lowerKeys = new string[keys.Length];
            for (int j = 0; j < keys.Length; j++)
            {
                string key = keys[j];
                Assert(!string.IsNullOrEmpty(key), $"列 {key} 类型为空");
                Assert(keys[j].IndexOf(" ") == -1, $"字段名 {key} 存在空格");
                lowerKeys[j] = StringUtil.ToFirstCharLower(key);
            }
            #endregion

            #region comment
            string commentLine = reader.ReadLine();//注释
            #endregion

            #region type and default
            string[] typeCols = reader.ReadLine().Split(SEPARATOR);//类型和缺省值
            types = new string[typeCols.Length];
            Assert(types.Length == keys.Length, $"types长度和flags长度不一致 {types.Length} != {keys.Length}");
            defaults = new string[typeCols.Length];
            configTypes = new IConfigType[typeCols.Length];
            for (int i = 0; i < typeCols.Length; i++)
            {
                string[] strs = typeCols[i].Replace(" ", "").Split('='); // 去除空格
                string type = strs[0];
                types[i] = type;
                Assert(!string.IsNullOrEmpty(type), $"列 {keys[i]} 类型为空");

                if (ConfigType.TryGetConfigType(type, out var configType))
                    configTypes[i] = configType;
                else
                    Assert(false, $"存在不支持的数据类型 ：{types[i]}");

                string @default = strs.Length > 1 ? strs[1] : null;
                defaults[i] = GetDefaultValue(configTypes[i], keys[i], @default);
            }
            #endregion

            #region flag
            string[] flagCols = reader.ReadLine().Split(SEPARATOR);//标签
            flags = Array.ConvertAll(flagCols, x => Flag.Parse(x));
            Assert(flags.Length == keys.Length, $"types长度和flags长度不一致 {flags.Length} != {keys.Length}");
            #endregion

            mainKeyType = GetMainKeyType();

            if (isReadRow)
                ReadRows(reader);
        }
        void ReadRows(StreamReader reader) 
        {
            string rowStr;
            //第五行开始为真正的内容
            int rowIndex = 4;
            while ((rowStr = reader.ReadLine()) != null)
            {
                rowIndex++;

                if (string.IsNullOrEmpty(rowStr) || IsEmptyLineOrCommentLine(rowStr)) // 跳过空行
                    continue;

                string[] values = rowStr.Split(SEPARATOR).Select(x => x.Trim()).ToArray();
                rowDatas.Add(values);
                rowIndexs.Add(rowIndex);

                // 主键值到行的数据
                string key = GetCombineMainKeyValue(values);
                mainKey2RowData.Add(key, values);
            }
        }
        
        /// <summary>
        /// 导入配置表之后得流程，当前主要用于做检测
        /// </summary>
        public void OnAfterImport() 
        {
            for (int i = 0; i < configTypes.Length; i++)
            {
                if (flags[i].IsNotExport) continue;

                //检测对应Class中要存在此字段
                string key = keys[i];
                var configType = configTypes[i];
                if (configType.IsReference)
                {
                    string refFileName = configType.ReferenceFileName;
                    Assert(context.fileName2Importer.ContainsKey(refFileName), $"列 {key} 引用的表 {refFileName} 并不存在");

                    key = configType.ParseKeyName(key);
                }

                Type type = AssemblyUtil.GetType(rowClassName);
                DebugUtil.Assert(type != null, $"找不到类：{rowClassName}");
                PropertyInfo filed = type.GetProperty(key, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                DebugUtil.Assert(filed != null, $"类 {rowClassName} 中找不到字段名：{key}，尝试执行菜单【XConfig=>Generate Code】");
            }

            for (int i = 0; i < rowDatas.Count; i++) 
            {
                string[] values = rowDatas[i];
                Assert(values.Length == keys.Length, $"第 {rowIndexs[i]} 行主键列数和值列数不一致 {values.Length} != {keys.Length}");

                for (int j = 0; j < values.Length; j++) 
                    if (flags[j].IsMainKey) Assert(!string.IsNullOrEmpty(values[j]), $"第 {rowIndexs[i]} 行主键 {keys[j]} 为空");
            }

            /// 子表需检测
            /// 1. 是否与父表主键名及类型一致
            /// 2. 是否所有的行父表中都有相应主键值一致的行
            if (isChild) 
            {
                Assert(mainKeys.Count == parentImporter.mainKeys.Count, $"与父表 {parentImporter.fileName} 主键数量不一致 {mainKeys.Count} != {parentImporter.mainKeys.Count}");
                for (int i = 0; i < mainKeys.Count; i++) 
                {
                    Assert(mainKeys[i] == parentImporter.mainKeys[i], $"与父表 {parentImporter.fileName} 主键名不一致 {mainKeys[i]} != {parentImporter.mainKeys[i]}");
                    Assert(mainTypes[i] == parentImporter.mainTypes[i], $"与父表 {parentImporter.fileName} 主键类型不一致 {mainTypes[i]} != {parentImporter.mainTypes[i]}");
                }

                for (int i = 0; i < rowDatas.Count; i++)
                {
                    string[] values = rowDatas[i];
                    string key = GetCombineMainKeyValue(values);
                    Assert(parentImporter.mainKey2RowData.ContainsKey(key), $"无法在父表 {parentImporter.fileName} 中找到主键为 {key} 的行");
                }
            }

            /// 父表需检测
            /// 1. 存在的多余行
            if (isParent)
            {
                Dictionary<string, string> combineKey2FileName = new Dictionary<string, string>();
                for (int i = 0; i < childImporters.Count; i++)
                {
                    foreach (var kvp in childImporters[i].mainKey2RowData)
                    { 
                        DebugUtil.Assert(!combineKey2FileName.TryGetValue(kvp.Key, out var name), $"子表 {name} 和 子表 {childImporters[i].fileName} 存在相同的主键 {combineKey2FileName}，同一个主键只能存在于一个子表");
                        combineKey2FileName[kvp.Key] = childImporters[i].fileName;
                    }
                }

                foreach (var kvp in mainKey2RowData)
                    DebugUtil.Assert(combineKey2FileName.ContainsKey(kvp.Key), $"{fileName}为父表，存在主键为 {kvp.Key} 的多余的行，没有子表与之对应");
            }
        }
        bool IsEmptyLineOrCommentLine(string rowStr)
        {
            string[] values = rowStr.Split(SEPARATOR);
            for (int i = 0; i < values.Length; i++)
                if (values[i].Length > 0)
                    return false;
            return true;
        }

        EnumTableMainKeyType GetMainKeyType() 
        {
            for (int i = 0; i < flags.Length; i++)
            {
                if (flags[i].IsMainKey)
                {
                    mainKeys.Add(keys[i]);
                    mainTypes.Add(types[i]);
                    mainKeyIndexs.Add(i);
                }
            }
            Assert(mainKeys.Count > 0, $"表中没有主键，缺少flag配置为M的列");

            if (mainKeys.Count == 1)
            {
                if (mainTypes[0] == "int" || mainTypes[0] == "string")
                    return EnumTableMainKeyType.SINGLE;
                else
                    DebugUtil.Assert(false, $"单主键类型只支持int和string类型 ：{fileName} 主键为 {mainTypes[0]}");
            }
            else if (mainKeys.Count == 2)
            {
                if (mainTypes[0] == "int" && mainTypes[1] == "int")
                    return EnumTableMainKeyType.DOUBLE;
                else
                    Assert(false, $"只支持表的两个主键都为int类型的双主键");
            }
            else
                Assert(false, "不支持三个或以上的主键");
            return EnumTableMainKeyType.OTHER;
        }
        string GetDefaultValue(IConfigType configType, string fieldName, string defaultVaule)
        {
            if (string.IsNullOrEmpty(defaultVaule) || defaultVaule == "null")
                return configType.DefaultValue;

            // 不为空时检查值合法性
            if (!configType.CheckConfigFormat(defaultVaule, out var error))
                Assert(false, $"{fieldName} 字段默认值异常, {error}");

            return configType.ParseDefaultValue(defaultVaule);
        }
        /// <summary>
        /// 主键值下划线合并
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public string GetCombineMainKeyValue(string[] values) 
        {
            string combinekey = values[mainKeyIndexs[0]];
            for (int i = 1; i < mainKeyIndexs.Count; i++)
                combinekey += $",{values[mainKeyIndexs[i]]}";

            return combinekey;
        }

        void Assert(bool isValid, string msg)
        {
            DebugUtil.Assert(isValid, $"配置表 {fullFileName} 异常 : {msg}");
        }
    }
}