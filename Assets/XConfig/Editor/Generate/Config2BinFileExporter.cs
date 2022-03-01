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
            string fileName = importer.fileName;
            buffer.WriteString(fileName);
            //行数
            int rowCount = importer.isParent ? 0 : importer.rowDatas.Count;
            DebugUtil.Assert(rowCount < ushort.MaxValue, $"表{importer.fileName} 行数上限突破了{ushort.MaxValue}:{rowCount}");
            buffer.WriteUInt16((ushort)rowCount);//行数上限到65536

            //循环行
            for (int i = 0; i < rowCount; i++)
            {
                lineNumber = importer.rowIndexs[i];
                string[] values = importer.rowDatas[i];

                //先将所有父表对应行的各列数据写进流里
                if (importer.isChild)
                {
                    string combineKey = importer.GetCombineMainKeyValue(values);
                    var parentImporter = importer.parentImporter;
                    string[] parentValues = parentImporter.mainKey2RowData[combineKey];
                    WriteRow(parentImporter.configTypes, parentValues, parentImporter.flags, true);
                }

                //再把子表当前行的各列数据写进流里
                WriteRow(importer.configTypes, values, importer.flags, false);
            }
            using (FileStream fs = new FileStream(outFilePath, FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter writer = new BinaryWriter(fs))
                {
                    writer.Write(buffer.buffer, 0, buffer.size);
                }
            }
        }
        void WriteRow(IConfigType[] configTypes, string[] values, Flag[] flags, bool isParentValue)
        {
            for (int i = 0; i < configTypes.Length; i++)
            {
                string value = values[i];
                Flag flag = flags[i];
                IConfigType configType = configTypes[i];

                if (flag.IsNotExport) continue;
                if (!isParentValue && flag.IsMainKey && importer.isChild) continue; // 子表不用导出主键

                if (string.IsNullOrEmpty(value))
                { 
                    buffer.WriteByte(0);
                }
                else
                {
                    buffer.WriteByte(1);
                    WriteBasicType(configType, value);
                }
            }
        }
        void WriteBasicType(IConfigType configType, string value)
        {
            if (!configType.CheckConfigFormat(value, out var error))
                Assert(false, error);

            configType.WriteToBytes(buffer, value);
        }
        protected void Assert(bool isValid, string msg, params object[] args)
        {
            string logStr = string.Format(msg, args);
            DebugUtil.Assert(isValid, $"表 = {importer.fullFileName} 行号 = {lineNumber} 异常，{logStr}");
        }
    }
}