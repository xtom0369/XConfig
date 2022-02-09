using System.IO;

namespace XConfig.Editor 
{
    public class Config2CSharpAdapter : TextFileAdapter
    {
        string filePath;
        string outFilePath;
        string fileName;
        ConfigFileContext context;
        public Config2CSharpAdapter(string filePath, string outFilePath, string fileName, ConfigFileContext context)
        {
            this.filePath = filePath;
            this.outFilePath = outFilePath;
            this.fileName = fileName;
            this.context = context;
        }
        public override void Convert()
        {
            TextFileImporter importer = context.fileName2ImporterDic[fileName];
            ConfigCodeFileExporter exporter = new ConfigCodeFileExporter(outFilePath, importer, context);
            exporter.Export();
        }
    }
}