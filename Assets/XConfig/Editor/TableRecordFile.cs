using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System;

public class TableRecordFile : ScriptableObject
{
    //使用流程
    //1、LoadRecord加载对象最后修改记录
    //2、GetChangedNros传入当前生成的对象数组，返回有修改的对象数组
    //3、保存新的对象数组至记录
    static public TableRecordFile LoadRecord(string recordFilePath)
    {
        TableRecordFile record = AssetDatabase.LoadAssetAtPath(recordFilePath, typeof(TableRecordFile)) as TableRecordFile;
        if (record == null)
        {
            Debug.Log("record = null");
            record = ScriptableObject.CreateInstance<TableRecordFile>();
        }
        return record;
    }
    static public void SaveRecord<T>(string filePath, List<T> nros) where T : INeedRecordObject
    {
        TableRecordFile record = ScriptableObject.CreateInstance<TableRecordFile>();
        record.records = new List<TableRecord>();
        for (int i = 0; i < nros.Count; i++)
            record.records.Add(nros[i].ToRecord() as TableRecord);
        string dir = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        AssetDatabase.CreateAsset(record, filePath);
    }
    public List<TableRecord> records;
    public List<T> GetChangedNros<T>(List<T> nros) where T : INeedRecordObject
    {
        List<T> changedFiles = new List<T>();
        for (int i = 0; i < nros.Count; i++)
            if (IsNroChange(nros[i]))
                changedFiles.Add(nros[i]);
        return changedFiles;
    }
    public bool IsNroChange(INeedRecordObject nro)
    {
        //DebugUtil.Assert(File.Exists(info.csvFilePath), "存在配置类={0}，但不存在配置表={1}，不行", info.classFile, info.csvFilePath);
        if (records != null)
        {
            for (int i = 0; i < records.Count; i++)
            {
                if (records[i].IsMatchRecord(nro))
                    return (nro.isChange = nro.IsChanged(records[i]));
            }
        }
        Debug.Log(nro.ToString());
        nro.isChange = true;
        return true;
    }
}
public class NeedRecordTable : INeedRecordObject
{
    public string csvFilePath;
    public string binFilePath;
    public FileInfo csvFile;
    public FileInfo binFile;
    public FileInfo classFile;
    public string csvFileNameWithoutExtension
    {
        get { return Path.GetFileNameWithoutExtension(csvFile.Name); }
    }
    public bool isChange { get; set; }

    public NeedRecordTable(string csvFilePath, string classFilePath, string binFilePath)
    {
        this.csvFilePath = csvFilePath;
        this.binFilePath = binFilePath;
        csvFile = new FileInfo(csvFilePath);
        classFile = new FileInfo(classFilePath);
        binFile = new FileInfo(binFilePath);
    }
    public bool IsChanged(IRecord record)
    {
        if (!binFile.Exists)
        {
            Debug.Log(string.Format("csv binFile no exist: {0}", binFilePath));
            return true;
        }
        TableRecord csvRecord = record as TableRecord;
        long fileTime = csvFile.LastWriteTime.ToFileTime();
        long classFileTime = classFile.LastWriteTime.ToFileTime();
        if (fileTime != csvRecord.csvLastChangeTime)
        {
            Debug.Log(string.Format("changed {0}: {1} != {2}", csvFile.FullName, fileTime, csvRecord.csvLastChangeTime));
            return true;
        }
        else if (classFileTime != csvRecord.classLastChangeTime)
        {
            Debug.Log(string.Format("changed {0}: {1} != {2}", classFile.FullName, classFileTime, csvRecord.classLastChangeTime));
            return true;
        }
        else
            return false;
    }
    public IRecord ToRecord()
    {
        return new TableRecord(
            csvFile.FullName,
            csvFile.LastWriteTime.ToFileTime(),
            classFile.FullName,
            classFile.LastWriteTime.ToFileTime()
            );
    }
    override public string ToString()
    {
        return string.Format("changed {0}: no record", csvFile.FullName);
    }
}
[Serializable]
public class TableRecord : IRecord
{
    public string csvFilePath;
    public long csvLastChangeTime;//记录配置表的最后修改时间
    public string classFilePath;
    public long classLastChangeTime;//记录配置表生成的代码文件的最后修改时间

    public TableRecord(string filePath, long lastChangeTime, string classFilePath, long classFileLastChangeTime)
    {
        this.csvFilePath = filePath;
        this.csvLastChangeTime = lastChangeTime;
        this.classFilePath = classFilePath;
        this.classLastChangeTime = classFileLastChangeTime;
    }
    public bool IsMatchRecord(INeedRecordObject file)
    {
        return (file as NeedRecordTable).csvFile.FullName == csvFilePath ? true : false;
    }
}
