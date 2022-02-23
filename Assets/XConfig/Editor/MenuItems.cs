using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace XConfig.Editor
{
    public static class MenuItems
    {
        //配置表格式版本号，当配置表格式变化时，可修改此版本号，触发全量导出，避免资源跟代码格式不匹配的情况
        public const int CONFIG_FORMAT_VERSION = 1;
        public static readonly string TABLE_LAST_CHANGE_RECORD_PATH = Path.Combine(Application.persistentDataPath, $"ConfigRecord/Record{CONFIG_FORMAT_VERSION}.txt");

        [MenuItem("XConfig/Generate Code &c", false, 1)]
        public static void GenerateCode()
        {
            if (EditorApplication.isCompiling)
            {
                EditorUtility.DisplayDialog("Generate Code", "Wait for compilation to complete", "OK");
                return;
            }

            ClearConsole();

            string[] files = FileUtil.GetFiles(Settings.Inst.ConfigPath, Settings.Inst.FilePatterns, SearchOption.AllDirectories);
            List<string> fileClassNames = new List<string>(files.Length);
            ConfigFileContext context = new ConfigFileContext(files);
            foreach (var file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                if (!Settings.Inst.IsFileExclude(fileName))
                {
                    string className = ConvertUtil.UnderscoreToCamel(fileName) + "Table.cs";
                    fileClassNames.Add(className);

                    string outputFilePath = Path.Combine(Settings.Inst.GenerateCodePath, className);
                    ConfigFileImporter importer = context.fileName2ImporterDic[fileName];
                    ConfigCodeFileExporter exporter = new ConfigCodeFileExporter(outputFilePath, importer, context);
                    exporter.Export();
                }
            }

            // delete unuse cs class file
            string[] codeFiles = Directory.GetFiles(Settings.Inst.GenerateCodePath, "*.cs", SearchOption.AllDirectories);
            for (int i = 0; i < codeFiles.Length; i++)
            {
                string codeFileName = Path.GetFileName(codeFiles[i]);
                if (!fileClassNames.Contains(codeFileName))
                {
                    File.Delete(codeFiles[i]);
                    DebugUtil.Log($"delete unuse code class：{codeFiles[i]}");
                }
            }

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Generate Code", "Generate code success", "OK");
        }

        /// <summary>
        /// 增量导出配置表为二进制
        /// </summary>
        [MenuItem("XConfig/Generate Binary &b", false, 50)]
        public static void GenerateBinary()
        {
            if (EditorApplication.isCompiling)
            {
                EditorUtility.DisplayDialog("Generate Binary", "Wait for compilation to complete", "OK");
                return;
            }

            ClearConsole(); 

            // 导出所有配置
            ExportAllConfig(false);
        }

        /// <summary>
        /// 全量导出配置表为二进制
        /// </summary>
        [MenuItem("XConfig/Full Generate Binary", false, 50)]
        public static void FullGenerateBinary()
        {
            if (EditorApplication.isCompiling)
            {
                EditorUtility.DisplayDialog("Generate Binary", "Wait for compilation to complete", "OK");
                return;
            }

            ClearConsole();

            // 导出所有配置
            ExportAllConfig(true);
        }

        /// <summary>
        /// 导出所有配置
        /// </summary>
        /// <param name="isFullExport">是否全量</param>
        /// <param name="isNeedNotify">是否需要通知弹窗</param>
        /// <returns>配置信息</returns>
        public static Config ExportAllConfig(bool isFullExport = false)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Config config = RealExportConfig(isFullExport);
            if (config != null)
            {
                sw.Stop();
                EditorUtility.DisplayDialog("Generate Binary", $"Generate binary success, cost time {sw.ElapsedMilliseconds / 1000:N3}s", "OK");
            }
            return config;
        }

        static public Config RealExportConfig(bool isFullExport = false)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            if (!Directory.Exists(Settings.Inst.GenerateBinPath))
                Directory.CreateDirectory(Settings.Inst.GenerateBinPath);

            string[] filePaths = FileUtil.GetFiles(Settings.Inst.ConfigPath, Settings.Inst.FilePatterns,
                SearchOption.AllDirectories);
            Dictionary<string, ConfigRecordInfo> fileName2RecordDic = new Dictionary<string, ConfigRecordInfo>();
            foreach (string filePath in filePaths)
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                if (!Settings.Inst.IsFileExclude(fileName)) 
                {
                    string codeFileName = ConvertUtil.UnderscoreToCamel(fileName) + "Table";
                    string classFilePath = Settings.Inst.GenerateCodePath + codeFileName + ".cs";
                    string exportFilePath = Path.Combine(Settings.Inst.GenerateBinPath, $"{fileName}.bytes");
                    ConfigRecordInfo record = new ConfigRecordInfo(filePath, exportFilePath, classFilePath);
                    fileName2RecordDic.Add(fileName, record);
                }
            }

            // 获取需要导出的文件
            List<ConfigRecordInfo> changedFiles;
            if (!isFullExport)
            {
                ConfigRecordAsset record = ConfigRecordAsset.LoadRecord(TABLE_LAST_CHANGE_RECORD_PATH);
                changedFiles = record.FiltChangedRecord(fileName2RecordDic);
                Dictionary<string, bool> mark = new Dictionary<string, bool>();
                for (int i = 0; i < changedFiles.Count; i++)
                    mark.Add(changedFiles[i].sourceFileNameWithoutExtension, true);

                //如果是带继承关系的表，则其继承链上的所有表都要标记成有修改的文件
                int count = changedFiles.Count;
                for (int i = 0; i < count; i++)
                {
                    ConfigRecordInfo file = changedFiles[i];
                    List<string> inheritList = ConfigInherit.GetInheritTree(file);
                    for (int j = 0; inheritList != null && j < inheritList.Count; j++)
                    {
                        string inheritFileName = inheritList[j];
                        if (!mark.ContainsKey(inheritFileName))
                        {
                            DebugUtil.Assert(fileName2RecordDic.ContainsKey(inheritFileName), inheritFileName);
                            changedFiles.Add(fileName2RecordDic[inheritFileName]);
                            mark.Add(inheritFileName, true);
                        }
                    }
                }
            }
            else
                changedFiles = fileName2RecordDic.Select(x => x.Value).ToList();

            //创建导出上下文
            filePaths = changedFiles.Select(x => x.sourceFilePath).ToArray();
            ConfigFileContext context = new ConfigFileContext(filePaths, true);
            DebugUtil.Log($"create context cost:【{(float)sw.ElapsedMilliseconds/1000:N3}】");
            BytesBuffer buffer = new BytesBuffer(2 * 1024);
            foreach (ConfigRecordInfo recordFile in changedFiles)
            {
                sw.Reset();
                sw.Start();
                string inputFilePath = recordFile.sourceFilePath;
                string fileName = Path.GetFileNameWithoutExtension(inputFilePath);
                string outputFilePath = Settings.Inst.GenerateBinPath + fileName + ".bytes";
                ConfigFileImporter importer = context.fileName2ImporterDic[fileName];
                Config2BinFileExporter exporter = new Config2BinFileExporter(outputFilePath, importer, buffer);
                exporter.Export();
                float costTime = (float)sw.ElapsedMilliseconds / 1000;
                DebugUtil.Log($"export {fileName} cost:【{costTime:N3}】");
            }

            //生成配置实例
            Config config;
            if (!EditorApplication.isPlaying) // 正常游戏外刷表
            {
                sw.Reset();
                sw.Start();
                config = Config.Inst = new Config();
                config.Init(true);
                DebugUtil.Log($"Init cost:【{(float)sw.ElapsedMilliseconds/1000:N2}】");

                // 对配置做合法性检验
                config.CheckConfigAfterAllExport();
                DebugUtil.Log($"config.CheckConfigAfterAllExport cost:【{(float)sw.ElapsedMilliseconds/1000:N2}】");

                config.CheckPath(Settings.Inst.ConfigPath);
                AssetDatabase.Refresh();
            }
            else //游戏中刷表
            {
                config = Config.Inst;
                config.HotReload();
            }

            //所有都执行成功才保存记录
            if (!isFullExport)
            { 
                ConfigRecordAsset.SaveRecord(TABLE_LAST_CHANGE_RECORD_PATH, fileName2RecordDic);
                DebugUtil.Log($"save record file => {TABLE_LAST_CHANGE_RECORD_PATH}");
            }

            return Config.Inst;
        }

        /// <summary>
        /// 清除Unity Console输出
        /// </summary>
        public static void ClearConsole()
        {
            Assembly assembly = Assembly.Load("UnityEditor");
            Type logEntries = assembly.GetType("UnityEditor.LogEntries");
            var method = logEntries.GetMethod("Clear");
            method.Invoke(new object(), null);
        }
    }
}