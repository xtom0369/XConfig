using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

namespace XConfig.Editor
{
    public class Config2BinFileExporter
    {
        protected string outFilePath;
        protected ConfigFileImporter importer;
        protected BytesBuffer buffer;
        int lineNumber;
        public Config2BinFileExporter(string outFilePath, ConfigFileImporter importer, BytesBuffer buffer)
        {
            this.outFilePath = outFilePath;
            this.importer = importer;
            this.buffer = buffer;
        }
        public void Export()
        {
            buffer.Clear();
            //表名
            string csvFileName = importer.fileName;
            buffer.WriteString(csvFileName);
            //行数
            int rowCount = importer.childFileImporters.Count == 0 ? importer.cellStrs.Count : 0;//有子表说明此表的行都不需要写，子表会关联到这些行数据
            DebugUtil.Assert(rowCount < ushort.MaxValue, $"表{importer.fileName} 行数上限突破了{ushort.MaxValue}:{rowCount}");
            buffer.WriteUInt16((ushort)rowCount);//行数上限到65536
                                                 //建立所有父表的数组，如果有
            List<ConfigFileImporter> parentImporters = new List<ConfigFileImporter>();
            ConfigFileImporter parentImporter = importer.parentFileImporter;
            while (parentImporter != null)
            {
                parentImporters.Insert(0, parentImporter);
                parentImporter = parentImporter.parentFileImporter;
            }
            //循环行
            for (int i = 0; i < rowCount; i++)
            {
                lineNumber = importer.lineNumbers[i];
                string[] values = importer.cellStrs[i];
                DebugUtil.Assert(values.Length == importer.keys.Length,
                    importer.fileName + " 下面这行很可能是少了或多了一个列 {0} != {1} \n {2}",
                    values.Length, importer.keys.Length, string.Join("ConfigFileImporter.SEPARATOR", values));
                //先将所有父表对应行的各列数据写进流里
                for (int j = 0; j < parentImporters.Count; j++)
                {
                    parentImporter = parentImporters[j];
                    DebugUtil.Assert(parentImporter.firstKey2RowCells.ContainsKey(values[0]), $"子表{importer.relativePath} id={values[0]} 在父表{values[0]}中找不到同id的行，请检测是否漏配行！");
                    string[] parentValues = parentImporter.firstKey2RowCells[values[0]];
                    WriteRow(parentImporter.keys, parentImporter.types, parentValues, parentImporter.flags, parentImporter.parentFileImporter == null);
                }
                //再把子表当前行的各列数据写进流里
                WriteRow(importer.keys, importer.types, values, importer.flags, false);
            }
            using (FileStream fs = new FileStream(outFilePath, FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter writer = new BinaryWriter(fs))
                {
                    writer.Write(buffer.buffer, 0, buffer.size);
                }
            }
        }
        void WriteRow(string[] keys, string[] types, string[] values, Flag[] flags, bool isParentRow)
        {
            for (int i = 0; i < types.Length; i++)
            {
                string key = keys[i];
                if (!string.IsNullOrEmpty(key))//有效列
                {
                    string value = values[i];
                    string type = types[i];
                    Flag flag = flags[i];
                    if (flag.IsMajorKey)
                        Assert(!string.IsNullOrEmpty(value), "主键那列不能为空");
                    //子类不用导出主键
                    if (!isParentRow && flag.IsMajorKey && importer.parentFileImporter != null || flag.IsNotExport)
                    {
                        continue;
                    }
                    if (string.IsNullOrEmpty(value))//未配置，填入字节0表示无字段
                        buffer.WriteByte(0);
                    else
                    {
                        buffer.WriteByte(1);//有配置，填入字节1表示有字段
                        if (type.StartsWith("List<"))//数组类型
                            WriteListType(type, value, flag);
                        else//基本类型
                            WriteBasicType(type, value, flag);
                    }
                }
            }
            buffer.WriteInt32(lineNumber);
        }
        void WriteListType(string type, string value, Flag flag)
        {
            int startIdx = type.IndexOf('<');
            int endIdx = type.IndexOf('>');
            string itemType = type.Substring(startIdx + 1, endIdx - startIdx - 1);//数组项的类型
            string[] items = value.Split('|');
            DebugUtil.Assert(items.Length < byte.MaxValue, "表{0} 数组上限突破了{1}:{2}",
                importer.fileName, byte.MaxValue, items.Length);
            buffer.WriteByte((byte)items.Length);//先写入数组长度
            foreach (var listItem in items)
            {
                Assert(!string.IsNullOrEmpty(listItem), "列表元素不能为空：{0}", value);
                WriteBasicType(itemType, listItem, flag);//写入每一项
            }
        }
        void WriteBasicType(string type, string value, Flag flag)
        {
            if (ConfigType.TryGetConfigType(type, out var configType))
            {
                if (!configType.CheckConfigFormat(value, out var error))
                    Assert(false, error);

                configType.WriteToBytes(buffer, value);
                return;
            }

            Assert(false, "不支持的数据类型：" + type);
        }
        protected void Assert(bool isValid, string msg, params object[] args)
        {
            string logStr = string.Format(msg, args);
            DebugUtil.Assert(isValid, $"表 = {importer.fileName} 行号 = {lineNumber} 异常，{logStr}");
        }
    }
}