using System.IO;

public class Csv2CSharpAdapter : TextFileAdapter
{
    string filePath;
    string outFilePath;
    string fileName;
    CsvFileContext context;
    public Csv2CSharpAdapter(string filePath, string outFilePath, string fileName, CsvFileContext context)
    {
        this.filePath = filePath;
        this.outFilePath = outFilePath;
        this.fileName = fileName;
        this.context = context;
    }
    public override void Convert()
    {
        TextFileImporter importer = context.fileName2ImporterDic[fileName];
        CsvClassFileExporter exporter = new CsvClassFileExporter(outFilePath, importer, context);
        exporter.Export();
    }
}
