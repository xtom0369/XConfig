using UnityEngine;
using System.Collections;
using System.IO;

public abstract class BinFileExporter
{
    protected string outFilePath;
    protected TextFileImporter importer;
    protected BytesBuffer buffer;
    public BinFileExporter(string outFilePath, TextFileImporter importer, BytesBuffer buffer)
    {
        this.outFilePath = outFilePath;
        this.importer = importer;
        this.buffer = buffer;
    }
    virtual public void Export()
    {
    }
}
