using System;
using System.Collections.Generic;
using System.IO;

namespace XConfig.Editor
{
    public class ConfigCodeFileExporter : TextFileExporter
    {
        private const string CLASS_TEMPLATE = @"using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using XConfig;
#if UNITY_STANDALONE || SERVER_EDITOR
using System.IO;
using System.Text;
using System.Reflection;
#endif

/*
 * 自动生成代码，请勿编辑
 * 增加变量，函数请在其他目录新建对应的
 * partial class 里增加，
 * 重新生成代码会删除旧代码
 */";

        ConfigFileContext context;
        public ConfigCodeFileExporter(string outFilePath, TextFileImporter importer, ConfigFileContext context) : base(outFilePath, importer)
        {
            this.context = context;
        }
        public ConfigFileImporter csvImporter { get { return importer as ConfigFileImporter; } }
        private List<string> readOnlyCacheList = new List<string>();
        protected override void DoExport()
        {
            WriteLine(CLASS_TEMPLATE);//写类文件头
            WriteConfigClass();//写Config扩展类
            WriteTableClass();//写Table扩展类
            WriteRowClass();//写Row扩展类
        }
        void WriteTableClass()
        {
            WriteLine("[Serializable]");
            WriteLine("public partial class {0} : XTable", csvImporter.tableClassName);
            WriteLine("{");
            TabShift(1);
            WriteLine("public List<{0}> rows {{ get {{ return _tableRows; }}}}", csvImporter.rowClassName);
            WriteTableInternalRows();
            WriteTableFromBytesFunction();
            WriteTableExportCsvFunction();
            WriteTabelGetCsvPathFunction();
            WriteTableIsOpenCsvFunction();
            if (csvImporter.majorKeyType == EnumTableMojorkeyType.INT)
            {
                WriteInitRowsFunction_int();
                WriteGetRowFunction_int();
                WriteTryGetRowFunction_int();
                WriteContainsMajorKeyFunction_int();
                WriteAddRowFunction_int();
                WriteRemoveRowFunction_int();
                WriteSortRows_int();
            }
            else if (csvImporter.majorKeyType == EnumTableMojorkeyType.STRING)
            {
                WriteInitRowsFunction_string();
                WriteGetRowFunction_string();
                WriteTryGetRowFunction_string();
                WriteContainsMajorKeyFunction_string();
                WriteAddRowFunction_string();
                WriteRemoveRowFunction_string();
                WriteSortRows_string();
            }
            else if (csvImporter.majorKeyType == EnumTableMojorkeyType.INT_INT)
            {
                WriteInitRowsFunction_int_int();
                WriteGetRowFunction_int_int();
                WriteTryGetRowFunction_int_int();
                WriteContainsMajorKeyFunction_int_int();
                WriteGetMajorKeyFunction_int_int();
                WriteAddRowFunction_int_int();
                WriteRemoveRowFunction_int_int();
                WriteSortRows_int_int();
            }
            else
            {
                DebugUtil.Assert(false);
            }
            //if (csvImporter.childFileImporters.Count > 0 && csvImporter.parentFileImporter == null)//是总表
            //    WriteAddRowFunction();
            WriteAllTableInitCompleteFunction();
            TabShift(-1);
            WriteLine("}");
        }

        void WriteTableInternalRows()
        {
            WriteLine("List<{0}> _tableRows;", csvImporter.rowClassName);
        }

        void WriteTableFromBytesFunction()
        {
            WriteLine("override public void FromBytes(BytesBuffer buffer)");
            WriteLine("{");
            TabShift(1);
            WriteLine("#if UNITY_STANDALONE || SERVER_EDITOR");
            WriteLine("keys = buffer.ReadString();");
            WriteLine("comments = buffer.ReadString();");
            WriteLine("types = buffer.ReadString();");
            WriteLine("flags = buffer.ReadString();");
            WriteLine("#endif");
            WriteLine("if (_tableRows == null)");
            WriteLine("{");
            TabShift(1);
            WriteLine("_tableRows = new List<{0}>();", csvImporter.rowClassName);
            WriteLine("ushort rowCount = buffer.ReadUInt16();");
            WriteLine("for (int i = 0; i < rowCount; i++)");
            WriteLine("{");
            TabShift(1);
            WriteLine("{0} row = new {0}();", csvImporter.rowClassName);
            WriteLine("row.FromBytes(buffer);");
            WriteLine("_tableRows.Add(row);");
            TabShift(-1);
            WriteLine("}");
            TabShift(-1);
            WriteLine("}");
            WriteLine("else");
            WriteLine("{");
            TabShift(1);
            WriteLine("ushort rowCount = buffer.ReadUInt16();");
            WriteLine("for (int i = 0; i < rowCount; i++)");
            WriteLine(1, "_tableRows[i].FromBytes(buffer);");
            TabShift(-1);
            WriteLine("}");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteTableExportCsvFunction()
        {
            WriteLine("#if UNITY_STANDALONE || SERVER_EDITOR");
            WriteLine("override public void ExportCsv()");
            WriteLine("{");
            TabShift(1);
            WriteLine("string csvPath = GetCsvPath();");
            WriteLine("if (!string.IsNullOrEmpty(csvPath) && !IsOpenCsv())");
            WriteLine("{");
            TabShift(1);
            WriteLine("using (FileStream fs = new FileStream(csvPath, FileMode.Create, FileAccess.Write))");
            WriteLine("{");
            TabShift(1);
            WriteLine("using (StreamWriter writer = new StreamWriter(fs, Encoding.GetEncoding(\"GB2312\")))");
            WriteLine("{");
            TabShift(1);
            WriteLine("writer.NewLine = \"\\r\\n\";");
            WriteLine("writer.WriteLine(keys);");
            WriteLine("writer.WriteLine(comments);");
            WriteLine("writer.WriteLine(types);");
            WriteLine("writer.WriteLine(flags);");
            WriteLine("if(_tableRows != null)");
            WriteLine("{");
            TabShift(1);
            WriteLine("if (IsOverrideSort())");
            WriteLine(1, "_tableRows.Sort(Sort);");
            WriteLine("else");
            WriteLine(1, "_tableRows.Sort(SortRows);");
            //WriteLine("int lastRowIndex = 5;");
            WriteLine("for (int i = 0; i < _tableRows.Count; i++)");
            WriteLine("{");
            TabShift(1);
            //WriteLine("while (lastRowIndex < _tableRows[i].rowIndex)");
            //WriteLine("{");
            //WriteLine(1, "writer.WriteLine();");
            //WriteLine(1, "lastRowIndex++;");
            //WriteLine("}");
            WriteLine("_tableRows[i].ExportCsvRow(writer);");
            WriteLine("writer.Write(\"\\r\\n\");");
            //WriteLine("lastRowIndex++;");
            TabShift(-1);
            WriteLine("}");
            TabShift(-1);
            WriteLine("}");
            WriteLine("writer.Dispose();");
            WriteLine("writer.Close();");
            TabShift(-1);
            WriteLine("}");
            TabShift(-1);
            WriteLine("}");
            TabShift(-1);
            WriteLine("}");
            TabShift(-1);
            WriteLine("}");
            WriteLine("#endif");
        }
        void WriteTabelGetCsvPathFunction()
        {
            WriteLine("#if UNITY_STANDALONE || SERVER_EDITOR");
            WriteLine("public static string GetCsvPath()");
            WriteLine("{");
            TabShift(1);
            WriteLine("string path = null;");
            WriteLine("BindCsvPathAttribute attr = typeof(Config).GetField(\"{0}\").GetCustomAttribute<BindCsvPathAttribute>(false);", ConvertUtil.ToFirstCharlower(csvImporter.tableClassName));
            WriteLine("if (attr != null)");
            WriteLine("{");
            TabShift(1);
            WriteLine("path = string.Format(\"{0}{1}.bytes\", \"../config/\", attr.csvPath);");
            TabShift(-1);
            WriteLine("}");
            WriteLine("return path;");
            TabShift(-1);
            WriteLine("}");
            WriteLine("#endif");
        }
        void WriteTableIsOpenCsvFunction()
        {
            WriteLine("#if UNITY_STANDALONE || SERVER_EDITOR");
            WriteLine("public static bool IsOpenCsv()");
            WriteLine("{");
            TabShift(1);
            WriteLine("bool ret = false;");
            WriteLine("string path = GetCsvPath();");
            WriteLine("{");
            TabShift(1);
            WriteLine("ret = FileUtil.IsFileInUse(path);");
            TabShift(-1);
            WriteLine("}");
            WriteLine("return ret;");
            TabShift(-1);
            WriteLine("}");
            WriteLine("#endif");
        }
        void WriteAllTableInitCompleteFunction()
        {
            WriteLine("override public void AllTableInitComplete()");
            WriteLine("{");
            TabShift(1);
            WriteLine("if (_tableRows.Count > 0 && _tableRows[0] is IRowInitComplete)");
            WriteLine("{");
            TabShift(1);
            WriteLine("for (int i = 0; i < _tableRows.Count; i++)");
            WriteLine("{");
            TabShift(1);
            WriteLine("_tableRows[i].ClearReadOnlyCache();");
            WriteLine("(_tableRows[i] as IRowInitComplete).AfterRowInitComplete();");
            TabShift(-1);
            WriteLine("}");
            TabShift(-1);
            WriteLine("}");
            WriteLine("if (this is ITableInitComplete)");
            WriteLine(1, "(this as ITableInitComplete).AfterTableInitComplete();");
            if (csvImporter.parentFileImporter != null)//是子表
            {
                ConfigFileImporter rootImporter = csvImporter.parentFileImporter;
                while (rootImporter.parentFileImporter != null)
                    rootImporter = rootImporter.parentFileImporter;
                WriteLine("for (int i = 0; i < _tableRows.Count; i++)//子表才需要往总表添加");
                WriteLine(1, "Config.Inst.{0}.AddRow(_tableRows[i]);", ConvertUtil.ToFirstCharlower(rootImporter.tableClassName));
            }
            TabShift(-1);
            WriteLine("}");
        }
        void WriteInitRowsFunction_int()
        {
            WriteLine("Dictionary<int, {0}> _intMajorKey2Row;", csvImporter.rowClassName);
            WriteLine("override public void InitRows()");
            WriteLine("{");
            TabShift(1);
            WriteLine("{0} row = null;", csvImporter.rowClassName);
            WriteLine("_intMajorKey2Row = new Dictionary<int, {0}>();", csvImporter.rowClassName);
            WriteLine("for (int i = 0; i < _tableRows.Count; i++)");
            WriteLine("{");
            TabShift(1);
            WriteLine("row = _tableRows[i];");
            WriteLine("int majorKey = row.{0};", csvImporter.majorKeys[0]);
            WriteLine("DebugUtil.Assert(!_intMajorKey2Row.ContainsKey(majorKey), \"{0} 主键重复：{1}，请先按键盘【alt+r】导出配置试试！\", name, majorKey);");
            WriteLine("_intMajorKey2Row.Add(majorKey, row);");
            TabShift(-1);
            WriteLine("}");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteInitRowsFunction_int_int()
        {
            WriteLine("Dictionary<long, {0}> _intMajorKey2Row;", csvImporter.rowClassName);
            WriteLine("override public void InitRows()");
            WriteLine("{");
            TabShift(1);
            WriteLine("{0} row = null;", csvImporter.rowClassName);
            WriteLine("_intMajorKey2Row = new Dictionary<long, {0}>();", csvImporter.rowClassName);
            WriteLine("for (int i = 0; i < _tableRows.Count; i++)");
            WriteLine("{");
            TabShift(1);
            WriteLine("row = _tableRows[i];");
            WriteLine("long majorKey = ((long)row.{0}<<32) + row.{1};", csvImporter.majorKeys[0], csvImporter.majorKeys[1]);
            string temp = string.Format("row.{0}, row.{1}", csvImporter.majorKeys[0], csvImporter.majorKeys[1]);
            WriteLine("DebugUtil.Assert(!_intMajorKey2Row.ContainsKey(majorKey), \"{0} 主键重复：{1} {2}\", name, " + temp + ");");
            WriteLine("_intMajorKey2Row.Add(majorKey, row);");
            TabShift(-1);
            WriteLine("}");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteInitRowsFunction_string()
        {
            WriteLine("Dictionary<string, {0}> _stringMajorKey2Row;", csvImporter.rowClassName);
            WriteLine("override public void InitRows()");
            WriteLine("{");
            TabShift(1);
            WriteLine("{0} row = null;", csvImporter.rowClassName);
            WriteLine("_stringMajorKey2Row = new Dictionary<string, {0}>();", csvImporter.rowClassName);
            WriteLine("for (int i = 0; i < _tableRows.Count; i++)");
            WriteLine("{");
            TabShift(1);
            WriteLine("row = _tableRows[i];");
            WriteLine("string majorKey = row.{0};", csvImporter.majorKeys[0]);
            WriteLine("DebugUtil.Assert(!_stringMajorKey2Row.ContainsKey(majorKey), \"{0} 主键重复：{1}\", name, majorKey);");
            WriteLine("_stringMajorKey2Row.Add(majorKey, row);");
            TabShift(-1);
            WriteLine("}");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteGetRowFunction_int()
        {
            WriteLine("virtual public {0} GetRow(int majorKey, bool isAssert=true)", csvImporter.rowClassName);
            WriteLine("{");
            TabShift(1);
            WriteLine("{0} row;", csvImporter.rowClassName);
            WriteLine("if (_intMajorKey2Row.TryGetValue(majorKey, out row))");
            WriteLine(1, "return row;");
            WriteLine("if (isAssert)");
            WriteLine(1, "DebugUtil.Assert(row != null, \"{0} 找不到指定主键为 {1} 的行，请先按键盘【alt+r】导出配置试试！\", name, majorKey);");
            WriteLine("return null;");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteTryGetRowFunction_int()
        {
            WriteLine("virtual public bool TryGetRow(int majorKey, out {0} row)", csvImporter.rowClassName);
            WriteLine("{");
            TabShift(1);
            WriteLine("row = null;");
            WriteLine("return _intMajorKey2Row.TryGetValue(majorKey, out row);");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteContainsMajorKeyFunction_int()
        {
            WriteLine("public bool ContainsMajorKey(int majorKey)");
            WriteLine("{");
            TabShift(1);
            WriteLine("return _intMajorKey2Row.ContainsKey(majorKey);");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteAddRowFunction_int()
        {
            WriteLine("public void AddRow({0} row)", csvImporter.rowClassName);
            WriteLine("{");
            TabShift(1);
            WriteLine("if (!_intMajorKey2Row.ContainsKey(row.{0}))", csvImporter.majorKeys[0]);
            WriteLine("{");
            TabShift(1);
            WriteLine("_tableRows.Add(row);");
            WriteLine("_intMajorKey2Row.Add(row.{0}, row);", csvImporter.majorKeys[0]);
            if (csvImporter.parentFileImporter != null)
            {
                ConfigFileImporter rootImporter = csvImporter.parentFileImporter;
                while (rootImporter.parentFileImporter != null)
                    rootImporter = rootImporter.parentFileImporter;
                WriteLine("Config.Inst.{0}.AddRow(row);//子表才需要往总表添加", ConvertUtil.ToFirstCharlower(rootImporter.tableClassName));
            }
            TabShift(-1);
            WriteLine("}");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteRemoveRowFunction_int()
        {
            WriteLine("#if UNITY_STANDALONE || SERVER_EDITOR");
            WriteLine("public void RemoveRow({0} row)", csvImporter.rowClassName);
            WriteLine("{");
            TabShift(1);
            WriteLine("if (row != null)");
            WriteLine(1, "RemoveRow(row.{0});", csvImporter.majorKeys[0]);
            TabShift(-1);
            WriteLine("}");

            WriteLine("public void RemoveRow(int majorKey)", csvImporter.rowClassName);
            WriteLine("{");
            TabShift(1);
            WriteLine("if (_intMajorKey2Row.ContainsKey(majorKey))");
            WriteLine("{");
            TabShift(1);
            WriteLine("{0} row = GetRow(majorKey);", csvImporter.rowClassName);
            WriteLine("_tableRows.Remove(row);");
            WriteLine("_intMajorKey2Row.Remove(majorKey);");
            TabShift(-1);
            WriteLine("}");
            TabShift(-1);
            WriteLine("}");
            WriteLine("#endif");
        }
        void WriteSortRows_int()
        {
            WriteLine("private int SortRows({0} left, {0} right)", csvImporter.rowClassName);
            WriteLine("{");
            TabShift(1);
            WriteLine("int result = left.{0}.CompareTo(right.{0});", csvImporter.majorKeys[0]);
            WriteLine("return result;");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteSortRows_string()
        {
            WriteLine("private int SortRows({0} left, {0} right)", csvImporter.rowClassName);
            WriteLine("{");
            TabShift(1);
            WriteLine("int result = left.{0}.CompareTo(right.{0});", csvImporter.majorKeys[0]);
            WriteLine("return result;");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteSortRows_int_int()
        {
            WriteLine("private int SortRows({0} left, {0} right)", csvImporter.rowClassName);
            WriteLine("{");
            TabShift(1);
            WriteLine("int result = left.{0}.CompareTo(right.{0});", csvImporter.majorKeys[0]);
            WriteLine("if (result == 0)");
            WriteLine(1, "result = left.{0}.CompareTo(right.{0});", csvImporter.majorKeys[1]);
            WriteLine("return result;");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteGetRowFunction_int_int()
        {
            WriteLine("virtual public {0} GetRow(int key1, int key2, bool isAssert=true)", csvImporter.rowClassName);
            WriteLine("{");
            TabShift(1);
            WriteLine("{0} row;", csvImporter.rowClassName);
            WriteLine("long majorKey = ((long)key1<<32) + key2;");
            WriteLine("if (_intMajorKey2Row.TryGetValue(majorKey, out row))");
            WriteLine(1, "return row;");
            WriteLine("if (isAssert)");
            WriteLine(1, "DebugUtil.Assert(row != null, \"{0} 找不到指定主键为 {1} {2} 的行，请先按键盘【alt+r】导出配置试试！\", name, key1, key2);");
            WriteLine("return null;");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteTryGetRowFunction_int_int()
        {
            WriteLine("virtual public bool TryGetRow(int key1, int key2, out {0} row)", csvImporter.rowClassName);
            WriteLine("{");
            TabShift(1);
            WriteLine("row = null;");
            WriteLine("long majorKey = ((long)key1<<32) + key2;");
            WriteLine("return _intMajorKey2Row.TryGetValue(majorKey, out row);");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteContainsMajorKeyFunction_int_int()
        {
            WriteLine("public bool ContainsMajorKey(int key1, int key2)");
            WriteLine("{");
            TabShift(1);
            WriteLine("long majorKey = GetMajorKey(key1, key2);");
            WriteLine("return _intMajorKey2Row.ContainsKey(majorKey);");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteGetMajorKeyFunction_int_int()
        {
            WriteLine("public long GetMajorKey(int key1, int key2)");
            WriteLine("{");
            TabShift(1);
            WriteLine("return ((long)key1<<32) + key2;");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteAddRowFunction_int_int()
        {
            WriteLine("public void AddRow({0} row)", csvImporter.rowClassName);
            WriteLine("{");
            TabShift(1);
            WriteLine("long majorKey = ((long)row.{0} << 32) + row.{1};", csvImporter.majorKeys[0], csvImporter.majorKeys[1]);
            WriteLine("if (!_intMajorKey2Row.ContainsKey(majorKey))");
            WriteLine("{");
            TabShift(1);
            WriteLine("_tableRows.Add(row);");
            WriteLine("_intMajorKey2Row.Add(majorKey, row);");
            if (csvImporter.parentFileImporter != null)
            {
                ConfigFileImporter rootImporter = csvImporter.parentFileImporter;
                while (rootImporter.parentFileImporter != null)
                    rootImporter = rootImporter.parentFileImporter;
                WriteLine("Config.Inst.{0}.AddRow(row);//子表才需要往总表添加", ConvertUtil.ToFirstCharlower(rootImporter.tableClassName));
            }
            TabShift(-1);
            WriteLine("}");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteRemoveRowFunction_int_int()
        {
            WriteLine("#if UNITY_STANDALONE || SERVER_EDITOR");
            WriteLine("public void RemoveRow({0} row)", csvImporter.rowClassName);
            WriteLine("{");
            TabShift(1);
            WriteLine("if (row != null)");
            WriteLine(1, "RemoveRow(row.{0}, row.{1});", csvImporter.majorKeys[0], csvImporter.majorKeys[1]);
            TabShift(-1);
            WriteLine("}");

            WriteLine("public void RemoveRow(int key1, int key2)");
            WriteLine("{");
            TabShift(1);
            WriteLine("long majorKey = GetMajorKey(key1, key2);");
            WriteLine("if (_intMajorKey2Row.ContainsKey(majorKey))");
            WriteLine("{");
            TabShift(1);
            WriteLine("{0} removeRow = GetRow(key1, key2);", csvImporter.rowClassName);
            WriteLine("_tableRows.Remove(removeRow);");
            WriteLine("_intMajorKey2Row.Remove(majorKey);");
            TabShift(-1);
            WriteLine("}");
            TabShift(-1);
            WriteLine("}");
            WriteLine("#endif");
        }
        void WriteGetRowFunction_string()
        {
            WriteLine("virtual public {0} GetRow(string majorKey, bool isAssert=true)", csvImporter.rowClassName);
            WriteLine("{");
            TabShift(1);
            WriteLine("{0} row;", csvImporter.rowClassName);
            WriteLine("if (_stringMajorKey2Row.TryGetValue(majorKey, out row))");
            WriteLine(1, "return row;");
            WriteLine("if (isAssert)");
            WriteLine(1, "DebugUtil.Assert(row != null, \"{0} 找不到指定主键为 {1}的行，请先按键盘【alt+r】导出配置试试！\", name, majorKey);");
            WriteLine("return null;");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteTryGetRowFunction_string()
        {
            WriteLine("virtual public bool TryGetRow(string majorKey, out {0} row)", csvImporter.rowClassName);
            WriteLine("{");
            TabShift(1);
            WriteLine("row = null;", csvImporter.rowClassName);
            WriteLine("return _stringMajorKey2Row.TryGetValue(majorKey, out row);");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteContainsMajorKeyFunction_string()
        {
            WriteLine("public bool ContainsMajorKey(string majorKey)");
            WriteLine("{");
            TabShift(1);
            WriteLine("return _stringMajorKey2Row.ContainsKey(majorKey);");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteAddRowFunction_string()
        {
            WriteLine("public void AddRow({0} row)", csvImporter.rowClassName);
            WriteLine("{");
            TabShift(1);
            WriteLine("if (!_stringMajorKey2Row.ContainsKey(row.{0}))", csvImporter.majorKeys[0]);
            WriteLine("{");
            TabShift(1);
            WriteLine("_tableRows.Add(row);");
            WriteLine("_stringMajorKey2Row.Add(row.{0}, row);", csvImporter.majorKeys[0]);
            if (csvImporter.parentFileImporter != null)
            {
                ConfigFileImporter rootImporter = csvImporter.parentFileImporter;
                while (rootImporter.parentFileImporter != null)
                    rootImporter = rootImporter.parentFileImporter;
                WriteLine("Config.Inst.{0}.AddRow(row);//子表才需要往总表添加", ConvertUtil.ToFirstCharlower(rootImporter.tableClassName));
            }
            TabShift(-1);
            WriteLine("}");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteRemoveRowFunction_string()
        {
            WriteLine("#if UNITY_STANDALONE || SERVER_EDITOR");
            WriteLine("public void RemoveRow({0} row)", csvImporter.rowClassName);
            WriteLine("{");
            TabShift(1);
            WriteLine("if (row != null)");
            WriteLine(1, "RemoveRow(row.{0});", csvImporter.majorKeys[0]);
            TabShift(-1);
            WriteLine("}");

            WriteLine("public void RemoveRow(string majorKey)");
            WriteLine("{");
            TabShift(1);
            WriteLine("if (_stringMajorKey2Row.ContainsKey(majorKey))");
            WriteLine("{");
            TabShift(1);
            WriteLine("{0} removeRow = GetRow(majorKey);", csvImporter.rowClassName);
            WriteLine("_tableRows.Remove(removeRow);");
            WriteLine("_stringMajorKey2Row.Remove(majorKey);");
            TabShift(-1);
            WriteLine("}");
            TabShift(-1);
            WriteLine("}");
            WriteLine("#endif");
        }
        void WriteRowClass()
        {
            WriteLine("[Serializable]");
            if (csvImporter.parentFileImporter != null)
                WriteLine("public partial class {0} : {1}", csvImporter.rowClassName, csvImporter.parentFileImporter.rowClassName);
            else
                WriteLine("public partial class {0} : XRow", csvImporter.rowClassName);
            WriteLine("{");
            TabShift(1);

            List<ConfigFileImporter> parentImporters = new List<ConfigFileImporter>();
            ConfigFileImporter parentImporter = csvImporter.parentFileImporter;
            while (parentImporter != null)
            {
                parentImporters.Insert(0, parentImporter);
                parentImporter = parentImporter.parentFileImporter;
            }
            Dictionary<string, bool> parentImporterKeyDic = new Dictionary<string, bool>();
            foreach (ConfigFileImporter importer in parentImporters)
            {
                parentImporter = importer;
                foreach (string key in parentImporter.keys)
                {
                    if (string.IsNullOrEmpty(key)) continue;
                    if (!parentImporterKeyDic.ContainsKey(key))
                        parentImporterKeyDic.Add(key, true);
                }
            }

            //是否是继承关系的表
            bool isInheritClass = csvImporter.parentFileImporter != null || csvImporter.childFileImporters.Count > 0;
            for (int i = 0; i < csvImporter.keys.Length; i++)
            {
                string key = csvImporter.keys[i];
                if (string.IsNullOrEmpty(key)) continue;
                string flag = csvImporter.flags[i];
                if (ConfigFileImporter.IsFilterNotUseColoum(flag)) continue;
                string type = csvImporter.types[i];
                //string defaultValue = csvImporter.defaults[i];
                if (isInheritClass && flag.Contains("M"))
                {
                    DebugUtil.Assert(key.Equals("Id"), "包含子父表继承关系时，主键变量名必须为【Id】,请修改配置表【{0}】主键", csvImporter.fileName);
                }
                // 子表跟注释不生成主键字段
                if ((parentImporters.Count > 0 && flag.Contains("M")) || flag.Contains("N"))
                {
                    continue;
                }
                if (parentImporters.Count > 0)
                {
                    DebugUtil.Assert(!parentImporterKeyDic.ContainsKey(key), "子表的变量名不能父表相同，请检查并修改【{0}】表的【{1}】字段", csvImporter.fileName, key);
                }
                if (flag.Contains("R"))//引用类型
                {
                    if (type.StartsWith("List<"))//列表引用
                    {
                        WriteListReference(key, type, flag);
                        WriteListReferencePlus(key, type, flag);
                    }
                    else//单引用
                        WriteReference(key, type, flag);
                }
                else if (type.Contains("DateTime") || type.Contains("date"))//日期类型
                {
                    if (type.StartsWith("List<"))
                        WriteListDateTime(key, type, flag);
                    else
                        WriteDateTime(key, type, flag);
                }
                else
                {
                    if (flag.Contains("L"))
                        WriteLine("[CsvLayerInteger]");
                    //time类型先处理成用string代替
                    if (type.Contains("time"))
                        type = type.Replace("time", "string");
                    WriteLine("[SerializeField]");
                    if (type.StartsWith("List<"))
                    {
                        string lowerName = ConvertUtil.ToFirstCharlower(key);
                        WriteLine("private {0} _{1};", type, key);
                        string readOnlyType = type.Replace("List", "ReadOnlyCollection");
                        string cacheReadOnlyKey = $"_{lowerName}ReadOnlyCache";
                        WriteLine($"private {readOnlyType} {cacheReadOnlyKey};");
                        readOnlyCacheList.Add(cacheReadOnlyKey);
                        WriteLine("public {0} {1}", readOnlyType, key);
                        WriteLine("{");
                        TabShift(1);

                        WriteLine("get");
                        WriteLine("{");
                        TabShift(1);

                        WriteLine("if ({0} == null)", cacheReadOnlyKey);
                        WriteLine(1, "{0} = _{1}.AsReadOnly();", cacheReadOnlyKey, key);
                        WriteLine("return {0};", cacheReadOnlyKey);

                        TabShift(-1);
                        WriteLine("}");

                        TabShift(-1);
                        WriteLine("}");
                    }
                    else
                    {
                        WriteLine("private {0} _{1};", type, key);
                        WriteLine("public {0} {1}{{ get {{ return _{1}; }}}}", type, key);
                    }
                }
            }

            // editor 相关生成用 region 包起来，方便阅读
            WriteLine("");
            WriteLine("#region editor fields 编辑模式使用的成员变量");
            WriteLine("#if UNITY_STANDALONE || SERVER_EDITOR");
            for (int i = 0; i < csvImporter.keys.Length; i++)
            {

                string key = csvImporter.keys[i];
                if (string.IsNullOrEmpty(key)) continue;
                string flag = csvImporter.flags[i];
                bool isNeedGenerateField = ConfigFileImporter.IsFilterNotUseColoum(flag);
                string type = csvImporter.types[i];
                //string defaultValue = csvImporter.defaults[i];
                if (isInheritClass && flag.Contains("M"))
                {
                    DebugUtil.Assert(key.Equals("Id"), "包含子父表继承关系时，主键变量名必须为【Id】,请修改配置表【{0}】主键", csvImporter.fileName);
                }
                // 子表不生成主键字段
                if (parentImporters.Count > 0 && flag.Contains("M"))
                {
                    continue;
                }
                if (parentImporters.Count > 0)
                {
                    DebugUtil.Assert(!parentImporterKeyDic.ContainsKey(key), "子表的变量名不能父表相同，请检查并修改【{0}】表的【{1}】字段", csvImporter.fileName, key);
                }
                if (flag.Contains("R"))//引用类型
                {
                    if (type.StartsWith("List<"))//列表引用
                    {
                        if (isNeedGenerateField)
                        {
                            WriteListReference(key, type, flag, true);
                            WriteListReferencePlus(key, type, flag, true);
                        }
                        WriteListReference_editor(key, type, flag);
                    }
                    else//单引用
                    {
                        if (isNeedGenerateField)
                            WriteReference(key, type, flag, true);
                        WriteReference_editor(key, type, flag);
                    }
                }
                else if (type.Contains("DateTime") || type.Contains("date"))//日期类型
                {
                    if (type.StartsWith("List<"))
                    {
                        if (isNeedGenerateField)
                            WriteListDateTime(key, type, flag, true);
                        WriteListDateTime_editor(key, type, flag);
                    }
                    else
                    {
                        if (isNeedGenerateField)
                            WriteDateTime(key, type, flag, true);
                        WriteDateTime_editor(key, type, flag);
                    }
                }
                else
                {
                    //time类型先处理成用string代替
                    if (type.Contains("time"))
                        type = type.Replace("time", "string");
                    if (type.StartsWith("List<"))
                    {
                        string lowerName = ConvertUtil.ToFirstCharlower(key);
                        string readOnlyType = type.Replace("List", "ReadOnlyCollection");
                        string cacheReadOnlyKey = $"_{lowerName}ReadOnlyCache";
                        if (isNeedGenerateField)
                        {
                            readOnlyCacheList.Add(cacheReadOnlyKey);
                            WriteLine("private {0} _{1};", type, key);
                            WriteLine($"private {readOnlyType} {cacheReadOnlyKey};");
                            WriteLine("public {0} {1}", readOnlyType, key);
                            WriteLine("{");
                            TabShift(1);

                            WriteLine("get");
                            WriteLine("{");
                            TabShift(1);

                            WriteLine("if ({0} == null)", cacheReadOnlyKey);
                            WriteLine(1, "{0} = _{1}.AsReadOnly();", cacheReadOnlyKey, key);
                            WriteLine("return {0};", cacheReadOnlyKey);

                            TabShift(-1);
                            WriteLine("}");

                            TabShift(-1);
                            WriteLine("}");
                        }

                        WriteLine("public {0} {1}_editor{{ get {{ return _{1}; }} set {{ _{1} = value; {2} = null; }}}}", type, key, cacheReadOnlyKey);
                    }
                    else if (flag.Contains("N"))
                    {
                        WriteLine("private {0} _{1};", type, key);
                        WriteLine("private {0} {1}{{ get {{ return _{1}; }}}}", type, key);
                        WriteLine("public {0} {1}_editor{{ get {{ return _{1}; }} set {{ _{1} = value; }}}}", type, key);
                    }
                    else
                    {
                        if (isNeedGenerateField)
                        {
                            WriteLine("private {0} _{1};", type, key);
                            WriteLine("private {0} {1}{{ get {{ return _{1}; }}}}", type, key);
                        }

                        WriteLine("public {0} {1}_editor{{ get {{ return _{1}; }} set {{ _{1} = value; }}}}", type, key);
                    }
                }
            }
            WriteLine("#endif");
            WriteLine("#endregion");

            WriteRowFromBytesFunction();
            WriteRowExportCsvFunction();
            WriteRowCloneFunction();
            WriteRowClearReadOnlyCache();

            TabShift(-1);
            WriteLine("}");
        }
        void WriteRowFromBytesFunction()
        {
            WriteLine("override public void FromBytes(BytesBuffer buffer)");
            WriteLine("{");
            TabShift(1);
            if (csvImporter.parentFileImporter != null)
                WriteLine("base.FromBytes(buffer);");
            for (int i = 0; i < csvImporter.keys.Length; i++)
            {
                string key = csvImporter.keys[i];
                if (string.IsNullOrEmpty(key)) continue;
                string type = csvImporter.types[i];
                string flag = csvImporter.flags[i];
                if (csvImporter.parentFileImporter != null && flag.Contains("M")) continue;
                string defaultValue = csvImporter.defaults[i];
                bool isComment = flag.Contains("N");
                bool isEditorFields = ConfigFileImporter.IsFilterNotUseColoum(flag);
                if (isComment || isEditorFields)
                    WriteLine("#if UNITY_STANDALONE || SERVER_EDITOR");
                if (flag.Contains("R"))//添加清除引用cache的代码，用于配置热加载
                {
                    string lowerName = ConvertUtil.ToFirstCharlower(key);
                    string cacheFieldName = "_" + lowerName + "Cache";
                    WriteLine("{0} = null;", cacheFieldName);
                }
                if (type.StartsWith("List<"))//数组
                    WriteListFromBytes(key, type, flag, defaultValue);
                else
                    WriteBasicFromBytes(key, type, flag, defaultValue);
                var majorType = csvImporter.majorKeyType;
                if (isComment || isEditorFields)
                    WriteLine("#endif");
            }
            WriteLine("#if UNITY_STANDALONE || SERVER_EDITOR");
            WriteLine("rowIndex = buffer.ReadInt32();");
            WriteLine("#endif");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteRowExportCsvFunction()
        {
            WriteLine("#if UNITY_STANDALONE || SERVER_EDITOR");
            if (csvImporter.parentFileImporter != null)
                WriteLine("public new void ExportCsvRow(StreamWriter writer)");
            else
                WriteLine("public void ExportCsvRow(StreamWriter writer)");
            WriteLine("{");
            TabShift(1);
            for (int i = 0; i < csvImporter.keys.Length; i++)
            {
                string key = csvImporter.keys[i];
                if (string.IsNullOrEmpty(key)) continue;
                string type = csvImporter.types[i];
                string flag = csvImporter.flags[i];
                string defaultValue = csvImporter.defaults[i];
                if (type.StartsWith("List<")) // 数组
                    WriteListExportCsv(key, type, flag, defaultValue);
                else
                    WriteBasicExportCsv(key, type, flag, defaultValue);
                if (i < csvImporter.keys.Length - 1)
                    WriteLine("writer.Write(XTable.SEPARATOR);");
            }
            TabShift(-1);
            WriteLine("}");
            WriteLine("#endif");
        }
        void WriteRowCloneFunction()
        {
            WriteLine("#if UNITY_STANDALONE || SERVER_EDITOR");
            if (csvImporter.parentFileImporter != null)
                WriteLine("public new {0} Clone()", csvImporter.rowClassName);
            else
                WriteLine("public {0} Clone()", csvImporter.rowClassName);
            WriteLine("{");
            TabShift(1);

            WriteLine("{0} row = new {0}();", csvImporter.rowClassName);
            // 如果是继承子表，需要先把父类的字段赋值。
            if (csvImporter.parentFileImporter != null)
            {
                List<ConfigFileImporter> parentImporters = new List<ConfigFileImporter>();
                ConfigFileImporter parentImporter = csvImporter.parentFileImporter;
                while (parentImporter != null)
                {
                    parentImporters.Insert(0, parentImporter);
                    parentImporter = parentImporter.parentFileImporter;
                }
                Dictionary<string, bool> csvImporterKeyDic = new Dictionary<string, bool>();
                for (int i = 0; i < csvImporter.keys.Length; i++)
                {
                    string key = csvImporter.keys[i];
                    string flag = csvImporter.flags[i];
                    if (string.IsNullOrEmpty(key) || flag.Contains("M")) continue;
                    if (!csvImporterKeyDic.ContainsKey(key)) csvImporterKeyDic.Add(key, true);
                }
                WriteLine("//子类先拷贝父类的变量值");
                for (int i = 0; i < parentImporters.Count; i++)
                {
                    parentImporter = parentImporters[i];
                    for (int j = 0; j < parentImporter.keys.Length; j++)
                    {
                        string key = parentImporter.keys[j];
                        if (string.IsNullOrEmpty(key) || csvImporterKeyDic.ContainsKey(key))
                            continue;
                        string type = parentImporter.types[j];
                        string flag = parentImporter.flags[j];
                        string defaultValue = parentImporter.defaults[j];
                        if (type.StartsWith("List<")) // 数组
                            WriteListClone(key, type, flag, defaultValue);
                        else
                            WriteLine("row.{0}_editor = {0}_editor;", GetFinalKeyStr(key, type, flag));
                    }
                }
            }

            for (int i = 0; i < csvImporter.keys.Length; i++)
            {
                string key = csvImporter.keys[i];
                if (string.IsNullOrEmpty(key)) continue;
                string type = csvImporter.types[i];
                string flag = csvImporter.flags[i];
                if (csvImporter.parentFileImporter != null && flag.Contains("M")) continue;
                string defaultValue = csvImporter.defaults[i];
                if (type.StartsWith("List<")) // 数组
                    WriteListClone(key, type, flag, defaultValue);
                else
                    WriteLine("row.{0}_editor = _{0};", GetFinalKeyStr(key, type, flag));
            }
            WriteLine("row.rowIndex = rowIndex;");
            WriteLine("return row;");
            TabShift(-1);
            WriteLine("}");

            WriteLine("/// <summary>");
            WriteLine("/// 深拷贝数据，但不修改实例的内存地址");
            WriteLine("/// </summary>");
            if (csvImporter.parentFileImporter != null)
                WriteLine("public new void Clone({0} row)", csvImporter.rowClassName);
            else
                WriteLine("public void Clone({0} row)", csvImporter.rowClassName);
            WriteLine("{");
            TabShift(1);

            // 如果是继承子表，需要先把父类的字段赋值。
            if (csvImporter.parentFileImporter != null)
            {
                List<ConfigFileImporter> parentImporters = new List<ConfigFileImporter>();
                ConfigFileImporter parentImporter = csvImporter.parentFileImporter;
                while (parentImporter != null)
                {
                    parentImporters.Insert(0, parentImporter);
                    parentImporter = parentImporter.parentFileImporter;
                }
                Dictionary<string, bool> csvImporterKeyDic = new Dictionary<string, bool>();
                for (int i = 0; i < csvImporter.keys.Length; i++)
                {
                    string key = csvImporter.keys[i];
                    string flag = csvImporter.flags[i];
                    if (string.IsNullOrEmpty(key) || flag.Contains("M")) continue;
                    if (!csvImporterKeyDic.ContainsKey(key)) csvImporterKeyDic.Add(key, true);
                }
                WriteLine("//子类先拷贝父类的变量值");
                for (int i = 0; i < parentImporters.Count; i++)
                {
                    parentImporter = parentImporters[i];
                    for (int j = 0; j < parentImporter.keys.Length; j++)
                    {
                        string key = parentImporter.keys[j];
                        if (string.IsNullOrEmpty(key) || csvImporterKeyDic.ContainsKey(key))
                            continue;
                        string type = parentImporter.types[j];
                        string flag = parentImporter.flags[j];
                        string defaultValue = parentImporter.defaults[j];
                        if (flag.Contains("R"))//添加清除引用cache的代码，用于配置热重载
                        {
                            WriteLine("row.{0}_editor = null;", key);
                        }
                        if (type.StartsWith("List<")) // 数组
                            WriteListClone(key, type, flag, defaultValue);
                        else
                            WriteLine("row.{0}_editor = {0}_editor;", GetFinalKeyStr(key, type, flag));
                    }
                }
            }

            for (int i = 0; i < csvImporter.keys.Length; i++)
            {
                string key = csvImporter.keys[i];
                if (string.IsNullOrEmpty(key)) continue;
                string type = csvImporter.types[i];
                string flag = csvImporter.flags[i];
                if (csvImporter.parentFileImporter != null && flag.Contains("M")) continue;
                string defaultValue = csvImporter.defaults[i];
                if (flag.Contains("R"))//添加清除引用cache的代码，用于配置热重载
                {
                    WriteLine("row.{0}_editor = null;", key);
                }
                if (type.StartsWith("List<")) // 数组
                    WriteListClone(key, type, flag, defaultValue);
                else
                    WriteLine("row.{0}_editor = _{0};", GetFinalKeyStr(key, type, flag));
            }

            WriteLine("row.rowIndex = rowIndex;");
            TabShift(-1);
            WriteLine("}");

            // 根据配置表的默认值创建一个行实例
            if (csvImporter.parentFileImporter != null)
                WriteLine("public static new {0} CloneDefault()", csvImporter.rowClassName);
            else
                WriteLine("public static {0} CloneDefault()", csvImporter.rowClassName);

            WriteLine("{");
            TabShift(1);

            WriteLine("{0} row = new {0}();", csvImporter.rowClassName);
            // 如果是继承子表，需要先把父类的字段赋值。
            if (csvImporter.parentFileImporter != null)
            {
                List<ConfigFileImporter> parentImporters = new List<ConfigFileImporter>();
                ConfigFileImporter parentImporter = csvImporter.parentFileImporter;
                while (parentImporter != null)
                {
                    parentImporters.Insert(0, parentImporter);
                    parentImporter = parentImporter.parentFileImporter;
                }
                Dictionary<string, bool> csvImporterKeyDic = new Dictionary<string, bool>();
                for (int i = 0; i < csvImporter.keys.Length; i++)
                {
                    string key = csvImporter.keys[i];
                    string flag = csvImporter.flags[i];
                    if (string.IsNullOrEmpty(key) || flag.Contains("M")) continue;
                    if (!csvImporterKeyDic.ContainsKey(key)) csvImporterKeyDic.Add(key, true);
                }
                for (int i = 0; i < parentImporters.Count; i++)
                {
                    parentImporter = parentImporters[i];
                    for (int j = 0; j < parentImporter.keys.Length; j++)
                    {
                        string key = parentImporter.keys[j];
                        if (string.IsNullOrEmpty(key) || csvImporterKeyDic.ContainsKey(key))
                            continue;
                        string type = parentImporter.types[j];
                        string flag = parentImporter.flags[j];
                        string defaultValue = parentImporter.defaults[j];
                        if (type.StartsWith("List<")) // 数组
                            WriteListCloneDefault(key, type, flag, defaultValue);
                        else
                        {
                            if (!string.IsNullOrEmpty(defaultValue))
                                WriteLine("row.{0}_editor = {1};", GetFinalKeyStr(key, type, flag), defaultValue);
                        }
                    }
                }
            }

            for (int i = 0; i < csvImporter.keys.Length; i++)
            {
                string key = csvImporter.keys[i];
                if (string.IsNullOrEmpty(key)) continue;
                string type = csvImporter.types[i];
                string flag = csvImporter.flags[i];
                if (csvImporter.parentFileImporter != null && flag.Contains("M")) continue;
                string defaultValue = csvImporter.defaults[i];
                if (type.StartsWith("List<")) // 数组
                    WriteListCloneDefault(key, type, flag, defaultValue);
                else
                {
                    if (type == "string" && defaultValue == "null")
                        WriteLine("row.{0}_editor = string.Empty;", GetFinalKeyStr(key, type, flag));
                    else
                    {
                        if (!string.IsNullOrEmpty(defaultValue))
                            WriteLine("row.{0}_editor = {1};", GetFinalKeyStr(key, type, flag), defaultValue);
                    }
                }
            }
            WriteLine("return row;");
            TabShift(-1);
            WriteLine("}");

            WriteLine("#endif");
        }

        void WriteRowClearReadOnlyCache()
        {
            if (readOnlyCacheList.Count == 0) return;

            WriteLine("override public void ClearReadOnlyCache()", csvImporter.rowClassName);
            WriteLine("{");
            TabShift(1);

            foreach (var readOnlyCacheMemberName in readOnlyCacheList)
            {
                WriteLine($"{readOnlyCacheMemberName} = null;");
            }

            TabShift(-1);
            WriteLine("}");
        }

        void WriteListCloneDefault(string key, string type, string flag, string defaultValue)
        {
            string finalKey = GetFinalKeyStr(key, type, flag);
            if (flag.Contains("R") || type.Contains("DateTime") || type.Contains("date")) // 引用类型和日期类型
            {
                WriteLine("row.{0}_editor = new List<string>();", finalKey);
            }
            else
            {
                WriteLine("row.{0}_editor = {1};", finalKey, defaultValue);
            }
        }
        void WriteListClone(string key, string type, string flag, string defaultValue)
        {
            string finalKey = GetFinalKeyStr(key, type, flag);
            if (flag.Contains("R") || type.Contains("DateTime") || type.Contains("date")) // 引用类型和日期类型
            {
                WriteLine("row.{0}_editor = new List<string>();", finalKey);
            }
            else
            {
                WriteLine("row.{0}_editor = {1};", finalKey, defaultValue);
            }
            WriteLine("if({0}_editor != null)", finalKey);
            WriteLine(1, "row.{0}_editor.AddRange({0}_editor);", finalKey);

        }
        void WriteListFromBytes(string key, string type, string flag, string defaultValue)
        {
            int startIdx = type.IndexOf('<');
            int endIdx = type.IndexOf('>');
            string itemType = type.Substring(startIdx + 1, endIdx - startIdx - 1);//数组项的类型
            string finalKey = GetFinalKeyStr(key, type, flag);
            WriteLine("if (buffer.ReadByte() == 1)");
            WriteLine("{");
            TabShift(1);
            WriteLine("byte itemCount = buffer.ReadByte();");
            WriteLine("if (_{0} != null) _{0}.Clear();", finalKey);//如果有设置值，清掉默认值，用于配置热加载
            if (flag.Contains("R") || itemType.Contains("DateTime") || itemType.Contains("date"))//引用类型
            {
                //WriteLine("if (itemCount > 0) _{0} = new List<string>();", finalKey);
                WriteLine("else _{0} = new List<string>();", finalKey);
            }
            else//非引用类型
            {
                //WriteLine("if (itemCount > 0) _{0} = {1};", finalKey, defaultValue);
                WriteLine("else _{0} = {1};", finalKey, defaultValue);
            }
            WriteLine("for (int i = 0; i < itemCount; i++)");

            // 用户定义的配置表字段类型
            Type t = AssemblyUtil.GetType(itemType);
            if (t != null && typeof(IUserDefineType).IsAssignableFrom(t))
            {
                WriteLine("{");
                WriteLine(1, "{0} temp = new {1}();", itemType, itemType);
                WriteLine(1, "temp.ReadFromBytes(buffer) ;");
                WriteLine(1, "_{0}.Add(temp);", finalKey);
                WriteLine("}");
            }
            // 基础类型
            else
            {
                WriteLine(1, "_{0}.Add({1});", finalKey, EditorUtil.GetBufferReadStr(itemType, flag));
            }

            TabShift(-1);
            WriteLine("}");
            if (flag.Contains("R") || itemType.Contains("DateTime") || itemType.Contains("date"))//引用类型
            {
                WriteLine("else _{0} = new List<string>();", finalKey);
            }
            else//非引用类型
            {
                WriteLine("else _{0} = {1};", finalKey, defaultValue);
            }
        }
        void WriteListExportCsv(string key, string type, string flag, string defaultValue)
        {
            string finalKey = GetFinalKeyStr(key, type, flag);
            WriteLine("if (_{0} != null && _{0}.Count > 0)", finalKey);
            WriteLine("{");
            TabShift(1);
            WriteLine("for (int i = 0; i < _{0}.Count; i++)", finalKey);
            WriteLine("{");
            TabShift(1);
            WriteLine("writer.Write({0});", GetWriteCsvStr(key, type, flag));
            WriteLine("if (i < _{0}.Count - 1)", finalKey);
            WriteLine(1, "writer.Write(\"|\");");
            TabShift(-1);
            WriteLine("}");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteBasicFromBytes(string key, string type, string flag, string defaultValue)
        {
            string finalKeyStr = GetFinalKeyStr(key, type, flag);
            // 用户定义的配置表字段类型
            Type t = AssemblyUtil.GetType(type);
            if (t != null && typeof(IUserDefineType).IsAssignableFrom(t))
            {
                WriteLine("if (buffer.ReadByte() == 1)");
                WriteLine("{");
                TabShift(1);
                WriteLine("_{0} = new {1}();", finalKeyStr, type);
                WriteLine("_{0}.ReadFromBytes(buffer);", finalKeyStr);
                TabShift(-1);
                WriteLine("}");
            }
            // 基础类型
            else
            {
                WriteLine("if (buffer.ReadByte() == 1) _{0} = {1};", finalKeyStr, EditorUtil.GetBufferReadStr(type, flag));
            }

            if (defaultValue == null || defaultValue == "null")
            {
                WriteLine("else _{0} = null;", finalKeyStr);
            }
            else
            {
                WriteLine("else _{0} = {1};", finalKeyStr, defaultValue);
            }
        }
        void WriteBasicExportCsv(string key, string type, string flag, string defaultValue)
        {
            string finalKeyStr = GetFinalKeyStr(key, type, flag);
            if (string.IsNullOrEmpty(defaultValue) || flag.Contains("M") || flag.Contains("L")/*|| flag.Contains("S")*/)
            {
                WriteLine("writer.Write({0});", GetWriteCsvStr(key, type, flag));
            }
            else
            {
                WriteLine("writer.Write(_{0} == {1} ? \"\" : {2});", finalKeyStr, defaultValue, GetWriteCsvStr(key, type, flag));
            }
        }
        string GetWriteCsvStr(string key, string type, string flag)
        {
            string finalKeyStr = GetFinalKeyStr(key, type, flag);
            if (type.StartsWith("List<"))
            {
                finalKeyStr += "[i]";
                int startIdx = type.IndexOf('<');
                int endIdx = type.IndexOf('>');
                type = type.Substring(startIdx + 1, endIdx - startIdx - 1);//数组项的类型
            }
            if (type.StartsWith("Enum"))
                return string.Format("((int)_{0}).ToString()", finalKeyStr);
            else if (flag.Contains("L")) // 解析掩码
                return string.Format("ConvertUtil.ConvertLayerIntToCfgStr(_{0})", finalKeyStr);
            else if (flag.Contains("M") && csvImporter.parentFileImporter != null) // 是子表
                return string.Format("{0}.ToString()", finalKeyStr);
            else
            {
                if (!string.IsNullOrEmpty(flag) && flag.Contains("R")) // 引用类型，type为string
                    type = "string";
                string getFuncStr = "";
                switch (type)
                {
                    case "bool":
                        getFuncStr = string.Format("_{0} ? \"1\" : \"0\"", finalKeyStr);
                        break;
                    case "short":
                        getFuncStr = string.Format("_{0}.ToString()", finalKeyStr);
                        break;
                    case "ushort":
                        getFuncStr = string.Format("_{0}.ToString()", finalKeyStr);
                        break;
                    case "int":
                        getFuncStr = string.Format("_{0}.ToString()", finalKeyStr);
                        break;
                    case "uint":
                        getFuncStr = string.Format("_{0}.ToString()", finalKeyStr);
                        break;
                    case "long":
                        getFuncStr += string.Format("_{0}.ToString()", finalKeyStr);
                        break;
                    case "float":
                        getFuncStr += string.Format("_{0}.ToString()", finalKeyStr);
                        break;
                    case "FixFloat":
                        getFuncStr += string.Format("_{0}.ToString()", finalKeyStr);
                        break;
                    case "string":
                    case "DateTime":
                    case "date":
                        getFuncStr = string.Format("string.IsNullOrEmpty(_{0}) ? null : _{0}.Replace(\"\\n\",\"\\\\n\")", finalKeyStr);
                        break;
                    case "Vector2":
                        getFuncStr = "string.Format(\"({0}#{1})\"," + string.Format("_{0}.x, _{0}.y)", finalKeyStr);
                        break;
                    case "Vector3":
                        getFuncStr = "string.Format(\"({0}#{1}#{2})\"," + string.Format("_{0}.x, _{0}.y, _{0}.z)", finalKeyStr);
                        break;
                    case "FixVector3":
                        getFuncStr = "string.Format(\"({0}#{1}#{2})\"," + string.Format("_{0}.x, _{0}.y, _{0}.z)", finalKeyStr);
                        break;
                    case "Vector4":
                        getFuncStr = "string.Format(\"({0}#{1}#{2}#{3})\"," + string.Format("_{0}.x, _{0}.y, _{0}.z, _{0}.w)", finalKeyStr);
                        break;
                    case "Color":
                        getFuncStr = "string.Format(\"({0}#{1}#{2}#{3})\"," + string.Format("_{0}.r * 255, _{0}.g * 255, _{0}.b * 255, _{0}.a)", finalKeyStr);
                        break;
                    default:
                        // 用户定义的配置表字段类型
                        Type t = AssemblyUtil.GetType(type);
                        if (t != null && typeof(IUserDefineType).IsAssignableFrom(t))
                        {
                            return finalKeyStr + ".WriteToString()";
                        }

                        if (t.IsEnum)
                        {
                            DebugUtil.Assert(false, "枚举{0}命名不规范, 请详读doc\\coder\\规范文档\\客户端代码编程规范.txt", type);
                        }

                        DebugUtil.Assert(false, "不支持的数据类型：" + type);
                        break;
                }
                return getFuncStr;
            }
        }
        string GetFinalKeyStr(string key, string type, string flag)
        {
            //需要二次处理字段，其key值要修改
            if (type.StartsWith("DateTime") || type.StartsWith("date"))
                return key + "Str";
            else if (type.StartsWith("List<DateTime") || type.StartsWith("List<date"))
                return key + "Strs";
            else if (flag.Contains("R"))
            {
                if (type.StartsWith("List<"))
                    return key + "Ids";
                else
                    return key + "Id";
            }
            return key;
        }
        void WriteDateTime(string name, string type, string flag, bool isEditorMode = false)
        {
            string lowerName = ConvertUtil.ToFirstCharlower(name);
            string idFieldName = name + "Str";
            string cacheFieldName = "_" + lowerName + "Cache";
            //WriteLine("[CsvDateTime(\"{0}\")]", name);
            WriteLine("private string _{0};", idFieldName);
            if (isEditorMode == false)
                WriteLine("public string {0}{{ get {{ return _{0}; }}}}", idFieldName);
            else
                WriteLine("private string {0}{{ get {{ return _{0}; }}}}", idFieldName);
            WriteLine("private DateTime {0} = DateTime.MinValue;", cacheFieldName);
            if (isEditorMode == false)
                WriteLine("public DateTime {0}", name);
            else
                WriteLine("private DateTime {0}", name);
            WriteLine("{");
            TabShift(1);
            WriteLine("get");
            WriteLine("{");
            TabShift(1);
            WriteLine("if (string.IsNullOrEmpty({0})) return DateTime.MinValue;", idFieldName);
            WriteLine("if ({0} == DateTime.MinValue)", cacheFieldName);
            WriteLine("{");
            TabShift(1);
            WriteLine("string[] timeArr = {0}.Split(' ');", idFieldName);
            WriteLine("string[] ymd = timeArr[0].Split('/');");
            WriteLine("int year = int.Parse(ymd[0]);");
            WriteLine("int month = int.Parse(ymd[1]);");
            WriteLine("int day = int.Parse(ymd[2]);");
            WriteLine("int hour = 0;");
            WriteLine("int minute = 0;");
            WriteLine("int second = 0;");
            WriteLine("if (timeArr.Length > 1)");
            WriteLine("{");
            TabShift(1);
            WriteLine("string[] hms = timeArr[1].Split(':');");
            WriteLine("if (hms.Length > 0)");
            TabShift(1);
            WriteLine("hour = int.Parse(hms[0]);");
            TabShift(-1);
            WriteLine("if (hms.Length > 1)");
            TabShift(1);
            WriteLine("minute = int.Parse(hms[1]);");
            TabShift(-1);
            WriteLine("if (hms.Length > 2)");
            TabShift(1);
            WriteLine("second = int.Parse(hms[2]);");
            TabShift(-1);
            TabShift(-1);
            WriteLine("}");
            WriteLine("{0} = new DateTime(year, month, day, hour, minute, second);", cacheFieldName);
            TabShift(-1);
            WriteLine("}");
            WriteLine("return {0};", cacheFieldName);
            TabShift(-1);
            WriteLine("}");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteDateTime_editor(string name, string type, string flag)
        {
            string lowerName = ConvertUtil.ToFirstCharlower(name);
            string idFieldName = name + "Str";
            string cacheFieldName = "_" + lowerName + "Cache";
            WriteLine("public string {0}_editor{{ get {{ return _{0}; }} set {{ _{0} = value; {1} = DateTime.MinValue; }}}}", idFieldName, cacheFieldName);
            WriteLine("public DateTime {0}_editor", name);
            WriteLine("{");
            TabShift(1);
            WriteLine("get");
            WriteLine("{");
            TabShift(1);
            WriteLine("if (string.IsNullOrEmpty({0})) return DateTime.MinValue;", idFieldName);
            WriteLine("if ({0} == DateTime.MinValue)", cacheFieldName);
            WriteLine("{");
            TabShift(1);
            WriteLine("string[] timeArr = {0}.Split(' ');", idFieldName);
            WriteLine("string[] ymd = timeArr[0].Split('/');");
            WriteLine("int year = int.Parse(ymd[0]);");
            WriteLine("int month = int.Parse(ymd[1]);");
            WriteLine("int day = int.Parse(ymd[2]);");
            WriteLine("int hour = 0;");
            WriteLine("int minute = 0;");
            WriteLine("int second = 0;");
            WriteLine("if (timeArr.Length > 1)");
            WriteLine("{");
            TabShift(1);
            WriteLine("string[] hms = timeArr[1].Split(':');");
            WriteLine("if (hms.Length > 0)");
            TabShift(1);
            WriteLine("hour = int.Parse(hms[0]);");
            TabShift(-1);
            WriteLine("if (hms.Length > 1)");
            TabShift(1);
            WriteLine("minute = int.Parse(hms[1]);");
            TabShift(-1);
            WriteLine("if (hms.Length > 2)");
            TabShift(1);
            WriteLine("second = int.Parse(hms[2]);");
            TabShift(-1);
            TabShift(-1);
            WriteLine("}");
            WriteLine("{0} = new DateTime(year, month, day, hour, minute, second);", cacheFieldName);
            TabShift(-1);
            WriteLine("}");
            WriteLine("return {0};", cacheFieldName);
            TabShift(-1);
            WriteLine("}");
            WriteLine("set");
            WriteLine("{");
            TabShift(1);
            WriteLine("{0} = value;", cacheFieldName);
            TabShift(-1);
            WriteLine("}");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteListDateTime(string name, string type, string flag, bool isEditorMode = false)
        {
            string lowerName = ConvertUtil.ToFirstCharlower(name);
            string idsFieldName = name + "Strs";
            string cacheIdsReadOnlyKey = $"_{idsFieldName}ReadOnlyCache";
            string cacheFieldName = "_" + lowerName + "Caches";
            string cacheFieldNameReadOnly = $"_{lowerName}ReadOnlyCache";
            //WriteLine("[CsvDateTime(\"{0}\")]", name);
            WriteLine("private List<string> _{0};", idsFieldName);
            WriteLine("private ReadOnlyCollection<string> {0};", cacheIdsReadOnlyKey);
            readOnlyCacheList.Add(cacheIdsReadOnlyKey);
            readOnlyCacheList.Add(cacheFieldNameReadOnly);

            // ids
            if (isEditorMode == false)
                WriteLine("public ReadOnlyCollection<string> {0}", idsFieldName);
            else
                WriteLine("private ReadOnlyCollection<string> {0}", idsFieldName);
            WriteLine("{");
            TabShift(1);

            WriteLine("get");
            WriteLine("{");
            TabShift(1);

            WriteLine("if ({0} == null)", cacheIdsReadOnlyKey);
            WriteLine(1, "{0} = _{1}.AsReadOnly();", cacheIdsReadOnlyKey, idsFieldName);
            WriteLine("return {0};", cacheIdsReadOnlyKey);

            TabShift(-1);
            WriteLine("}");

            TabShift(-1);
            WriteLine("}");

            // list
            WriteLine("private List<DateTime> {0};", cacheFieldName);
            WriteLine("private ReadOnlyCollection<DateTime> {0};", cacheFieldNameReadOnly);
            if (isEditorMode == false)
                WriteLine("public ReadOnlyCollection<DateTime> {0}", name);
            else
                WriteLine("private ReadOnlyCollection<DateTime> {0}", name);
            WriteLine("{");
            TabShift(1);

            WriteLine("get");
            WriteLine("{");
            TabShift(1);

            WriteLine("if ({0} == null)", cacheFieldName);
            WriteLine("{");
            TabShift(1);

            WriteLine("{0} = new List<DateTime>();", cacheFieldName);
            WriteLine("for (int i = 0; i < {0}.Count; i++)", idsFieldName);
            WriteLine("{");
            TabShift(1);
            WriteLine("string[] timeArr = {0}[i].Split(' ');", idsFieldName);
            WriteLine("string[] ymd = timeArr[0].Split('/');");
            WriteLine("int year = int.Parse(ymd[0]);");
            WriteLine("int month = int.Parse(ymd[1]);");
            WriteLine("int day = int.Parse(ymd[2]);");
            WriteLine("int hour = 0;");
            WriteLine("int minute = 0;");
            WriteLine("int second = 0;");
            WriteLine("if (timeArr.Length > 1)");
            WriteLine("{");
            TabShift(1);
            WriteLine("string[] hms = timeArr[1].Split(':');");
            WriteLine("if (hms.Length > 0)");
            TabShift(1);
            WriteLine("hour = int.Parse(hms[0]);");
            TabShift(-1);
            WriteLine("if (hms.Length > 1)");
            TabShift(1);
            WriteLine("minute = int.Parse(hms[1]);");
            TabShift(-1);
            WriteLine("if (hms.Length > 2)");
            TabShift(1);
            WriteLine("second = int.Parse(hms[2]);");
            TabShift(-1);
            TabShift(-1);
            WriteLine("}");
            WriteLine("{0}.Add(new DateTime(year, month, day, hour, minute, second));", cacheFieldName);
            TabShift(-1);
            WriteLine("}");

            TabShift(-1);
            WriteLine("}"); // if cacheFieldName == null end

            WriteLine("if ({0} == null)", cacheFieldNameReadOnly);
            WriteLine(1, "{0} = {1}.AsReadOnly();", cacheFieldNameReadOnly, cacheFieldName);

            WriteLine("return {0};", cacheFieldNameReadOnly);

            TabShift(-1);
            WriteLine("}"); // end get

            TabShift(-1);
            WriteLine("}");
        }
        void WriteListDateTime_editor(string name, string type, string flag)
        {
            string lowerName = ConvertUtil.ToFirstCharlower(name);
            string idsFieldName = name + "Strs";
            string cacheIdsReadOnlyKey = $"_{idsFieldName}ReadOnlyCache";
            string cacheFieldName = "_" + lowerName + "Caches";
            string cacheFieldNameReadOnly = $"_{lowerName}ReadOnlyCache";
            //WriteLine("[CsvDateTime(\"{0}\")]", name);
            readOnlyCacheList.Add(cacheIdsReadOnlyKey);
            readOnlyCacheList.Add(cacheFieldNameReadOnly);
            WriteLine("public List<string> {0}_editor{{ get {{ return _{0}; }} set {{ _{0} = value; {1} = null; {2} = null; {3} = null;}}}}", idsFieldName, cacheIdsReadOnlyKey, cacheFieldName, cacheFieldNameReadOnly);
            WriteLine("public List<DateTime> {0}_editor", name);
            WriteLine("{");
            TabShift(1);
            WriteLine("get");
            WriteLine("{");
            TabShift(1);
            WriteLine("if ({0} == null)", cacheFieldName);
            WriteLine("{");
            TabShift(1);
            WriteLine("{0} = new List<DateTime>();", cacheFieldName);
            WriteLine("for (int i = 0; i < {0}.Count; i++)", idsFieldName);
            WriteLine("{");
            TabShift(1);
            WriteLine("string[] timeArr = {0}[i].Split(' ');", idsFieldName);
            WriteLine("string[] ymd = timeArr[0].Split('/');");
            WriteLine("int year = int.Parse(ymd[0]);");
            WriteLine("int month = int.Parse(ymd[1]);");
            WriteLine("int day = int.Parse(ymd[2]);");
            WriteLine("int hour = 0;");
            WriteLine("int minute = 0;");
            WriteLine("int second = 0;");
            WriteLine("if (timeArr.Length > 1)");
            WriteLine("{");
            TabShift(1);
            WriteLine("string[] hms = timeArr[1].Split(':');");
            WriteLine("if (hms.Length > 0)");
            TabShift(1);
            WriteLine("hour = int.Parse(hms[0]);");
            TabShift(-1);
            WriteLine("if (hms.Length > 1)");
            TabShift(1);
            WriteLine("minute = int.Parse(hms[1]);");
            TabShift(-1);
            WriteLine("if (hms.Length > 2)");
            TabShift(1);
            WriteLine("second = int.Parse(hms[2]);");
            TabShift(-1);
            TabShift(-1);
            WriteLine("}");
            WriteLine("{0}.Add(new DateTime(year, month, day, hour, minute, second));", cacheFieldName);
            TabShift(-1);
            WriteLine("}");
            TabShift(-1);
            WriteLine("}");
            WriteLine("return {0};", cacheFieldName);
            TabShift(-1);
            WriteLine("}");
            WriteLine("set");
            WriteLine("{");
            TabShift(1);
            WriteLine("{0} = value;", cacheFieldName);
            TabShift(-1);
            WriteLine("}");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteReference(string key, string type, string flag, bool isEditorMode = false)
        {
            DebugUtil.Assert(context.fileName2ImporterDic.ContainsKey(type), "表{0} 列{1} 引用的表{2}并不存在", csvImporter.fileName, key, type);
            ConfigFileImporter importer = context.fileName2ImporterDic[type] as ConfigFileImporter;
            string lowerName = ConvertUtil.ToFirstCharlower(key);
            string referenceRowType = GetReferenceRowType(type);
            string idFieldName = key + "Id";
            string cacheFieldName = "_" + lowerName + "Cache";
            WriteLine("[SerializeField]");
            WriteLine("[CsvReference(\"{0}\")]", key);
            WriteLine("private string _{0};", idFieldName);
            if (isEditorMode == false)
                WriteLine("public string {0}{{ get {{ return _{0}; }}}}", idFieldName);
            else
                WriteLine("private string {0}{{ get {{ return _{0}; }}}}", idFieldName);
            WriteLine("private {0} {1};", referenceRowType, cacheFieldName);
            if (isEditorMode == false)
                WriteLine("public {0} {1}", referenceRowType, key);
            else
                WriteLine("private {0} {1}", referenceRowType, key);
            WriteLine("{");
            TabShift(1);
            WriteLine("get");
            WriteLine("{");
            TabShift(1);
            WriteLine("if (string.IsNullOrEmpty(_{0})) return null;", idFieldName);
            WriteLine("if ({0} == null)", cacheFieldName);
            WriteLine("{");
            TabShift(1);
            if (importer.majorKeyType == EnumTableMojorkeyType.INT)
                WriteLine("{0} = Config.Inst.{1}.GetRow(int.Parse({2}));", cacheFieldName, GetReferenceTableLowerClassName(type), idFieldName);
            else if (importer.majorKeyType == EnumTableMojorkeyType.STRING)
                WriteLine("{0} = Config.Inst.{1}.GetRow({2});", cacheFieldName, GetReferenceTableLowerClassName(type), idFieldName);
            else
                DebugUtil.Assert(false, "不支持引用的表是双主键表 {0}:{1}", importer.fileName, key);
            TabShift(-1);
            WriteLine("}");
            WriteLine("return {0};", cacheFieldName);
            TabShift(-1);
            WriteLine("}");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteReference_editor(string key, string type, string flag)
        {
            DebugUtil.Assert(context.fileName2ImporterDic.ContainsKey(type), "表{0} 列{1} 引用的表{2}并不存在", csvImporter.fileName, key, type);
            ConfigFileImporter importer = context.fileName2ImporterDic[type] as ConfigFileImporter;
            string lowerName = ConvertUtil.ToFirstCharlower(key);
            string referenceRowType = GetReferenceRowType(type);
            string idFieldName = key + "Id";
            string cacheFieldName = "_" + lowerName + "Cache";
            WriteLine("public string {0}_editor{{ get {{ return _{0}; }} set {{ _{0} = value; {1} = null; }}}}", idFieldName, cacheFieldName);
            WriteLine("public {0} {1}_editor", referenceRowType, key);
            WriteLine("{");
            TabShift(1);
            WriteLine("get");
            WriteLine("{");
            TabShift(1);
            WriteLine("if (string.IsNullOrEmpty(_{0})) return null;", idFieldName);
            WriteLine("if ({0} == null)", cacheFieldName);
            WriteLine("{");
            TabShift(1);
            if (importer.majorKeyType == EnumTableMojorkeyType.INT)
                WriteLine("{0} = Config.Inst.{1}.GetRow(int.Parse({2}));", cacheFieldName, GetReferenceTableLowerClassName(type), idFieldName);
            else if (importer.majorKeyType == EnumTableMojorkeyType.STRING)
                WriteLine("{0} = Config.Inst.{1}.GetRow({2});", cacheFieldName, GetReferenceTableLowerClassName(type), idFieldName);
            else
                DebugUtil.Assert(false, "不支持引用的表是双主键表 {0}:{1}", importer.fileName, key);
            TabShift(-1);
            WriteLine("}");
            WriteLine("return {0};", cacheFieldName);
            TabShift(-1);
            WriteLine("}");
            WriteLine("set");
            WriteLine("{");
            TabShift(1);
            WriteLine("{0} = value;", cacheFieldName);
            TabShift(-1);
            WriteLine("}");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteListReference(string key, string type, string flag, bool isEditorMode = false)
        {
            string lowerName = ConvertUtil.ToFirstCharlower(key);
            string referenceRowType = GetReferenceRowType(type);
            string readOnlyType = referenceRowType.Replace("List", "ReadOnlyCollection");
            string referenceTableName = GetReferenceTableName(type);
            string idsFieldName = key + "Ids";
            string cacheIdsReadOnlyKey = $"_{idsFieldName}ReadOnlyCache";
            string cachesFieldName = "_" + lowerName + "Cache";
            string cachesFieldNameReadOnly = $"_{lowerName}ReadOnlyCache";
            readOnlyCacheList.Add(cacheIdsReadOnlyKey);
            readOnlyCacheList.Add(cachesFieldNameReadOnly);
            WriteLine("[SerializeField]");
            WriteLine("[CsvReference(\"{0}\")]", key);
            WriteLine("private List<string> _{0};", idsFieldName);
            WriteLine("private ReadOnlyCollection<string> {0};", cacheIdsReadOnlyKey);

            // ids
            if (isEditorMode == false)
                WriteLine("public ReadOnlyCollection<string> {0}", idsFieldName);
            else
                WriteLine("private ReadOnlyCollection<string> {0}", idsFieldName);
            WriteLine("{");
            TabShift(1);

            WriteLine("get");
            WriteLine("{");
            TabShift(1);

            WriteLine("if ({0} == null)", cacheIdsReadOnlyKey);
            WriteLine(1, "{0} = _{1}.AsReadOnly();", cacheIdsReadOnlyKey, idsFieldName);
            WriteLine("return {0};", cacheIdsReadOnlyKey);

            TabShift(-1);
            WriteLine("}");

            TabShift(-1);
            WriteLine("}");

            // 
            WriteLine("private {0} {1};", referenceRowType, cachesFieldName);
            WriteLine("private {0} {1};", readOnlyType, cachesFieldNameReadOnly);
            if (isEditorMode == false)
                WriteLine("public {0} {1}", readOnlyType, key);
            else
                WriteLine("private {0} {1}", readOnlyType, key);
            WriteLine("{");
            TabShift(1);

            WriteLine("get");
            WriteLine("{");
            TabShift(1);

            WriteLine("if ({0} == null)", cachesFieldName);
            WriteLine("{");
            TabShift(1);

            WriteLine("{0} = new {1}();", cachesFieldName, referenceRowType);
            WriteLine("for (int i = 0; i < {0}.Count; i++)", idsFieldName);
            DebugUtil.Assert(context.fileName2ImporterDic.ContainsKey(referenceTableName), referenceTableName);
            ConfigFileImporter importer = context.fileName2ImporterDic[referenceTableName] as ConfigFileImporter;
            if (importer.majorKeyType == EnumTableMojorkeyType.INT)
                WriteLine(1, "{0}.Add(Config.Inst.{1}.GetRow(int.Parse({2}[i])));", cachesFieldName, GetReferenceTableLowerClassName(type), idsFieldName);
            else if (importer.majorKeyType == EnumTableMojorkeyType.STRING)
                WriteLine(1, "{0}.Add(Config.Inst.{1}.GetRow({2}[i]));", cachesFieldName, GetReferenceTableLowerClassName(type), idsFieldName);

            TabShift(-1);
            WriteLine("}"); // end if

            WriteLine("if ({0} == null)", cachesFieldNameReadOnly);
            WriteLine(1, "{0} = {1}.AsReadOnly();", cachesFieldNameReadOnly, cachesFieldName);

            WriteLine("return {0};", cachesFieldNameReadOnly);

            TabShift(-1);
            WriteLine("}"); // end get

            TabShift(-1);
            WriteLine("}");
        }

        /// <summary> 如果majorKey为int类型，那么额外导出一份数据 </summary>
        void WriteListReferencePlus(string key, string type, string flag, bool isEditorMode = false)
        {
            string referenceTableName = GetReferenceTableName(type);
            DebugUtil.Assert(context.fileName2ImporterDic.ContainsKey(referenceTableName), referenceTableName);
            ConfigFileImporter importer = context.fileName2ImporterDic[referenceTableName] as ConfigFileImporter;
            if (importer.majorKeyType != EnumTableMojorkeyType.INT)
                return;

            string srcIdsFieldName = $"_{key}Ids";
            string idsFieldName = key + "Ids2";
            string cacheIdsReadOnlyKey = $"_{idsFieldName}Caches";

            string mojorkeyTypeString = "int";
            WriteLine("[SerializeField]");
            WriteLine("[CsvReference(\"{0}\")]", key);

            WriteLine("private List<{0}> _{1};", mojorkeyTypeString, idsFieldName);
            WriteLine("private ReadOnlyCollection<{0}> {1};", mojorkeyTypeString, cacheIdsReadOnlyKey);

            WriteLine("public ReadOnlyCollection<{0}> {1}", mojorkeyTypeString, idsFieldName);
            WriteLine("{");
            TabShift(1);

            WriteLine("get");
            WriteLine("{");
            TabShift(1);

            WriteLine("if (_{0} == null)", idsFieldName);
            WriteLine("{");
            TabShift(1);
            WriteLine("_{0} = new List<{1}>();", idsFieldName, mojorkeyTypeString);
            WriteLine("foreach (var var in {0})", srcIdsFieldName);
            WriteLine(1, "_{0}.Add(int.Parse(var));", idsFieldName);
            TabShift(-1);
            WriteLine("}");

            WriteLine("if ({0} == null)", cacheIdsReadOnlyKey);
            WriteLine(1, "{0} = _{1}.AsReadOnly();", cacheIdsReadOnlyKey, idsFieldName);

            WriteLine("return {0};", cacheIdsReadOnlyKey);

            TabShift(-1);
            WriteLine("}");

            TabShift(-1);
            WriteLine("}");
        }

        void WriteListReference_editor(string key, string type, string flag)
        {
            string lowerName = ConvertUtil.ToFirstCharlower(key);
            string referenceRowType = GetReferenceRowType(type);
            string readOnlyType = referenceRowType.Replace("List", "ReadOnlyCollection");
            string referenceTableName = GetReferenceTableName(type);
            string idsFieldName = key + "Ids";
            string cacheIdsReadOnlyKey = $"_{idsFieldName}ReadOnlyCache";
            string cachesFieldName = "_" + lowerName + "Cache";
            string cachesFieldNameReadOnly = $"_{lowerName}ReadOnlyCache";
            readOnlyCacheList.Add(cacheIdsReadOnlyKey);
            readOnlyCacheList.Add(cachesFieldNameReadOnly);
            WriteLine("public List<string> {0}_editor{{ get {{ return _{0}; }} set {{ _{0} = value; {1} = null; {2} = null; {3} = null; }}}}", idsFieldName, cacheIdsReadOnlyKey, cachesFieldName, cachesFieldNameReadOnly);
            WriteLine("public {0} {1}_editor", referenceRowType, key);
            WriteLine("{");
            TabShift(1);
            WriteLine("get");
            WriteLine("{");
            TabShift(1);
            WriteLine("if ({0} == null)", cachesFieldName);
            WriteLine("{");
            TabShift(1);
            WriteLine("{0} = new {1}();", cachesFieldName, referenceRowType);
            WriteLine("for (int i = 0; i < {0}.Count; i++)", idsFieldName);
            DebugUtil.Assert(context.fileName2ImporterDic.ContainsKey(referenceTableName), referenceTableName);
            ConfigFileImporter importer = context.fileName2ImporterDic[referenceTableName] as ConfigFileImporter;
            if (importer.majorKeyType == EnumTableMojorkeyType.INT)
                WriteLine(1, "{0}.Add(Config.Inst.{1}.GetRow(int.Parse({2}[i])));", cachesFieldName, GetReferenceTableLowerClassName(type), idsFieldName);
            else if (importer.majorKeyType == EnumTableMojorkeyType.STRING)
                WriteLine(1, "{0}.Add(Config.Inst.{1}.GetRow({2}[i]));", cachesFieldName, GetReferenceTableLowerClassName(type), idsFieldName);
            TabShift(-1);
            WriteLine("}");
            WriteLine("return {0};", cachesFieldName);
            TabShift(-1);
            WriteLine("}");
            WriteLine("set");
            WriteLine("{");
            TabShift(1);
            WriteLine("{0} = value;", cachesFieldName);
            TabShift(-1);
            WriteLine("}");
            TabShift(-1);
            WriteLine("}");
        }

        void WriteConfigClass()
        {
            WriteLine("public partial class Config");
            WriteLine("{");
            TabShift(1);
            WriteLine("[BindCsvPath(\"{0}\")]", csvImporter.relativePath);
            WriteLine("public {0} {1} = new {0}();", csvImporter.tableClassName, ConvertUtil.ToFirstCharlower(csvImporter.tableClassName));
            TabShift(-1);
            WriteLine("}");
        }
        string GetReferenceRowType(string type)
        {
            if (type.StartsWith("List<"))
            {
                int index = type.IndexOf(">");
                string temp = type.Substring(5, index - 5);
                temp = ConvertUtil.Convert2HumpNamed(temp) + "Row";
                return "List<" + temp + ">";
            }
            else
                return ConvertUtil.Convert2HumpNamed(type) + "Row";
        }
        string GetReferenceTableName(string type)
        {
            return type.Replace("List<", "").Replace(">", "");
        }
        string GetReferenceTableLowerClassName(string type)
        {
            string fileName = type;
            if (type.StartsWith("List<"))
                fileName = fileName.Substring(5, fileName.IndexOf(">") - 5);
            string humpNamed = ConvertUtil.Convert2HumpNamed(fileName);
            humpNamed = ConvertUtil.ToFirstCharlower(humpNamed);
            return humpNamed + "Table";
        }
    }
}