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
        public const string CONFIG_FILE_NAME = "Config.cs";

        [MenuItem("XConfig/Generate Code &c", false, 1)]
        public static void GenerateCode()
        {
            if (EditorApplication.isCompiling)
            {
                EditorUtility.DisplayDialog("Generate Code", "Wait for compilation to complete", "OK");
                return;
            }

            ClearConsole();

            string[] files = FileUtil.GetFiles(Settings.Inst.ConfigPath, Settings.Inst.SourceFilePatterns, SearchOption.AllDirectories);
            List<string> fileClassNames = new List<string>(files.Length);
            ConfigFileContext context = new ConfigFileContext(files);
            files = files.Select(x => Path.GetFileNameWithoutExtension(x)).Where(x => !Settings.Inst.IsFileExclude(x)).ToArray();
            for (int i = 0; i < files.Length; i++)
            {
                var fileName = files[i];
                string className = $"{StringUtil.FileNameToTableName(fileName)}.cs";
                fileClassNames.Add(className);
                string outputFilePath = Path.Combine(Settings.Inst.GenerateCodePath, className);
                ConfigFileImporter importer = context.fileName2Importer[fileName];
                SingleConfigCodeFileExporter singleExporter = new SingleConfigCodeFileExporter(outputFilePath, importer, context);
                singleExporter.Export();
                outputFilePath = Path.Combine(Settings.Inst.GenerateCodePath, CONFIG_FILE_NAME);
                ConfigCodeFileExporter exporter = new ConfigCodeFileExporter(outputFilePath, context);
                exporter.Export();
                EditorUtility.DisplayProgressBar("Generate Code", $"Generate {className} ({i+1}/{files.Length})", ((float)i+1 / files.Length));
            }

            // delete unuse cs class file
            string[] codeFiles = Directory.GetFiles(Settings.Inst.GenerateCodePath, "*.cs", SearchOption.AllDirectories);
            for (int i = 0; i < codeFiles.Length; i++)
            {
                string codeFileName = Path.GetFileName(codeFiles[i]);
                if (!fileClassNames.Contains(codeFileName) && codeFileName != CONFIG_FILE_NAME)
                {
                    File.Delete(codeFiles[i]);
                    DebugUtil.Log($"delete unuse code class：{codeFiles[i]}");
                }
            }

            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
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
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("Generate Binary", $"Generate binary success, cost time {sw.ElapsedMilliseconds / 1000:N3}s", "OK");
            }
            return config;
        }

        public static Config RealExportConfig(bool isFullExport = false)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            if (!Directory.Exists(Settings.Inst.GenerateBinPath))
                Directory.CreateDirectory(Settings.Inst.GenerateBinPath);

            string[] filePaths = FileUtil.GetFiles(Settings.Inst.ConfigPath, Settings.Inst.SourceFilePatterns,
                SearchOption.AllDirectories);
            Dictionary<string, ConfigRecordInfo> fileName2RecordDic = new Dictionary<string, ConfigRecordInfo>();
            foreach (string filePath in filePaths)
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                if (!Settings.Inst.IsFileExclude(fileName)) // 过滤的表
                {
                    string codeFileName = StringUtil.FileNameToTableName(fileName);
                    string classFilePath = Path.Combine(Settings.Inst.GenerateCodePath, $"{codeFileName}.cs");
                    string exportFilePath = Path.Combine(Settings.Inst.GenerateBinPath, $"{fileName}.{Settings.Inst.OutputFileExtend}");
                    ConfigRecordInfo record = new ConfigRecordInfo(filePath, exportFilePath, classFilePath);
                    fileName2RecordDic.Add(fileName, record);
                }
            }

            //创建导出上下文
            ConfigFileContext context = new ConfigFileContext(filePaths, true);
            DebugUtil.Log($"create context cost:【{(float)sw.ElapsedMilliseconds/1000:N3}】");

            // 获取需要导出的文件
            List<ConfigRecordInfo> changedFiles;
            if (!isFullExport)
            {
                ConfigRecordAsset record = ConfigRecordAsset.LoadRecord(TABLE_LAST_CHANGE_RECORD_PATH);
                changedFiles = record.FiltChangedRecord(fileName2RecordDic);
            }
            else
                changedFiles = fileName2RecordDic.Select(x => x.Value).ToList();

            BytesBuffer buffer = new BytesBuffer(2 * 1024);
            for(int i = 0; i < changedFiles.Count; i++)
            {
                ConfigRecordInfo recordFile = changedFiles[i];
                sw.Reset();
                sw.Start();
                string inputFilePath = recordFile.sourceFilePath;
                string fileName = Path.GetFileNameWithoutExtension(inputFilePath);
                string outputFullFileName = $"{fileName}.{Settings.Inst.OutputFileExtend}";
                string outputFilePath = Path.Combine(Settings.Inst.GenerateBinPath, outputFullFileName);
                ConfigFileImporter importer = context.fileName2Importer[fileName];
                Config2BinFileExporter exporter = new Config2BinFileExporter(outputFilePath, importer, buffer);
                exporter.Export();
                float costTime = (float)sw.ElapsedMilliseconds / 1000;
                DebugUtil.Log($"export {fileName} cost:【{costTime:N3}】");
                EditorUtility.DisplayProgressBar("Generate Binary", $"Generate {outputFullFileName} ({i + 1}/{changedFiles.Count})", ((float)i+1 / changedFiles.Count));
            }

            //生成配置实例
            Config config;
            if (!EditorApplication.isPlaying) // 导表
            {
                sw.Restart();
                config = Config.Inst = new Config();
                config.Init(true);
                DebugUtil.Log($"Init cost:【{(float)sw.ElapsedMilliseconds/1000:N3}】");

                // 对配置做合法性检验
                sw.Restart();
                config.CheckConfigAfterExport();
                DebugUtil.Log($"CheckConfigAfterExport cost:【{(float)sw.ElapsedMilliseconds/1000:N3}】");

                ConfigUtil.CheckPath(Settings.Inst.ConfigPath);
                AssetDatabase.Refresh();
            }
            else // 热刷表
            {
                config = Config.Inst;
                config.HotReload();
            }

            // delete unuse bin file
            string[] binFiles = Directory.GetFiles(Settings.Inst.GenerateBinPath, $"*.{Settings.Inst.OutputFileExtend}", SearchOption.AllDirectories);
            for (int i = 0; i < binFiles.Length; i++)
            {
                string codeFileName = Path.GetFileNameWithoutExtension(binFiles[i]);
                if (!fileName2RecordDic.ContainsKey(codeFileName))
                {
                    File.Delete(binFiles[i]);
                    DebugUtil.Log($"delete unuse bin file：{binFiles[i]}");
                }
            }

            //所有都执行成功才保存记录
            if (!isFullExport)
            { 
                ConfigRecordAsset.SaveRecord(TABLE_LAST_CHANGE_RECORD_PATH, fileName2RecordDic);
                DebugUtil.Log($"save record file => {TABLE_LAST_CHANGE_RECORD_PATH}");
            }

            return Config.Inst;
        }

        [MenuItem("XConfig/Run Test Case &t", false, 50)]
        public static void RunTestCase() 
        {
            ClearConsole();

            TestCase.RunAllTestCase();
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