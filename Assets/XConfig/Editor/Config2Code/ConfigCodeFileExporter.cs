﻿using System;
using System.Collections.Generic;

namespace XConfig.Editor
{
    public class ConfigCodeFileExporter : TextFileExporter
    {
        private const string CLASS_TEMPLATE =
            @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using XConfig;
";

        ConfigFileImporter importer;
        ConfigFileContext context;

        public ConfigCodeFileExporter(string outFilePath, ConfigFileImporter importer, ConfigFileContext context) : base(outFilePath)
        {
            this.importer = importer;
            this.context = context;
        }
        protected override void DoExport()
        {
            WriteLine(CLASS_TEMPLATE);//写类文件头
            WriteConfigCode();
            WriteTableCode();
            WriteRowCode();
        }
        void WriteTableCode()
        {
            WriteLine($"public partial class {importer.tableClassName} : XTable");
            WriteLine("{");
            TabShift(1);
            WriteLine($"public List<{importer.rowClassName}> rows {{ get {{ return _tableRows; }}}}");
            WriteTableInternalRows();
            WriteTableFromBytesFunction();

            switch (importer.mainKeyType)
            {
                case EnumTableMainKeyType.INT:
                    WriteInitFunction_int();
                    WriteGetRowFunction_int();
                    WriteTryGetRowFunction_int();
                    WriteContainsMajorKeyFunction_int();
                    WriteAddRowFunction_int();
                    break;

                case EnumTableMainKeyType.STRING:
                    WriteInitFunction_string();
                    WriteGetRowFunction_string();
                    WriteTryGetRowFunction_string();
                    WriteContainsMajorKeyFunction_string();
                    WriteAddRowFunction_string();
                    break;

                case EnumTableMainKeyType.INT_INT:
                    WriteInitFunction_int_int();
                    WriteGetRowFunction_int_int();
                    WriteTryGetRowFunction_int_int();
                    WriteContainsMajorKeyFunction_int_int();
                    WriteGetMajorKeyFunction_int_int();
                    WriteAddRowFunction_int_int();
                    break;

                default:
                    DebugUtil.Assert(false, $"非法的主键类型 : {importer.mainKeyType}");
                    break;
            }

            WriteAllTableInitCompleteFunction();
            TabShift(-1);
            WriteLine("}");
        }
        void WriteTableInternalRows()
        {
            WriteLine($"List<{importer.rowClassName}> _tableRows;");
        }
        void WriteTableFromBytesFunction()
        {
            WriteLine("override public void ReadFromBytes(BytesBuffer buffer)");
            WriteLine("{");
            TabShift(1);
            WriteLine("if (_tableRows == null)");
            WriteLine("{");
            TabShift(1);
            WriteLine("_tableRows = new List<{0}>();", importer.rowClassName);
            WriteLine("ushort rowCount = buffer.ReadUInt16();");
            WriteLine("for (int i = 0; i < rowCount; i++)");
            WriteLine("{");
            TabShift(1);
            WriteLine("{0} row = new {0}();", importer.rowClassName);
            WriteLine("row.ReadFromBytes(buffer);");
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
            WriteLine(1, "_tableRows[i].ReadFromBytes(buffer);");
            TabShift(-1);
            WriteLine("}");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteAllTableInitCompleteFunction()
        {
            WriteLine("override public void OnInit()");
            WriteLine("{");
            TabShift(1);
            WriteLine("for (int i = 0; i < _tableRows.Count; i++)");
            WriteLine(1, "_tableRows[i].OnAfterInit();");

            EmptyLine();
            if (importer.parentFileImporter != null)//是子表
            {
                ConfigFileImporter rootImporter = importer.parentFileImporter;
                while (rootImporter.parentFileImporter != null)
                    rootImporter = rootImporter.parentFileImporter;
                WriteLine("for (int i = 0; i < _tableRows.Count; i++)//子表才需要往总表添加");
                WriteLine(1, $"Config.Inst.{ConvertUtil.ToFirstCharLower(rootImporter.tableClassName)}.AddRow(_tableRows[i]);");
            }
            EmptyLine();
            WriteLine("OnAfterInit();");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteInitFunction_int()
        {
            WriteLine("Dictionary<int, {0}> _intMajorKey2Row;", importer.rowClassName);
            WriteLine("override public void Init()");
            WriteLine("{");
            TabShift(1);
            WriteLine("{0} row = null;", importer.rowClassName);
            WriteLine("_intMajorKey2Row = new Dictionary<int, {0}>();", importer.rowClassName);
            WriteLine("for (int i = 0; i < _tableRows.Count; i++)");
            WriteLine("{");
            TabShift(1);
            WriteLine("row = _tableRows[i];");
            WriteLine("int majorKey = row.{0};", importer.mainKeys[0]);
            WriteLine("DebugUtil.Assert(!_intMajorKey2Row.ContainsKey(majorKey), \"{0} 主键重复：{1}，请先按键盘【alt+r】导出配置试试！\", name, majorKey);");
            WriteLine("_intMajorKey2Row.Add(majorKey, row);");
            TabShift(-1);
            WriteLine("}");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteInitFunction_int_int()
        {
            WriteLine("Dictionary<long, {0}> _intMajorKey2Row;", importer.rowClassName);
            WriteLine("override public void Init()");
            WriteLine("{");
            TabShift(1);
            WriteLine("{0} row = null;", importer.rowClassName);
            WriteLine("_intMajorKey2Row = new Dictionary<long, {0}>();", importer.rowClassName);
            WriteLine("for (int i = 0; i < _tableRows.Count; i++)");
            WriteLine("{");
            TabShift(1);
            WriteLine("row = _tableRows[i];");
            WriteLine("long majorKey = ((long)row.{0}<<32) + row.{1};", importer.mainKeys[0], importer.mainKeys[1]);
            string temp = string.Format("row.{0}, row.{1}", importer.mainKeys[0], importer.mainKeys[1]);
            WriteLine("DebugUtil.Assert(!_intMajorKey2Row.ContainsKey(majorKey), \"{0} 主键重复：{1} {2}\", name, " + temp + ");");
            WriteLine("_intMajorKey2Row.Add(majorKey, row);");
            TabShift(-1);
            WriteLine("}");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteInitFunction_string()
        {
            WriteLine("Dictionary<string, {0}> _stringMajorKey2Row;", importer.rowClassName);
            WriteLine("override public void Init()");
            WriteLine("{");
            TabShift(1);
            WriteLine("{0} row = null;", importer.rowClassName);
            WriteLine("_stringMajorKey2Row = new Dictionary<string, {0}>();", importer.rowClassName);
            WriteLine("for (int i = 0; i < _tableRows.Count; i++)");
            WriteLine("{");
            TabShift(1);
            WriteLine("row = _tableRows[i];");
            WriteLine("string majorKey = row.{0};", importer.mainKeys[0]);
            WriteLine("DebugUtil.Assert(!_stringMajorKey2Row.ContainsKey(majorKey), \"{0} 主键重复：{1}\", name, majorKey);");
            WriteLine("_stringMajorKey2Row.Add(majorKey, row);");
            TabShift(-1);
            WriteLine("}");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteGetRowFunction_int()
        {
            WriteLine("virtual public {0} GetValue(int majorKey, bool isAssert=true)", importer.rowClassName);
            WriteLine("{");
            TabShift(1);
            WriteLine("{0} row;", importer.rowClassName);
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
            WriteLine("virtual public bool TryGetValue(int majorKey, out {0} row)", importer.rowClassName);
            WriteLine("{");
            TabShift(1);
            WriteLine("return _intMajorKey2Row.TryGetValue(majorKey, out row);");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteContainsMajorKeyFunction_int()
        {
            WriteLine("public bool ContainsKey(int majorKey)");
            WriteLine("{");
            TabShift(1);
            WriteLine("return _intMajorKey2Row.ContainsKey(majorKey);");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteAddRowFunction_int()
        {
            WriteLine("public void AddRow({0} row)", importer.rowClassName);
            WriteLine("{");
            TabShift(1);
            WriteLine("if (!_intMajorKey2Row.ContainsKey(row.{0}))", importer.mainKeys[0]);
            WriteLine("{");
            TabShift(1);
            WriteLine("_tableRows.Add(row);");
            WriteLine("_intMajorKey2Row.Add(row.{0}, row);", importer.mainKeys[0]);
            if (importer.parentFileImporter != null)
            {
                var rootImporter = importer.parentFileImporter;
                while (rootImporter.parentFileImporter != null)
                    rootImporter = rootImporter.parentFileImporter;
                WriteLine("Config.Inst.{0}.AddRow(row);//子表才需要往总表添加", ConvertUtil.ToFirstCharLower(rootImporter.tableClassName));
            }
            TabShift(-1);
            WriteLine("}");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteGetRowFunction_int_int()
        {
            WriteLine("virtual public {0} GetValue(int key1, int key2, bool isAssert=true)", importer.rowClassName);
            WriteLine("{");
            TabShift(1);
            WriteLine("{0} row;", importer.rowClassName);
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
            WriteLine("virtual public bool TryGetValue(int key1, int key2, out {0} row)", importer.rowClassName);
            WriteLine("{");
            TabShift(1);
            WriteLine("long majorKey = ((long)key1<<32) + key2;");
            WriteLine("return _intMajorKey2Row.TryGetValue(majorKey, out row);");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteContainsMajorKeyFunction_int_int()
        {
            WriteLine("public bool ContainsKey(int key1, int key2)");
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
            WriteLine("public void AddRow({0} row)", importer.rowClassName);
            WriteLine("{");
            TabShift(1);
            WriteLine("long majorKey = ((long)row.{0} << 32) + row.{1};", importer.mainKeys[0], importer.mainKeys[1]);
            WriteLine("if (!_intMajorKey2Row.ContainsKey(majorKey))");
            WriteLine("{");
            TabShift(1);
            WriteLine("_tableRows.Add(row);");
            WriteLine("_intMajorKey2Row.Add(majorKey, row);");
            if (importer.parentFileImporter != null)
            {
                var rootImporter = importer.parentFileImporter;
                while (rootImporter.parentFileImporter != null)
                    rootImporter = rootImporter.parentFileImporter;
                WriteLine("Config.Inst.{0}.AddRow(row);//子表才需要往总表添加", ConvertUtil.ToFirstCharLower(rootImporter.tableClassName));
            }
            TabShift(-1);
            WriteLine("}");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteGetRowFunction_string()
        {
            WriteLine("virtual public {0} GetValue(string majorKey, bool isAssert=true)", importer.rowClassName);
            WriteLine("{");
            TabShift(1);
            WriteLine("{0} row;", importer.rowClassName);
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
            WriteLine("virtual public bool TryGetValue(string majorKey, out {0} row)", importer.rowClassName);
            WriteLine("{");
            WriteLine(1, "return _stringMajorKey2Row.TryGetValue(majorKey, out row);");
            WriteLine("}");
        }
        void WriteContainsMajorKeyFunction_string()
        {
            WriteLine("public bool ContainsKey(string majorKey)");
            WriteLine("{");
            WriteLine(1, "return _stringMajorKey2Row.ContainsKey(majorKey);");
            WriteLine("}");
        }
        void WriteAddRowFunction_string()
        {
            WriteLine("public void AddRow({0} row)", importer.rowClassName);
            WriteLine("{");
            TabShift(1);
            WriteLine("if (!_stringMajorKey2Row.ContainsKey(row.{0}))", importer.mainKeys[0]);
            WriteLine("{");
            TabShift(1);
            WriteLine("_tableRows.Add(row);");
            WriteLine("_stringMajorKey2Row.Add(row.{0}, row);", importer.mainKeys[0]);
            if (importer.parentFileImporter != null)
            {
                var rootImporter = importer.parentFileImporter;
                while (rootImporter.parentFileImporter != null)
                    rootImporter = rootImporter.parentFileImporter;
                WriteLine("Config.Inst.{0}.AddRow(row);//子表才需要往总表添加", ConvertUtil.ToFirstCharLower(rootImporter.tableClassName));
            }
            TabShift(-1);
            WriteLine("}");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteRowCode()
        {
            if (importer.parentFileImporter != null)
                WriteLine("public partial class {0} : {1}", importer.rowClassName, importer.parentFileImporter.rowClassName);
            else
                WriteLine("public partial class {0} : XRow", importer.rowClassName);
            WriteLine("{");
            TabShift(1);

            List<ConfigFileImporter> parentImporters = new List<ConfigFileImporter>();
            ConfigFileImporter parentImporter = importer.parentFileImporter;
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
            bool isInheritClass = importer.parentFileImporter != null || importer.childFileImporters.Count > 0;
            for (int i = 0; i < importer.keys.Length; i++)
            {
                string key = importer.keys[i];
                if (string.IsNullOrEmpty(key)) continue;
                var flag = importer.flags[i];
                if (flag.IsNotExport) continue;
                string type = importer.types[i];
                string defaultValue = importer.defaults[i];
                IConfigType configType = importer.configTypes[i];
                if (isInheritClass && flag.IsMainKey)
                {
                    DebugUtil.Assert(key.Equals("Id"), "包含子父表继承关系时，主键变量名必须为【Id】,请修改配置表【{0}】主键", importer.fileName);
                }
                // 子表跟注释不生成主键字段
                if ((parentImporters.Count > 0 && flag.IsMainKey) || flag.IsNotExport)
                {
                    continue;
                }
                if (parentImporters.Count > 0)
                {
                    DebugUtil.Assert(!parentImporterKeyDic.ContainsKey(key), "子表的变量名不能父表相同，请检查并修改【{0}】表的【{1}】字段", importer.fileName, key);
                }
                if (configType.IsReference)//引用类型
                {
                    if (configType.IsList)//列表引用
                        WriteListReference(key, configType as ListType, flag, defaultValue);
                    else//单引用
                        WriteReference(key, configType, type, flag, defaultValue);
                }
                else
                {
                    if (configType.IsList)
                    {
                        WriteLine("private {0} _{1};", type, key);
                        string readOnlyType = type.Replace("List", "ReadOnlyCollection");
                        string cacheReadOnlyKey = $"_{ConvertUtil.ToFirstCharLower(key)}ReadOnlyCache";
                        WriteLine($"private {readOnlyType} {cacheReadOnlyKey};");
                        WriteLine($"public {readOnlyType} {key} {{ get {{ return {cacheReadOnlyKey} ?? ({cacheReadOnlyKey} = _{key}.AsReadOnly()); }} }}");
                    }
                    else
                    {
                        WriteLine("private {0} _{1};", type, key);
                        if (flag.IsMainKey) WriteLine($"[ConfigMainKey]");
                        WriteLine("public {0} {1} {{ get {{ return _{1}; }}}}", type, key);
                    }
                }
            }

            WriteRowFromBytesFunction();

            TabShift(-1);
            WriteLine("}");
        }
        void WriteRowFromBytesFunction()
        {
            WriteLine("public override void ReadFromBytes(BytesBuffer buffer)");
            WriteLine("{");
            TabShift(1);
            if (importer.parentFileImporter != null)
                WriteLine("base.ReadFromBytes(buffer);");
            for (int i = 0; i < importer.keys.Length; i++)
            {
                string key = importer.keys[i];
                if (string.IsNullOrEmpty(key)) continue;
                var configType = importer.configTypes[i];
                var flag = importer.flags[i];
                if (importer.parentFileImporter != null && flag.IsMainKey) continue;
                if (flag.IsNotExport) continue;
                string defaultValue = importer.defaults[i];

                if (flag.IsReference)//添加清除引用cache的代码，用于配置热加载
                    WriteLine($"_{ConvertUtil.ToFirstCharLower(key)} = null;");

                string finalKey = GetFinalKeyStr(key, configType, flag);
                if (configType.IsList)
                {
                    WriteLine($"_{ConvertUtil.ToFirstCharLower(key)}ReadOnlyCache = null;");
                    WriteListFromBytes(finalKey, configType as ListType, flag, defaultValue);
                }
                else
                    WriteBasicFromBytes(finalKey, configType, flag, defaultValue);
            }
            WriteLine("rowIndex = buffer.ReadInt32();");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteListFromBytes(string key, ListType listConfigType, Flag flag, string defaultValue)
        {
            var itemConfigType = listConfigType.itemConfigType; // 列表项类型
            string itemType = itemConfigType.TypeName;

            if (itemConfigType is ReferenceType referenceType) // 引用类型需要指向主键类型
                itemType = referenceType.mainKeyConfigType.TypeName;

            WriteLine($"_{key} = new List<{itemType}>();");
            WriteLine("if (buffer.ReadByte() == 1)");
            WriteLine("{");
            TabShift(1);
            WriteLine("byte itemCount = buffer.ReadByte();");
            // 用户定义的配置表字段类型
            if (itemConfigType.NeedExplicitCast)
                WriteLine($"for (int i = 0; i < itemCount; i++) {{ {itemConfigType.ConfigTypeName}.ReadFromBytes(buffer, out var value); _{key}.Add(({itemType})value); }}");
            else
                WriteLine($"for (int i = 0; i < itemCount; i++) {{ {itemConfigType.ConfigTypeName}.ReadFromBytes(buffer, out {itemType} value); _{key}.Add(value); }}");

            TabShift(-1);
            WriteLine("}");
        }
        void WriteBasicFromBytes(string key, IConfigType configType, Flag flag, string defaultValue)
        {
            string type = configType.TypeName;
            if (configType is ReferenceType referenceType) // 引用类型需要指向主键类型
                type = referenceType.mainKeyConfigType.TypeName;

            if (configType.NeedExplicitCast)
                WriteLine($"if (buffer.ReadByte() == 1) {{ {configType.ConfigTypeName}.ReadFromBytes(buffer, out var value); _{key} = ({type})value;}}");
            else
                WriteLine($"if (buffer.ReadByte() == 1) {configType.ConfigTypeName}.ReadFromBytes(buffer, out _{key});");

            WriteLine("else _{0} = {1};", key, defaultValue);
        }
        string GetFinalKeyStr(string key, IConfigType configType, Flag flag)
        {
            //需要二次处理字段，其key值要修改
            if (flag.IsReference)
            {
                if (configType.IsList)
                    return key + "Ids";
                else
                    return key + "Id";
            }
            return key;
        }
        void WriteReference(string key, IConfigType configType, string type, Flag flag, string defaultValue)
        {
            DebugUtil.Assert(context.fileName2ImporterDic.ContainsKey(type), "表{0} 列{1} 引用的表{2}并不存在", importer.fileName, key, type);
            ConfigFileImporter refImporter = context.fileName2ImporterDic[type];
            string lowerName = ConvertUtil.ToFirstCharLower(key);
            string referenceRowType = configType.TypeName;
            string mainKeyType = (configType as ReferenceType).mainKeyConfigType.TypeName;
            string idFieldName = key + "Id";
            string cacheFieldName = "_" + lowerName;
            WriteLine("[ConfigReference(\"{0}\")]", key);
            WriteLine($"private {mainKeyType} _{idFieldName};");
            WriteLine($"public {mainKeyType} {idFieldName} {{ get {{ return _{idFieldName}; }}}}");
            WriteLine("private {0} {1};", referenceRowType, cacheFieldName);
            WriteLine("public {0} {1}", referenceRowType, key);
            WriteLine("{");
            TabShift(1);
            WriteLine("get");
            WriteLine("{");
            TabShift(1);
            WriteLine($"if (_{idFieldName} == {defaultValue}) return null;");
            WriteLine($"return {cacheFieldName} ?? ({cacheFieldName} = Config.Inst.{GetReferenceTableLowerClassName(referenceRowType)}.GetValue({idFieldName}));");
            if (refImporter.mainKeyType == EnumTableMainKeyType.INT_INT)
                DebugUtil.Assert(false, "不支持引用的表是双主键表 {0}:{1}", refImporter.fileName, key);
            TabShift(-1);
            WriteLine("}");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteListReference(string key, ListType listConfigType, Flag flag, string defaultValue)
        {
            var itemConfigType = listConfigType.itemConfigType;
            string lowerName = ConvertUtil.ToFirstCharLower(key);
            string referenceRowType = listConfigType.TypeName;
            string readOnlyType = referenceRowType.Replace("List", "ReadOnlyCollection");
            string idsFieldName = key + "Ids";
            string cacheIdsReadOnlyKey = $"_{idsFieldName}ReadOnlyCache";
            string cachesFieldName = "_" + lowerName;
            string cachesFieldNameReadOnly = $"_{lowerName}ReadOnlyCache";
            string mainKeyType = (itemConfigType as ReferenceType).mainKeyConfigType.TypeName;
            WriteLine("[ConfigReference(\"{0}\")]", key);
            WriteLine($"private List<{mainKeyType}> _{idsFieldName};");
            WriteLine($"private ReadOnlyCollection<{mainKeyType}> {cacheIdsReadOnlyKey};");

            // ids
            WriteLine($"public ReadOnlyCollection<{mainKeyType}> {idsFieldName} {{ get {{ return {cacheIdsReadOnlyKey} ?? ({cacheIdsReadOnlyKey} = _{idsFieldName}.AsReadOnly()); }} }}");

            WriteLine("private {0} {1};", referenceRowType, cachesFieldName);
            WriteLine("private {0} {1};", readOnlyType, cachesFieldNameReadOnly);
            WriteLine($"public {readOnlyType} {key}");
            WriteLine("{");
            TabShift(1);

            WriteLine("get");
            WriteLine("{");
            TabShift(1);

            WriteLine("if ({0} == null)", cachesFieldName);
            WriteLine("{");
            WriteLine(1, $"{cachesFieldName} = new {referenceRowType}();");
            WriteLine(1, $"for (int i = 0; i < TypesIds.Count; i++) {cachesFieldName}.Add(Config.Inst.{GetReferenceTableLowerClassName(itemConfigType.TypeName)}.GetValue({idsFieldName}[i]));");
            WriteLine("}"); // end if

            WriteLine($"return {cachesFieldNameReadOnly} ?? ({cachesFieldNameReadOnly} = {cachesFieldName}.AsReadOnly());");
            TabShift(-1);
            WriteLine("}");

            TabShift(-1);
            WriteLine("}");

        }
        void WriteConfigCode()
        {
            WriteLine("public partial class Config");
            WriteLine("{");
            TabShift(1);
            WriteLine("[BindConfigPath(\"{0}\")]", importer.relativePath);
            WriteLine("public {0} {1} = new {0}();", importer.tableClassName, ConvertUtil.ToFirstCharLower(importer.tableClassName));
            TabShift(-1);
            WriteLine("}");
        }
        string GetReferenceTableLowerClassName(string type)
        {
            string humpNamed = ConvertUtil.ToFirstCharLower(type);
            return humpNamed.Replace("Row", "Table");
        }
    }
}