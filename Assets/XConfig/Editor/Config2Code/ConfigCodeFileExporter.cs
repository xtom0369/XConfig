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
        void WriteConfigCode()
        {
            WriteLine("public partial class Config");
            WriteLine("{");
            TabShift(1);
            WriteLine($"[BindConfigFileName(\"{importer.fileName}\")]");
            WriteLine("public {0} {1} = new {0}();", importer.tableClassName, ConvertUtil.ToFirstCharLower(importer.tableClassName));
            TabShift(-1);
            WriteLine("}");
        }
        void WriteTableCode()
        {
            WriteLine($"[BindConfigFileName(\"{importer.fileName}\")]");
            if(importer.mainKeyType ==  EnumTableMainKeyType.SINGLE)
                WriteLine($"public partial class {importer.tableClassName} : XTable<{importer.mainTypes[0]}, {importer.rowClassName}>");
            else
                WriteLine($"public partial class {importer.tableClassName} : XTable<{importer.rowClassName}>");
            WriteLine("{");
            TabShift(1);

            switch (importer.mainKeyType)
            {
                case EnumTableMainKeyType.SINGLE:
                    string mainKeyTypeName = importer.mainTypes[0];
                    WriteInitFunction(mainKeyTypeName);
                    WriteAddRowFunction(mainKeyTypeName);
                    break;

                case EnumTableMainKeyType.DOUBLE:
                    WriteInitFunction_int_int();
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
        void WriteAllTableInitCompleteFunction()
        {
            WriteLine("public override void OnInit()");
            WriteLine("{");
            TabShift(1);
            WriteLine("for (int i = 0; i < _rows.Count; i++)");
            WriteLine(1, "_rows[i].OnAfterInit();");
            if (importer.parentFileImporter != null)//是子表
            {
                ConfigFileImporter rootImporter = importer.parentFileImporter;
                while (rootImporter.parentFileImporter != null)
                    rootImporter = rootImporter.parentFileImporter;
                WriteLine("for (int i = 0; i < _rows.Count; i++)//子表才需要往总表添加");
                WriteLine(1, $"Config.Inst.{ConvertUtil.ToFirstCharLower(rootImporter.tableClassName)}.AddRow(_rows[i]);");
            }
            EmptyLine();
            WriteLine("OnAfterInit();");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteInitFunction(string mainKeyType)
        {
            WriteLine("public override void Init()");
            WriteLine("{");
            TabShift(1);
            WriteLine($"_mainKey2Row = new Dictionary<{mainKeyType}, {importer.rowClassName}>();");
            WriteLine("for (int i = 0; i < _rows.Count; i++)");
            WriteLine("{");
            TabShift(1);
            WriteLine($"{importer.rowClassName} row = _rows[i];");
            WriteLine($"{mainKeyType} mainKey = row.{importer.mainKeys[0]};");
            WriteLine("DebugUtil.Assert(!_mainKey2Row.ContainsKey(mainKey), \"{0} 主键重复：{1}，请先按键盘【alt+r】导出配置试试！\", name, mainKey);");
            WriteLine("_mainKey2Row.Add(mainKey, row);");
            TabShift(-1);
            WriteLine("}");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteAddRowFunction(string mainKeyType)
        {
            WriteLine("public void AddRow({0} row)", importer.rowClassName);
            WriteLine("{");
            TabShift(1);
            WriteLine("if (!_mainKey2Row.ContainsKey(row.{0}))", importer.mainKeys[0]);
            WriteLine("{");
            TabShift(1);
            WriteLine("_rows.Add(row);");
            WriteLine("_mainKey2Row.Add(row.{0}, row);", importer.mainKeys[0]);
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
        void WriteInitFunction_int_int()
        {
            WriteLine("public override void Init()");
            WriteLine("{");
            TabShift(1);
            WriteLine("_mainKey2Row = new Dictionary<long, {0}>();", importer.rowClassName);
            WriteLine("for (int i = 0; i < _rows.Count; i++)");
            WriteLine("{");
            TabShift(1);
            WriteLine("var row = _rows[i];");
            WriteLine($"long mainKey = GetMainKey(row.{importer.mainKeys[0]}, row.{importer.mainKeys[1]});");
            WriteLine($"DebugUtil.Assert(!_mainKey2Row.ContainsKey(mainKey), $\"{{name}} 主键重复：{{row.{importer.mainKeys[0]}}} {{row.{importer.mainKeys[1]}}}\");");
            WriteLine("_mainKey2Row.Add(mainKey, row);");
            TabShift(-1);
            WriteLine("}");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteAddRowFunction_int_int()
        {
            WriteLine("public void AddRow({0} row)", importer.rowClassName);
            WriteLine("{");
            TabShift(1);
            WriteLine($"long mainKey = GetMainKey(row.{importer.mainKeys[0]}, row.{importer.mainKeys[1]});");
            WriteLine("if (!_mainKey2Row.ContainsKey(mainKey))");
            WriteLine("{");
            TabShift(1);
            WriteLine("_rows.Add(row);");
            WriteLine("_mainKey2Row.Add(mainKey, row);");
            if (importer.parentFileImporter != null)
            {
                var rootImporter = importer.parentFileImporter;
                while (rootImporter.parentFileImporter != null)
                    rootImporter = rootImporter.parentFileImporter;
                WriteLine("Config.Inst.{0}.AddRow(row); // 子表才需要往总表添加", ConvertUtil.ToFirstCharLower(rootImporter.tableClassName));
            }
            TabShift(-1);
            WriteLine("}");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteRowCode()
        {
            WriteLine($"[BindConfigFileName(\"{importer.fileName}\")]");
            string parentClassName = importer.parentFileImporter != null ? importer.parentFileImporter.rowClassName : nameof(XRow);
            WriteLine($"public partial class {importer.rowClassName} : {parentClassName}");
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
                        string lowerName = ConvertUtil.ToFirstCharLower(key);
                        string readOnlyType = type.Replace("List", "ReadOnlyCollection");
                        string cacheReadOnlyKey = $"_{lowerName}ReadOnlyCache";
                        WriteLine($"public {readOnlyType} {key} {{ get {{ return {cacheReadOnlyKey} ?? ({cacheReadOnlyKey} = _{lowerName}.AsReadOnly()); }} }}");
                        WriteLine($"{type} _{lowerName};");
                        WriteLine($"{readOnlyType} {cacheReadOnlyKey};");
                    }
                    else
                    {
                        string lowerName = ConvertUtil.ToFirstCharLower(key);
                        if (flag.IsMainKey) WriteLine($"[ConfigMainKey]");
                        WriteLine($"public {type} {key} {{ get {{ return _{lowerName}; }}}}");
                        WriteLine($"{type} _{lowerName};");
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
                var configType = importer.configTypes[i];
                var flag = importer.flags[i];
                string defaultValue = importer.defaults[i];

                if (string.IsNullOrEmpty(key)) continue;
                if (importer.parentFileImporter != null && flag.IsMainKey) continue;
                if (flag.IsNotExport) continue;

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
            string lowerkey = ConvertUtil.ToFirstCharLower(key);
            var itemConfigType = listConfigType.ItemConfigType; // 列表项类型
            string itemType = itemConfigType.ConfigTypeName;

            if (itemConfigType is ReferenceType referenceType) // 引用类型需要指向主键类型
                itemType = referenceType.mainKeyConfigType.ConfigTypeName;

            WriteLine($"_{lowerkey} = new List<{itemType}>();");
            WriteLine("if (buffer.ReadByte() == 1)");
            WriteLine("{");
            TabShift(1);
            WriteLine("byte itemCount = buffer.ReadByte();");
            // 用户定义的配置表字段类型
            WriteLine($"for (int i = 0; i < itemCount; i++) {{ {itemConfigType.ReadByteClassName}.ReadFromBytes(buffer, out {itemConfigType.WriteByteTypeName} value); _{lowerkey}.Add(({itemType})value); }}");

            TabShift(-1);
            WriteLine("}");
        }
        void WriteBasicFromBytes(string key, IConfigType configType, Flag flag, string defaultValue)
        {
            key = ConvertUtil.ToFirstCharLower(key);
            string type = configType.ConfigTypeName;
            if (configType is ReferenceType referenceType) // 引用类型需要指向主键类型
                type = referenceType.mainKeyConfigType.ConfigTypeName;

            WriteLine($"if (buffer.ReadByte() == 1) {{ {configType.ReadByteClassName}.ReadFromBytes(buffer, out {configType.WriteByteTypeName} value); _{key} = ({type})value;}}");
            WriteLine($"else _{key} = {defaultValue};");
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
            DebugUtil.Assert(context.fileName2ImporterDic.ContainsKey(type), $"表{importer.fileName} 列{key} 引用的表{type}并不存在");
            ConfigFileImporter refImporter = context.fileName2ImporterDic[type];
            string lowerName = ConvertUtil.ToFirstCharLower(key);
            string referenceRowType = configType.TypeName;
            string mainKeyType = (configType as ReferenceType).mainKeyConfigType.ConfigTypeName;
            string idFieldName = key + "Id";
            string lowerIdFieldName = ConvertUtil.ToFirstCharLower(idFieldName);
            string cacheFieldName = "_" + lowerName;

            WriteLine($"public {mainKeyType} {idFieldName} {{ get {{ return _{lowerIdFieldName}; }}}}");
            WriteLine("[ConfigReference(\"{0}\")]", key);
            WriteLine($"{mainKeyType} _{lowerIdFieldName};");
            WriteLine("public {0} {1}", referenceRowType, key);
            WriteLine("{");
            TabShift(1);
            WriteLine("get");
            WriteLine("{");
            TabShift(1);
            WriteLine($"if (_{lowerIdFieldName} == {defaultValue}) return null;");
            WriteLine($"return {cacheFieldName} ?? ({cacheFieldName} = Config.Inst.{GetReferenceTableLowerClassName(referenceRowType)}.GetRow({idFieldName}));");
            if (refImporter.mainKeyType == EnumTableMainKeyType.DOUBLE)
                DebugUtil.Assert(false, "不支持引用的表是双主键表 {0}:{1}", refImporter.fileName, key);
            TabShift(-1);
            WriteLine("}");
            TabShift(-1);
            WriteLine("}");
            WriteLine("{0} {1};", referenceRowType, cacheFieldName);
        }
        void WriteListReference(string key, ListType listConfigType, Flag flag, string defaultValue)
        {
            var itemConfigType = listConfigType.ItemConfigType;
            string lowerName = ConvertUtil.ToFirstCharLower(key);
            string referenceRowType = listConfigType.TypeName;
            string readOnlyType = referenceRowType.Replace("List", "ReadOnlyCollection");
            string idsFieldName = key + "Ids";
            string lowerIdsFieldName = ConvertUtil.ToFirstCharLower(idsFieldName);
            string cacheIdsReadOnlyKey = $"_{lowerIdsFieldName}ReadOnlyCache";
            string cachesFieldName = "_" + lowerName;
            string cachesFieldNameReadOnly = $"_{lowerName}ReadOnlyCache";
            string mainKeyType = (itemConfigType as ReferenceType).mainKeyConfigType.ConfigTypeName;
            WriteLine("[ConfigReference(\"{0}\")]", key);
            WriteLine($"List<{mainKeyType}> _{lowerIdsFieldName};");
            WriteLine($"ReadOnlyCollection<{mainKeyType}> {cacheIdsReadOnlyKey};");

            // ids
            WriteLine($"public ReadOnlyCollection<{mainKeyType}> {idsFieldName} {{ get {{ return {cacheIdsReadOnlyKey} ?? ({cacheIdsReadOnlyKey} = _{lowerIdsFieldName}.AsReadOnly()); }} }}");

            WriteLine("{0} {1};", referenceRowType, cachesFieldName);
            WriteLine("{0} {1};", readOnlyType, cachesFieldNameReadOnly);
            WriteLine($"public {readOnlyType} {key}");
            WriteLine("{");
            TabShift(1);

            WriteLine("get");
            WriteLine("{");
            TabShift(1);

            WriteLine("if ({0} == null)", cachesFieldName);
            WriteLine("{");
            WriteLine(1, $"{cachesFieldName} = new {referenceRowType}();");
            WriteLine(1, $"for (int i = 0; i < {idsFieldName}.Count; i++) {cachesFieldName}.Add(Config.Inst.{GetReferenceTableLowerClassName(itemConfigType.TypeName)}.GetRow({idsFieldName}[i]));");
            WriteLine("}"); // end if

            WriteLine($"return {cachesFieldNameReadOnly} ?? ({cachesFieldNameReadOnly} = {cachesFieldName}.AsReadOnly());");
            TabShift(-1);
            WriteLine("}");

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