using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class CsvFileContext
{
    public Dictionary<string, TextFileImporter> fileName2ImporterDic = new Dictionary<string, TextFileImporter>();
    /// <summary>
    /// 包含所有Csv表的上下文
    /// </summary>
    /// <param name="files">所有csv表的路径</param>
    /// <param name="isReadContentRow">像生成客户端配置代码时，是不需要读取表的实际内容的，只需要知道表头</param>
    public CsvFileContext(string[] files, bool isReadContentRow = false)
    {
        foreach (var filePath in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            if (fileName != "csv_template")//csv_template是模板配置，不需要生成CsvFileImporter
            {
                CsvFileImporter importer = new CsvFileImporter(filePath, fileName, isReadContentRow);
                fileName2ImporterDic.Add(fileName, importer);

            }
        }
        //生成总表和子表关联，注意放到这里处理是因为需要等待所有CsvFileImporter建立完毕
        foreach (KeyValuePair<string, TextFileImporter> kvp in fileName2ImporterDic)
        {
            string fileName = kvp.Key;
            CsvFileImporter importer = kvp.Value as CsvFileImporter;
            if (importer.parentFileName != null)
            {
                DebugUtil.Assert(fileName2ImporterDic.ContainsKey(importer.parentFileName), "{0}", importer.parentFileName);
                CsvFileImporter parentImporter = fileName2ImporterDic[importer.parentFileName] as CsvFileImporter;
                parentImporter.childFileImporters.Add(kvp.Value as CsvFileImporter);
                importer.parentFileImporter = parentImporter;
            }
        }
        //导出
        foreach (KeyValuePair<string, TextFileImporter> kvp in fileName2ImporterDic)
        {
            CsvFileImporter importer = kvp.Value as CsvFileImporter;
            using (FileStream fs = new FileStream(importer.fileFullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                //现在项目统一使用wps处理表格，所以格式是gbk2312
                using (StreamReader fr = new StreamReader(fs, Encoding.GetEncoding("GB2312")))
                {
                    kvp.Value.Import(fr);
                }
            }
        }
        if (isReadContentRow)//导表才需要跑下面的检测
        {
            //检测总表行数要=所有子表行数之合
            foreach (KeyValuePair<string, TextFileImporter> kvp in fileName2ImporterDic)
            {
                string fileName = kvp.Key;
                CsvFileImporter importer = kvp.Value as CsvFileImporter;
                if (importer.childFileImporters.Count > 0)//有子表，表明是一个父表
                {
                    int totalCount = 0;
                    for (int i = 0; i < importer.childFileImporters.Count; i++)
                        totalCount += importer.childFileImporters[i].cellStrs.Count;
                    DebugUtil.Assert(totalCount == importer.cellStrs.Count, "父表={0}的行数跟其所有子表加起来的行数不相等 {1} != {2}", importer.fileName, importer.cellStrs.Count, totalCount);
                }
            }
        }
    }
}
