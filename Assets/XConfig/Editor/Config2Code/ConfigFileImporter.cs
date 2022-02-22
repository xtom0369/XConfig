using System.IO;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace XConfig.Editor 
{
    public class ConfigFileImporter
    {
        public static char[] SEPARATOR = { '\t' };

        public string fileFullPath;
        public string fileName;//文件名
        public bool isReadRow;//是否读取表内容
        public string tableClassName;//表类名
        public string rowClassName;//表行类名
        public string keyLine;
        public string commentLine;
        public string typeLine;
        public string flagLine;
        public string[] keys;
        public List<string> mainKeys;
        public string[] types;
        public List<string> mainTypes;
        public string[] defaults;
        public IConfigType[] configTypes;
        public Flag[] flags;
        public List<string[]> cellStrs;//表内容的所有单位格，注意只有isReadContentRow=true才会有内容
        public Dictionary<string, string[]> firstKey2RowCells;
        public List<int> lineNumbers;//表内容每一行的行号
        public EnumTableMainKeyType mainKeyType;
        public string parentFileName;//父表文件名
        public ConfigFileImporter parentFileImporter;//父表
        public List<ConfigFileImporter> childFileImporters = new List<ConfigFileImporter>();//子表数组

        //isReadContentRow:是否读取内容行到rows数组
        public ConfigFileImporter(string fileFullPath, string fileName, bool isReadContentRow = false)
        {
            this.fileFullPath = fileFullPath;
            this.fileName = fileName;
            this.parentFileName = ConfigInherit.GetParentFileName(fileName);
            this.isReadRow = isReadContentRow;
            string humpNamed = ConvertUtil.UnderscoreToCamel(fileName);
            this.tableClassName = humpNamed + "Table";
            this.rowClassName = humpNamed + "Row";
        }

        public void Import(StreamReader reader)
        {
            keyLine = reader.ReadLine();
            keys = keyLine.Split(SEPARATOR);//字段名
            for (int j = 0; j < keys.Length; j++)
                DebugUtil.Assert(keys[j].IndexOf(" ") == -1, "表 {0}.bytes 字段名 {1} 存在空格", fileName, keys[j]);

            commentLine = reader.ReadLine();//注释
            typeLine = reader.ReadLine();
            string[] line = typeLine.Split(SEPARATOR);//类型和缺省值
            types = new string[line.Length];
            defaults = new string[line.Length];
            configTypes = new IConfigType[line.Length];

            flagLine = reader.ReadLine();
            string[] flagCols = flagLine.Split(SEPARATOR);//标签
            flags = Array.ConvertAll(flagCols, x => Flag.Parse(x));

            DebugUtil.Assert(keys.Length == flags.Length, $"表 {fileName}.bytes keys长度和flags长度不一致 {keys.Length} != {this.flags.Length}");
            for (int i = 0; i < line.Length; i++)
            {
                string[] strs = line[i].Replace(" ", "").Split('='); // 去除空格
                types[i] = strs[0];
                types[i] = SetDefaultType(types[i], flags[i]);

                string type = types[i];
                if (ConfigType.TryGetConfigType(type, out var configType))
                    configTypes[i] = configType;
                else
                    DebugUtil.Assert(false, $"类 {rowClassName} 中存在不支持的数据类型 ：{types[i]}");

                defaults[i] = GetDefaultValue(configTypes[i], keys[i], flags[i], strs.Length > 1 ? strs[1] : null);
            }

            CheckValid();
            mainKeyType = GetMainKeyType();
            if (isReadRow)
            {
                lineNumbers = new List<int>();
                cellStrs = new List<string[]>();
                //有子表的表才创建此字典
                if (childFileImporters.Count > 0)
                    firstKey2RowCells = new Dictionary<string, string[]>();
                string rowStr;
                //第五行开始为真正的内容
                int rowIndex = 4;
                while ((rowStr = reader.ReadLine()) != null)
                {
                    rowIndex++;
                    if (!string.IsNullOrEmpty(rowStr) &&
                        !IsEmptyLineOrCommentLine(rowStr))//跳过空行
                    {
                        string[] values = rowStr.Split(SEPARATOR).Select(x => x.Trim()).ToArray();
                        cellStrs.Add(values);
                        lineNumbers.Add(rowIndex);
                        if (childFileImporters.Count > 0)
                            firstKey2RowCells.Add(values[0], values);
                    }
                }
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
        string SetDefaultType(string type, Flag flag)
        {
            string ret = type;
            // 没填类型默认为字符串
            if (string.IsNullOrEmpty(type))
                ret = "string";

            return ret;
        }

        void CheckValid()
        {
            for (int i = 0; i < types.Length; i++)
            {
                if (!string.IsNullOrEmpty(keys[i]))
                {
                    if (flags[i].IsNotExport) continue;

                    DebugUtil.Assert(!string.IsNullOrEmpty(types[i]), "表 {0}.bytes 字段 {1} 的类型不能为空", fileName, keys[i]);
                    if (isReadRow)//用于检测表列存在但是代码字段不存在的情况
                    {
                        //检测对应Class中要存在此字段
                        string key = keys[i];
                        if (flags[i].IsReference)
                        {
                            key += "Id";
                            if (types[i].IndexOf("List") != -1)
                                key += "s";
                        }
                        Type type = AssemblyUtil.GetType(rowClassName);
                        DebugUtil.Assert(type != null, "找不到类：{0}", rowClassName);
                        PropertyInfo filed = type.GetProperty(key, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                        DebugUtil.Assert(filed != null, "类{0}中找不到字段名：{1}，确认已导出配置表类！【操作方法：XConfig=>Generate Code】或者【快捷键alt+g】", rowClassName, key);
                    }
                }
            }
        }
        EnumTableMainKeyType GetMainKeyType() 
        {
            mainKeys = new List<string>();
            mainTypes = new List<string>();
            for (int i = 0; i < flags.Length; i++)
            {
                if (flags[i].IsMainKey)
                {
                    mainKeys.Add(keys[i]);
                    mainTypes.Add(types[i]);
                }
            }
            if (mainKeys.Count == 0 && keys.Length > 0)//容错，未设置则视为第一列是主键
            {
                mainKeys.Add(keys[0]);
                mainTypes.Add(types[0]);
            }
            if (mainKeys.Count == 1)
            {
                if (mainTypes[0] == "int")
                    return EnumTableMainKeyType.INT;
                else if (mainTypes[0] == "string")
                    return EnumTableMainKeyType.STRING;
            }
            else if (mainKeys.Count == 2)
            {
                if (mainTypes[0] == "int" && mainTypes[1] == "int")
                    return EnumTableMainKeyType.INT_INT;
                else
                    DebugUtil.Assert(false, "现在只支持表的两个主键都为int类型：{0}", fileName);
            }
            else
                DebugUtil.Assert(false, "不支持三个或以上的主键：{0}", fileName);
            return EnumTableMainKeyType.OTHER;
        }
        string GetDefaultValue(IConfigType configType, string fieldName, Flag flag, string defaultVaule)
        {
            if (string.IsNullOrEmpty(defaultVaule) || defaultVaule == "null")
                return configType.DefaultValue;

            // 不为空时检查值合法性
            if (!configType.CheckConfigFormat(defaultVaule, out var error))
                DebugUtil.Assert(false, $"{fileName}.bytes 中 {fieldName} 字段默认值异常, {error}");

            return configType.ParseDefaultValue(defaultVaule);
        }
    }
}