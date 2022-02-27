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
        string isParent;

        public ConfigCodeFileExporter(string outFilePath, ConfigFileImporter importer, ConfigFileContext context) : base(outFilePath)
        {
            this.importer = importer;
            this.context = context;
            this.isParent = importer.isParent ? "true" : "false";
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
            WriteLine($"[BindConfigFileName(\"{importer.fileName}\", {isParent})]");
            WriteLine("public {0} {1} = new {0}();", importer.tableClassName, importer.lowerTableClassName);
            TabShift(-1);
            WriteLine("}");
        }
        void WriteTableCode()
        {
            WriteLine($"[BindConfigFileName(\"{importer.fileName}\", {isParent})]");
            string parentClassName = importer.mainKeyType == EnumTableMainKeyType.SINGLE ? $"{nameof(XTable)}<{importer.mainTypes[0]}, {importer.rowClassName}>" : $"{nameof(XTable)}<{importer.mainTypes[0]}, {importer.mainTypes[1]}, {importer.rowClassName}>";
            WriteLine($"public partial class {importer.tableClassName} : {parentClassName}");
            WriteLine("{");
            TabShift(1);

            switch (importer.mainKeyType)
            {
                case EnumTableMainKeyType.SINGLE:
                    if(importer.isParent)
                        WriteAddRowFunction_single();
                    break;

                case EnumTableMainKeyType.DOUBLE:
                    if(importer.isParent)
                        WriteAddRowFunction_double();
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
            if (importer.isChild) // 是子表
            {
                EmptyLine();
                WriteLine("for (int i = 0; i < _rows.Count; i++)");
                WriteLine(1, $"Config.Inst.{importer.parentImporter.lowerTableClassName}.AddRow(_rows[i]);");
            }
            EmptyLine();
            WriteLine("OnAfterInit();");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteAddRowFunction_single()
        {
            WriteLine("public void AddRow({0} row)", importer.rowClassName);
            WriteLine("{");
            TabShift(1);
            WriteLine("if (!mainKey2Row.ContainsKey(row.{0}))", importer.mainKeys[0]);
            WriteLine("{");
            TabShift(1);
            WriteLine("_rows.Add(row);");
            EmptyLine();
            WriteLine("mainKey2Row.Add(row.{0}, row);", importer.mainKeys[0]);
            if (importer.isChild)
                WriteLine("Config.Inst.{0}.AddRow(row);", importer.parentImporter.lowerTableClassName);
            TabShift(-1);
            WriteLine("}");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteAddRowFunction_double()
        {
            WriteLine("public void AddRow({0} row)", importer.rowClassName);
            WriteLine("{");
            TabShift(1);
            WriteLine($"if (!_mainKey2Row.TryGetValue(row.{importer.mainKeys[0]}, out var secondKey2Row))");
            WriteLine("{");
            TabShift(1);
            WriteLine($"secondKey2Row = new Dictionary<{importer.mainTypes[1]}, {importer.rowClassName}>();");
            WriteLine($"_mainKey2Row.Add(row.{importer.mainKeys[0]}, secondKey2Row);");
            TabShift(-1);
            WriteLine("}");
            EmptyLine();

            WriteLine($"if (!secondKey2Row.ContainsKey(row.{importer.mainKeys[1]}))");
            WriteLine("{");
            TabShift(1);
            WriteLine($"_rows.Add(row);");
            WriteLine($"secondKey2Row.Add(row.{importer.mainKeys[1]}, row);");
            if (importer.isChild)
                WriteLine("Config.Inst.{0}.AddRow(row); // 子表才需要往总表添加", importer.parentImporter.lowerTableClassName);
            TabShift(-1);
            WriteLine("}");

            TabShift(-1);
            WriteLine("}");
        }
        void WriteRowCode()
        {
            WriteLine($"[BindConfigFileName(\"{importer.fileName}\", {isParent})]");
            string parentClassName = importer.mainKeyType == EnumTableMainKeyType.SINGLE ? $"XRow<{importer.mainTypes[0]}>" : $"XRow<{importer.mainTypes[0]}, {importer.mainTypes[1]}>";
            parentClassName = importer.parentImporter != null ? importer.parentImporter.rowClassName : parentClassName;
            WriteLine($"public partial class {importer.rowClassName} : {parentClassName}");
            WriteLine("{");
            TabShift(1);

            Dictionary<string, bool> parentImporterKeyDic = new Dictionary<string, bool>();
            if (importer.isChild)
            {
                ConfigFileImporter parentImporter = importer.parentImporter;
                foreach (string key in parentImporter.keys)
                {
                    if (!parentImporterKeyDic.ContainsKey(key))
                        parentImporterKeyDic.Add(key, true);
                }
            }

            //是否是继承关系的表
            bool isInheritClass = importer.isParent || importer.isChild;
            if (!importer.isChild) 
            {
                for (int j = 0; j < importer.mainKeys.Count; j++)
                    WriteLine($"public override {importer.mainTypes[j]} mainKey{j + 1} => {importer.mainKeys[j]};");
            }

            for (int i = 0; i < importer.keys.Length; i++)
            {
                string key = importer.keys[i];
                string lowerKey = importer.lowerKeys[i];
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
                if ((importer.isChild && flag.IsMainKey) || flag.IsNotExport)
                {
                    continue;
                }

                if (importer.isChild)
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
                        string readOnlyType = type.Replace("List", "ReadOnlyCollection");
                        string cacheReadOnlyKey = $"_{lowerKey}ReadOnlyCache";
                        WriteLine($"public {readOnlyType} {key} {{ get {{ return {cacheReadOnlyKey} ?? ({cacheReadOnlyKey} = _{lowerKey}.AsReadOnly()); }} }}");
                        WriteLine($"{type} _{lowerKey};");
                        WriteLine($"{readOnlyType} {cacheReadOnlyKey};");
                    }
                    else
                    {
                        WriteLine($"public {type} {key} => _{lowerKey}; {type} _{lowerKey};");
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
            if (importer.parentImporter != null)
                WriteLine("base.ReadFromBytes(buffer);");
            for (int i = 0; i < importer.keys.Length; i++)
            {
                string key = importer.keys[i];
                string lowerKey = importer.lowerKeys[i];
                var configType = importer.configTypes[i];
                var flag = importer.flags[i];
                string defaultValue = importer.defaults[i];

                if (string.IsNullOrEmpty(key)) continue;
                if (importer.parentImporter != null && flag.IsMainKey) continue;
                if (flag.IsNotExport) continue;

                if (configType.IsReference)//添加清除引用cache的代码，用于配置热加载
                    WriteLine($"_{lowerKey} = null;");

                string finalKey = GetFinalKeyStr(lowerKey, configType, flag);
                if (configType.IsList)
                {
                    WriteLine($"_{lowerKey}ReadOnlyCache = null;");
                    WriteListFromBytes(finalKey, configType as ListType, flag, defaultValue);
                }
                else
                    WriteBasicFromBytes(finalKey, configType, flag, defaultValue);
            }
            WriteLine("rowIndex = buffer.ReadInt32();");
            TabShift(-1);
            WriteLine("}");
        }
        void WriteListFromBytes(string lowerKey, ListType listConfigType, Flag flag, string defaultValue)
        {
            var itemConfigType = listConfigType.ItemConfigType; // 列表项类型
            string itemType = itemConfigType.ConfigTypeName;

            if (itemConfigType is ReferenceType referenceType) // 引用类型需要指向主键类型
                itemType = referenceType.mainKeyConfigType.ConfigTypeName;

            WriteLine($"_{lowerKey} = new List<{itemType}>();");
            WriteLine("if (buffer.ReadByte() == 1)");
            WriteLine("{");
            TabShift(1);
            WriteLine("byte itemCount = buffer.ReadByte();");
            // 用户定义的配置表字段类型
            WriteLine($"for (int i = 0; i < itemCount; i++) {{ {itemConfigType.ReadByteClassName}.ReadFromBytes(buffer, out {itemConfigType.WriteByteTypeName} value); _{lowerKey}.Add(({itemType})value); }}");

            TabShift(-1);
            WriteLine("}");
        }
        void WriteBasicFromBytes(string lowerKey, IConfigType configType, Flag flag, string defaultValue)
        {
            string type = configType.ConfigTypeName;

            if (configType is ReferenceType referenceType) // 引用类型需要指向主键类型
                type = referenceType.mainKeyConfigType.ConfigTypeName;

            WriteLine($"if (buffer.ReadByte() == 1) {{ {configType.ReadByteClassName}.ReadFromBytes(buffer, out {configType.WriteByteTypeName} value); _{lowerKey} = ({type})value;}}");
            WriteLine($"else _{lowerKey} = {defaultValue};");
        }
        string GetFinalKeyStr(string key, IConfigType configType, Flag flag)
        {
            //需要二次处理字段，其key值要修改
            if (configType.IsReference)
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
            ConfigFileImporter refImporter = context.fileName2Importer[type];
            string lowerName = StringUtil.ToFirstCharLower(key);
            string referenceRowType = configType.TypeName;
            string mainKeyType = (configType as ReferenceType).mainKeyConfigType.ConfigTypeName;
            string idFieldName = key + "Id";
            string lowerIdFieldName = StringUtil.ToFirstCharLower(idFieldName);
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
            WriteLine($"return {cacheFieldName} ?? ({cacheFieldName} = Config.Inst.{refImporter.lowerTableClassName}.GetRow({idFieldName}));");
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
            ConfigFileImporter refImporter = context.fileName2Importer[listConfigType.ReferenceFileName];
            var itemConfigType = listConfigType.ItemConfigType;
            string lowerName = StringUtil.ToFirstCharLower(key);
            string referenceRowType = listConfigType.TypeName;
            string readOnlyType = referenceRowType.Replace("List", "ReadOnlyCollection");
            string idsFieldName = key + "Ids";
            string lowerIdsFieldName = StringUtil.ToFirstCharLower(idsFieldName);
            string cacheIdsReadOnlyKey = $"_{lowerIdsFieldName}ReadOnlyCache";
            string cachesFieldName = "_" + lowerName;
            string cachesFieldNameReadOnly = $"_{lowerName}ReadOnlyCache";
            string mainKeyType = (itemConfigType as ReferenceType).mainKeyConfigType.ConfigTypeName;
            WriteLine("[ConfigReference(\"{0}\")]", key);
            WriteLine($"List<{mainKeyType}> _{lowerIdsFieldName};");
            WriteLine($"ReadOnlyCollection<{mainKeyType}> {cacheIdsReadOnlyKey};");
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
            WriteLine(1, $"for (int i = 0; i < {idsFieldName}.Count; i++) {cachesFieldName}.Add(Config.Inst.{refImporter.lowerTableClassName}.GetRow({idsFieldName}[i]));");
            WriteLine("}"); // end if
            WriteLine($"return {cachesFieldNameReadOnly} ?? ({cachesFieldNameReadOnly} = {cachesFieldName}.AsReadOnly());");
            TabShift(-1);
            WriteLine("}");

            TabShift(-1);
            WriteLine("}");

        }
    }
}