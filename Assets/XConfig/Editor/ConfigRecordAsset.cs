using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System;

public class ConfigRecordAsset : ScriptableObject
{
    /// <summary>
    /// 记录列表，序列化并保存
    /// </summary>
    public List<SingleConfigRecord> recordList;

    /// <summary>
    /// 记录字典，文件路径 => 记录信息，用于快速搜索，加载后创建
    /// </summary>
    public Dictionary<string, SingleConfigRecord> filePath2Record = new Dictionary<string, SingleConfigRecord>();

    static public ConfigRecordAsset LoadRecord(string recordFilePath)
    {
        ConfigRecordAsset recordObject = AssetDatabase.LoadAssetAtPath<ConfigRecordAsset>(recordFilePath);
        if (recordObject == null)
        { 
            recordObject = ScriptableObject.CreateInstance<ConfigRecordAsset>();
            recordObject.recordList = new List<SingleConfigRecord>();
        }

        recordObject.filePath2Record.Clear();
        foreach (var record in recordObject.recordList) 
            recordObject.filePath2Record[record.sourceFilePath] = record;
        
        return recordObject;
    }

    static public void SaveRecord(string filePath, Dictionary<string, ConfigRecordInfo> recordDic)
    {
        ConfigRecordAsset record = ScriptableObject.CreateInstance<ConfigRecordAsset>();
        record.recordList = new List<SingleConfigRecord>();

        foreach (var kvp in recordDic)
            record.recordList.Add(kvp.Value.ToRecord());

        string parentDir = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(parentDir))
            Directory.CreateDirectory(parentDir);

        AssetDatabase.CreateAsset(record, filePath);
    }

    public List<ConfigRecordInfo> FiltChangedRecord(Dictionary<string, ConfigRecordInfo> recordInfoDic)
    {
        List<ConfigRecordInfo> changedFiles = new List<ConfigRecordInfo>();
        foreach (var kvp in recordInfoDic) 
        {
            if (IsChange(kvp.Value))
                changedFiles.Add(kvp.Value);
        }
        return changedFiles;
    }

    public bool IsChange(ConfigRecordInfo recordInfo)
    {
        if (filePath2Record.TryGetValue(recordInfo.sourceFilePath, out var record))
            return recordInfo.IsChanged(record);

        return true;
    }
}
[Serializable]
public class SingleConfigRecord
{
    /// <summary>
    /// 源文件路径
    /// </summary>
    public string sourceFilePath;

    /// <summary>
    /// 源文件修改时间
    /// </summary>
    public long sourceChangeTime;

    public SingleConfigRecord(string filePath, long lastChangeTime)
    {
        this.sourceFilePath = filePath;
        this.sourceChangeTime = lastChangeTime;
    }
}

public class ConfigRecordInfo
{
    public string sourceFilePath { get; }
    public string exportFilePath { get; }

    FileInfo sourceFileInfo;
    FileInfo exportFileInfo;
    long sourceFileChangeTime;

    public string sourceFileNameWithoutExtension
    {
        get { return Path.GetFileNameWithoutExtension(sourceFilePath); }
    }

    public ConfigRecordInfo(string sourceFilePath, string exportFilePath)
    {
        this.sourceFilePath = sourceFilePath;
        this.exportFilePath = exportFilePath;

        sourceFileInfo = new FileInfo(sourceFilePath);
        exportFileInfo = new FileInfo(exportFilePath);

        sourceFileChangeTime = sourceFileInfo.LastWriteTime.ToFileTime();
    }

    /// <summary>
    /// 检查配置是否改变
    /// </summary>
    /// <param name="record"></param>
    /// <returns></returns>
    public bool IsChanged(SingleConfigRecord record)
    {
        if (!exportFileInfo.Exists)
        {
            DebugUtil.Log($"{sourceFilePath} export file no exist: {exportFilePath}");
            return true;
        }

        if (sourceFileChangeTime != record.sourceChangeTime)
        {
            DebugUtil.Log($"{sourceFilePath} timestamp changed : {sourceFileChangeTime} != {record.sourceChangeTime}");
            return true;
        }
        else
            return false;
    }

    public SingleConfigRecord ToRecord()
    {
        return new SingleConfigRecord(
            sourceFilePath,
            sourceFileChangeTime
            );
    }
}