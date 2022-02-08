using UnityEngine;
using System.Collections;

public class Csv2BinAdapter
{
    string filePath;
    string outFilePath;
    string fileName;
    CsvFileContext context;
    public Csv2BinAdapter(string filePath, string outFilePath, string fileName, CsvFileContext context)
    {
        this.filePath = filePath;
        this.outFilePath = outFilePath;
        this.fileName = fileName;
        this.context = context;
    }
    public void Convert(BytesBuffer buffer)
    {
        TextFileImporter importer = context.fileName2ImporterDic[fileName];
        CsvBinFileExporter exporter = new CsvBinFileExporter(outFilePath, importer, context, buffer);
        exporter.Export();
    }
}
