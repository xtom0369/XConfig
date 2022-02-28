using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace XConfig.Editor 
{
    public class ConfigFileContext
    {
        public Dictionary<string, ConfigFileImporter> fileName2Importer = new Dictionary<string, ConfigFileImporter>();

        /// <summary>
        /// 包含所有表的上下文
        /// </summary>
        /// <param name="files">所有表的路径</param>
        /// <param name="isReadRow">生成代码时只需要知道表头，只需要知道表头</param>
        public ConfigFileContext(string[] files, bool isReadRow = false)
        {
            foreach (var filePath in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                if (!Settings.Inst.IsFileExclude(fileName))
                {
                    ConfigFileImporter importer = new ConfigFileImporter(filePath, fileName, this, isReadRow);
                    fileName2Importer.Add(fileName, importer);
                }
            }

            //生成总表和子表关联
            foreach (var kvp in fileName2Importer)
            {
                ConfigFileImporter importer = kvp.Value;
                if (importer.isChild)
                {
                    DebugUtil.Assert(fileName2Importer.ContainsKey(importer.parentFileName), "{0}", importer.parentFileName);
                    ConfigFileImporter parentImporter = fileName2Importer[importer.parentFileName];
                    parentImporter.childImporters.Add(kvp.Value);
                    importer.parentImporter = parentImporter;
                }
            }

            //导入表数据
            foreach (var kvp in fileName2Importer)
            {
                ConfigFileImporter importer = kvp.Value;
                using (FileStream fs = new FileStream(importer.fileFullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    //wps处理表格，格式是gbk2312
                    using (StreamReader fr = new StreamReader(fs, Encoding.GetEncoding("GB2312")))
                        kvp.Value.Import(fr);
                }
            }

            if (isReadRow) // 导表需要跑下面的检测
            {
                foreach (var kvp in fileName2Importer)
                {
                    ConfigFileImporter importer = kvp.Value;
                    importer.OnAfterImport();
                }
            }
        }
    }

}
