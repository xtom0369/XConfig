using System.IO;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace XConfig.Editor 
{
    //配置表主键的类型
    public enum EnumTableMojorkeyType
    {
        INT,//单一整型
        STRING,//单一字串
        INT_INT,//两个整型
        OTHER,//其它情况
    }
    public class ConfigFileImporter
    {
        public static char[] SEPARATOR = { '\t' };

        public string fileFullPath;
        public string relativePath;
        public string fileName;//文件名
        public bool isReadRow;//是否读取表内容
        public string tableClassName;//表类名
        public string rowClassName;//表行类名
        public string keyLine;
        public string commentLine;
        public string typeLine;
        public string flagLine;
        public string[] keys;
        public List<string> majorKeys;
        public string[] types;
        public List<string> majorTypes;
        public string[] defaults;
        public Flag[] flags;
        public List<string[]> cellStrs;//表内容的所有单位格，注意只有isReadContentRow=true才会有内容
        public Dictionary<string, string[]> firstKey2RowCells;
        public List<int> lineNumbers;//表内容每一行的行号
        public EnumTableMojorkeyType majorKeyType;
        public string parentFileName;//父表文件名
        public ConfigFileImporter parentFileImporter;//父表
        public List<ConfigFileImporter> childFileImporters = new List<ConfigFileImporter>();//子表数组

        //isReadContentRow:是否读取内容行到rows数组
        public ConfigFileImporter(string fileFullPath, string fileName, bool isReadContentRow = false)
        {
            this.fileFullPath = fileFullPath;
            this.relativePath = fileFullPath.Replace(Settings.Inst.CONFIG_PATH, "").Replace(".bytes", "").Replace("\\", "/");
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
            
            flagLine = reader.ReadLine();
            string[] flagCols = flagLine.Split(SEPARATOR);//标签
            flags = Array.ConvertAll(flagCols, x => Flag.Parse(x));

            DebugUtil.Assert(keys.Length == flags.Length, $"表 {fileName}.bytes keys长度和flags长度不一致 {keys.Length} != {this.flags.Length}");
            for (int i = 0; i < line.Length; i++)
            {
                string[] strs = line[i].Replace(" ", "").Split('='); // 去除空格
                types[i] = strs[0];
                types[i] = SetDefaultType(types[i], flags[i]);
                defaults[i] = GetDefaultValue(types[i], keys[i], flags[i], strs.Length > 1 ? strs[1] : null);
            }

            CheckValid();
            majorKeyType = GetMajorKeyType();
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
                        string[] values = rowStr.Split(SEPARATOR);
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
        public static bool IsNotExportCol(Flag flag)
        {
            return !flag.IsMajorKey && flag.IsNotExport;
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
        EnumTableMojorkeyType GetMajorKeyType()
        {
            majorKeys = new List<string>();
            majorTypes = new List<string>();
            for (int i = 0; i < flags.Length; i++)
            {
                if (flags[i].IsMajorKey)
                {
                    majorKeys.Add(keys[i]);
                    majorTypes.Add(types[i]);
                }
            }
            if (majorKeys.Count == 0 && keys.Length > 0)//容错，未设置则视为第一列是主键
            {
                majorKeys.Add(keys[0]);
                majorTypes.Add(types[0]);
            }
            if (majorKeys.Count == 1)
            {
                if (majorTypes[0] == "int")
                    return EnumTableMojorkeyType.INT;
                else if (majorTypes[0] == "string")
                    return EnumTableMojorkeyType.STRING;
            }
            else if (majorKeys.Count == 2)
            {
                if (majorTypes[0] == "int" && majorTypes[1] == "int")
                    return EnumTableMojorkeyType.INT_INT;
                else
                    DebugUtil.Assert(false, "现在只支持表的两个主键都为int类型：{0}", relativePath);
            }
            else
                DebugUtil.Assert(false, "不支持三个或以上的主键：{0}", relativePath);
            return EnumTableMojorkeyType.OTHER;
        }
        string GetDefaultValue(string type, string fieldName, Flag flag, string defaultVaule)
        {
            if (ConfigType.TryGetConfigType(type, out var configType))
            {
                if (!string.IsNullOrEmpty(defaultVaule) && !configType.CheckConfigFormat(defaultVaule, out var error))
                    DebugUtil.Assert(false, $"{fileName}.bytes 中 {fieldName} 字段默认值异常, {error}");

                return configType.ParseDefaultValueContent(defaultVaule);
            }

            if (type.StartsWith("List<"))//列表默认值为空列表，不为null，不然做检验的时候不好处理
            {
                string resultSt = "new " + type.ToString() + "()";
                if (!string.IsNullOrEmpty(defaultVaule) && defaultVaule != "null")
                {
                    resultSt += "{";
                    string[] items = defaultVaule.Split('|');
                    for (int i = 0; i < items.Length; i++)
                    {
                        if (i > 0)
                            resultSt += ",";
                        resultSt += items[i];
                    }
                    resultSt += "}";
                }
                return resultSt;
            }
            if (flag.IsReference) return "\"" + defaultVaule + "\"";//引用类型，缺省值都为字串
            return defaultVaule;
        }
    }
}