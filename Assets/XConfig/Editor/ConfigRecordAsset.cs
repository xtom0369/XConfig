using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System;

namespace XConfig.Editor 
{
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

        /// <summary>
        /// 导出代码文件路径
        /// </summary>
        public string codeFilePath;

        /// <summary>
        /// 导出代码文件修改时间
        /// </summary>
        public long codeChangeTime;

        public SingleConfigRecord(string filePath, long lastChangeTime, string codeFilePath, long codeChangeTime)
        {
            this.sourceFilePath = filePath;
            this.sourceChangeTime = lastChangeTime;
            this.codeFilePath = codeFilePath;
            this.codeChangeTime = codeChangeTime;
        }
    }

    public class ConfigRecordInfo
    {
        public string sourceFilePath { get; }
        public string exportBinFilePath { get; }
        public string exportCodeFilePath { get; }

        FileInfo sourceFileInfo;
        FileInfo exportBinFileInfo;
        FileInfo exportCodeFileInfo;

        long sourceFileChangeTime;
        long codeFileChangeTime;

        public string sourceFileNameWithoutExtension
        {
            get { return Path.GetFileNameWithoutExtension(sourceFilePath); }
        }

        public ConfigRecordInfo(string sourceFilePath, string exportBinFilePath, string exportCodeFilePath)
        {
            this.sourceFilePath = sourceFilePath;
            this.exportBinFilePath = exportBinFilePath;
            this.exportCodeFilePath = exportCodeFilePath;

            sourceFileInfo = new FileInfo(sourceFilePath);
            exportBinFileInfo = new FileInfo(exportBinFilePath);
            exportCodeFileInfo = new FileInfo(exportCodeFilePath);

            sourceFileChangeTime = sourceFileInfo.LastWriteTime.ToFileTime();
            codeFileChangeTime = exportCodeFileInfo.LastWriteTime.ToFileTime();
        }

        /// <summary>
        /// 检查配置是否改变
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public bool IsChanged(SingleConfigRecord record)
        {
            if (!exportCodeFileInfo.Exists)
            {
                DebugUtil.Log($"{sourceFilePath} export file no exist: {exportBinFilePath}");
                return true;
            }

            if (sourceFileChangeTime != record.sourceChangeTime)
            {
                DebugUtil.Log($"{sourceFilePath} timestamp changed : {sourceFileChangeTime} != {record.sourceChangeTime}");
                return true;
            }
            else if (codeFileChangeTime != record.codeChangeTime)
            {
                DebugUtil.Log($"{exportCodeFilePath} timestamp changed : {codeFileChangeTime} != {record.codeChangeTime}");
                return true;
            }
            else
                return false;
        }

        public SingleConfigRecord ToRecord()
        {
            return new SingleConfigRecord(
                sourceFilePath,
                sourceFileChangeTime,
                exportCodeFilePath,
                codeFileChangeTime
                );
        }
    }
}
