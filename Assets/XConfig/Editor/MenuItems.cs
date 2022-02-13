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
        static public readonly string TABLE_LAST_CHANGE_RECORD_PATH = $"Assets/Example/asset_records/records_{CONFIG_FORMAT_VERSION}.asset";

        [MenuItem("XConfig/Generate Code &g", false, 1)]
        public static void GenerateCode()
        {
            if (EditorApplication.isCompiling)
            {
                EditorUtility.DisplayDialog("Generate Code", "Wait for compilation to complete", "OK");
                return;
            }

            string[] files = FileUtil.GetFiles(Settings.Inst.CONFIG_PATH, Settings.Inst.FilePatterns, SearchOption.AllDirectories);
            List<string> fileClassNames = new List<string>(files.Length);
            ConfigFileContext context = new ConfigFileContext(files);
            foreach (var file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                if (!Settings.Inst.IsFileExclude(fileName))
                {
                    string className = ConvertUtil.Convert2HumpNamed(fileName) + "Table.cs";
                    fileClassNames.Add(className);

                    string outputFilePath = Path.Combine(Settings.Inst.GENERATE_CODE_PATH, className);
                    ConfigFileImporter importer = context.fileName2ImporterDic[fileName];
                    ConfigCodeFileExporter exporter = new ConfigCodeFileExporter(outputFilePath, importer, context);
                    exporter.Export();
                }
            }

            // delete unuse cs class file
            string[] codeFiles = Directory.GetFiles(Settings.Inst.GENERATE_CODE_PATH, "*.cs", SearchOption.AllDirectories);
            for (int i = 0; i < codeFiles.Length; i++)
            {
                string codeFileName = Path.GetFileName(codeFiles[i]);
                if (!fileClassNames.Contains(codeFileName))
                {
                    File.Delete(codeFiles[i]);
                    Debug.LogFormat($"delete unuse code class：{codeFiles[i]}");
                }
            }

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Generate Code", "Generate Code Success", "OK");
        }

        /// <summary>
        /// 导出配置表为二进制
        /// </summary>
        [MenuItem("XConfig/Generate Binary &r", false, 1)]
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
        /// 导出所有配置
        /// </summary>
        /// <param name="isFullExport">是否全量</param>
        /// <param name="isNeedNotify">是否需要通知弹窗</param>
        /// <returns>Csv配置信息</returns>
        public static Config ExportAllConfig(bool isFullExport = false)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Config config = RealExportConfig(isFullExport);
            if (config != null)
            {
                sw.Stop();
                EditorUtility.DisplayDialog("配置生成",
                string.Format("配置生成成功，耗时：{0:N3}秒", sw.ElapsedMilliseconds / 1000),
                "确认");
            }
            return config;
        }

        static public Config RealExportConfig(bool isFullExport = false)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            if (!Directory.Exists(Settings.Inst.CONFIG_BYTES_OUTPUT_PATH))
                Directory.CreateDirectory(Settings.Inst.CONFIG_BYTES_OUTPUT_PATH);
            //计算需要重新导出的配置数组
            string[] filePaths = FileUtil.GetFiles(Settings.Inst.CONFIG_PATH, new string[] { "*.bytes", "*.server", "*.web" },
                SearchOption.AllDirectories);
            List<NeedRecordTable> allNeedRecordFiles = new List<NeedRecordTable>();
            List<NeedRecordTable> changedFiles;
            Dictionary<string, NeedRecordTable> filePath2RecordDic = new Dictionary<string, NeedRecordTable>();
            foreach (string filePath in filePaths)
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                if (!Settings.Inst.IsFileExclude(fileName)) 
                {
                    string classFileName = ConvertUtil.Convert2HumpNamed(fileName) + "Table";
                    string classFilePath = Settings.Inst.GENERATE_CODE_PATH + classFileName + ".cs";
                    string binFilePath = Settings.Inst.CONFIG_BYTES_OUTPUT_PATH + fileName + ".bytes";
                    NeedRecordTable record = new NeedRecordTable(filePath, classFilePath, binFilePath);
                    allNeedRecordFiles.Add(record);
                    filePath2RecordDic.Add(record.csvFileNameWithoutExtension, record);
                }
            }

            if (!isFullExport)
            {
                TableRecordFile record = TableRecordFile.LoadRecord(TABLE_LAST_CHANGE_RECORD_PATH);
                changedFiles = record.GetChangedNros(allNeedRecordFiles);
                Dictionary<string, bool> mark = new Dictionary<string, bool>();
                for (int i = 0; i < changedFiles.Count; i++)
                    mark.Add(changedFiles[i].csvFileNameWithoutExtension, true);
                //如果是带继承关系的表，则其继承链上的所有表都要标记成有修改的文件
                int count = changedFiles.Count;
                for (int i = 0; i < count; i++)
                {
                    NeedRecordTable file = changedFiles[i];
                    List<string> inheritList = ConfigInherit.GetInheritTree(file);
                    for (int j = 0; inheritList != null && j < inheritList.Count; j++)
                    {
                        string csvFileName = inheritList[j];
                        if (!mark.ContainsKey(csvFileName))
                        {
                            DebugUtil.Assert(filePath2RecordDic.ContainsKey(csvFileName), csvFileName);
                            changedFiles.Add(filePath2RecordDic[csvFileName]);
                            mark.Add(csvFileName, true);
                        }
                    }
                }
            }
            else
                changedFiles = allNeedRecordFiles;

            filePaths = changedFiles.Select(x => x.csvFilePath).ToArray();
            //创建导出上下文
            ConfigFileContext context = new ConfigFileContext(filePaths, true);
            Debug.LogFormat("create context cost:【{0:N2}】", (float)sw.ElapsedMilliseconds / 1000);
            sw.Reset();
            sw.Start();
            BytesBuffer buffer = new BytesBuffer(2 * 1024);
            foreach (NeedRecordTable recordFile in changedFiles)
            {
                string inputFilePath = recordFile.csvFilePath;
                string fileName = Path.GetFileNameWithoutExtension(inputFilePath);
                string outputFilePath = Settings.Inst.CONFIG_BYTES_OUTPUT_PATH + fileName + ".bytes";
                ConfigFileImporter importer = context.fileName2ImporterDic[fileName];
                Config2BinFileExporter exporter = new Config2BinFileExporter(outputFilePath, importer, buffer);
                exporter.Export();
                float costTime = (float)sw.ElapsedMilliseconds / 1000;
                if (costTime > 0.1)
                    Debug.LogFormat("{0}:【{1:N2}】", fileName, costTime);
                sw.Reset();
                sw.Start();
            }

            Debug.LogFormat("csv2bin cost:【{0:N2}】", (float)sw.ElapsedMilliseconds / 1000);
            sw.Reset();
            sw.Start();
            //生成配置实例
            Config config;
            if (!EditorApplication.isPlaying) // 正常游戏外刷表
            {
                config = Config.Inst = new Config();
                config.Init(true);
                Debug.LogFormat("Init cost:【{0:N2}】", (float)sw.ElapsedMilliseconds / 1000);
                //对配置做合法性检验
                config.CheckConfigAfterAllExport();
                Debug.LogFormat("config.CheckConfigAfterAllExport cost:【{0:N2}】", sw.ElapsedMilliseconds / 1000);
                sw.Reset();
                sw.Start();
                //不合法路径
                config.CheckPath(Settings.Inst.CONFIG_PATH);
                AssetDatabase.Refresh();
                Debug.LogFormat("config.CheckPath cost:【{0:N2}】", sw.ElapsedMilliseconds / 1000);
                //Debug.Log(Md5Util.GetFileMd5(CONFIG_ASSET_PATH));
            }
            else //游戏中刷表
            {
                config = Config.Inst;
                config.HotReload();
            }

            //所有都执行成功才保存记录
            if (!isFullExport)
                TableRecordFile.SaveRecord<NeedRecordTable>(TABLE_LAST_CHANGE_RECORD_PATH, allNeedRecordFiles);

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