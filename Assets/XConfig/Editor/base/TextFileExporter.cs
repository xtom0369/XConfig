﻿using System.IO;

/// <summary>
/// 导出成文本格式的导出器
/// </summary>
public abstract class TextFileExporter
{
    protected string outFilePath;
    protected TextFileImporter importer;
    StreamWriter writer;
    int tabLevel = 0;
    public TextFileExporter(string outFilePath, TextFileImporter importer)
    {
        this.outFilePath = outFilePath;
        this.importer = importer;
    }
    System.IO.StreamWriter NewStreamWriter(System.IO.Stream stream)
    {
        System.IO.StreamWriter sw = new System.IO.StreamWriter(stream);
        sw.NewLine = "\r\n";
        return sw;
    }
    public void Export()
    {
        using (FileStream fs = new FileStream(outFilePath, FileMode.Create, FileAccess.Write))
        {
            using (writer = NewStreamWriter(fs))
                DoExport();
        }
        writer = null;
    }
    protected abstract void DoExport();

    protected void TabShift(int level)
    {
        tabLevel += level;

        if (tabLevel < 0)
            tabLevel = 0;
    }

    private void WriteTab(int level)
    {
        for (int i = 0; i < (level + tabLevel); i++)
            writer.Write("	");
    }

    protected void EmptyLine() { writer.WriteLine(""); }

    protected void Write(string value) { writer.Write(value); }

    protected void Write(int tabLevel, string value) { WriteTab(tabLevel); writer.Write(value); }
    protected void Write(int tabLevel, string format, object arg0) { WriteTab(tabLevel); writer.Write(format, arg0); }
    protected void Write(int tabLevel, string format, object arg0, object arg1) { WriteTab(tabLevel); writer.Write(format, arg0, arg1); }
    protected void Write(int tabLevel, string format, object arg0, object arg1, object arg2) { WriteTab(tabLevel); writer.Write(format, arg0, arg1, arg2); }

    protected void Write(string format, object arg0) { writer.Write(format, arg0); }
    protected void Write(string format, object arg0, object arg1) { writer.Write(format, arg0, arg1); }
    protected void Write(string format, object arg0, object arg1, object arg2) { writer.Write(format, arg0, arg1, arg2); }

    protected void WriteLine(string value) { WriteTab(0); writer.WriteLine(value); }
    protected void WriteLine(int tabLevel, string value) { WriteTab(tabLevel); writer.WriteLine(value); }
    protected void WriteLine(int tabLevel, string format, object arg0) { WriteTab(tabLevel); writer.WriteLine(format, arg0); }
    protected void WriteLine(int tabLevel, string format, object arg0, object arg1) { WriteTab(tabLevel); writer.WriteLine(format, arg0, arg1); }
    protected void WriteLine(int tabLevel, string format, object arg0, object arg1, object arg2) { WriteTab(tabLevel); writer.WriteLine(format, arg0, arg1, arg2); }

    protected void WriteLine(string format, object arg0) { WriteTab(0); writer.WriteLine(format, arg0); }
    protected void WriteLine(string format, object arg0, object arg1) { WriteTab(0); writer.WriteLine(format, arg0, arg1); }
    protected void WriteLine(string format, object arg0, object arg1, object arg2) { WriteTab(0); writer.WriteLine(format, arg0, arg1, arg2); }
    protected void WriteLine(string format, object arg0, object arg1, object arg2, object arg3) { WriteTab(0); writer.WriteLine(format, arg0, arg1, arg2, arg3); }
    protected void WriteLine(string format, object arg0, object arg1, object arg2, object arg3, object arg4) { WriteTab(0); writer.WriteLine(format, arg0, arg1, arg2, arg3, arg4); }

    protected void Close()
    {
        writer.Flush();
        writer = null;
    }
}
