using UnityEngine;
using System.Collections;

namespace XConfig.Editor
{
    public class Config2BinAdapter
    {
        string filePath;
        string outFilePath;
        string fileName;
        ConfigFileContext context;
        public Config2BinAdapter(string filePath, string outFilePath, string fileName, ConfigFileContext context)
        {
            this.filePath = filePath;
            this.outFilePath = outFilePath;
            this.fileName = fileName;
            this.context = context;
        }
        public void Convert(BytesBuffer buffer)
        {
            TextFileImporter importer = context.fileName2ImporterDic[fileName];
            Config2BinFileExporter exporter = new Config2BinFileExporter(outFilePath, importer, context, buffer);
            exporter.Export();
        }
    }
}