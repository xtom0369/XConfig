using System.IO;

namespace XConfig.Editor
{ 
    /// <summary>
    /// 导出成文本格式的导出器
    /// </summary>
    public abstract class TextFileExporter
    {
        protected string outFilePath;
        StreamWriter writer;
        int tabLevel = 0;

        public TextFileExporter(string outFilePath)
        {
            this.outFilePath = outFilePath;
        }

        StreamWriter NewStreamWriter(Stream stream)
        {
            StreamWriter sw = new StreamWriter(stream);
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

        protected void EmptyLine() { writer.WriteLine(string.Empty); }
        protected void WriteLine(string value) { WriteTab(0); writer.WriteLine(value); }
        protected void WriteLine(int tabLevel, string value) { WriteTab(tabLevel); writer.WriteLine(value); }
        protected void WriteLine(string format, object arg0) { WriteTab(0); writer.WriteLine(format, arg0); }
        protected void WriteLine(string format, object arg0, object arg1) { WriteTab(0); writer.WriteLine(format, arg0, arg1); }

        protected void Close()
        {
            writer.Flush();
            writer = null;
        }
    }
}
