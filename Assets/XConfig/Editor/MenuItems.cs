using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace XConfig.Editor
{
    public static class MenuItems
    {
        //配置表格式版本号，当配置表格式变化时，可修改此版本号，触发全量导出，避免资源跟代码格式不匹配的情况
        public const int CONFIG_FORMAT_VERSION = 2;
        static public readonly string TABLE_LAST_CHANGE_RECORD_PATH = $"Assets/asset_records/records_{CONFIG_FORMAT_VERSION}.asset";
        static public readonly string CONFIG_ASSETS_OUTPUT_PATH = "Assets/XConfig/Example/GenerateBin/";
        static public readonly string GENERATE_CLASS_PATH = "Assets/XConfig/Example/GenerateCode/";
        static public string tableNameFilter = ""; //空表示不过滤

        /// <summary>
        /// 生成配置表代码
        /// </summary>
        [MenuItem("XConfig/GenerateCode")]
        public static void GenerateCode()
        {
            if (EditorApplication.isCompiling)
            {
                EditorUtility.DisplayDialog("前端配置代码生成失败", "正在编译c#代码，请等待编译完再生成", "确认");
                return;
            }

            CsvInherit.Init();
            string[] files = FileUtil.GetFiles(CsvFileImporter.CONFIG_PATH, new string[] { "*.bytes", "*.server", "*.web" },
                SearchOption.AllDirectories);
            CsvFileContext context = new CsvFileContext(files);
            foreach (var file in files)
            {
                string inputFilePath = file;
                string fileName = Path.GetFileNameWithoutExtension(file);
                if (string.IsNullOrEmpty(tableNameFilter) || fileName == tableNameFilter) //添加导出过滤支持，方便出问题时对某个表进行单独导出
                {
                    if (fileName != "csv_template")
                    {
                        string outputFilePath = GENERATE_CLASS_PATH +
                                                ConvertUtil.Convert2HumpNamed(fileName) + "Table.cs";
                        new Csv2CSharpAdapter(inputFilePath, outputFilePath, fileName, context).Convert();
                    }
                }
            }

            //转换文件名
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = ConvertUtil.Convert2HumpNamed(Path.GetFileNameWithoutExtension(files[i]));
                //Debug.Log(files[i]);
            }

            //删除不存在对应cs文件的配置类c#文件
            string[] classFiles = Directory.GetFiles(GENERATE_CLASS_PATH, "*.cs");
            for (int i = 0; i < classFiles.Length; i++)
            {
                string classFileName = Path.GetFileNameWithoutExtension(classFiles[i]).Replace("Table", "");
                bool isExistCsvFile = false;
                for (int j = 0; j < files.Length; j++)
                {
                    if (files[j] == classFileName)
                    {
                        isExistCsvFile = true;
                        break;
                    }
                }

                if (!isExistCsvFile)
                {
                    File.Delete(classFiles[i]);
                    Debug.LogFormat("删除废弃的配置表类文件：{0}", classFiles[i]);
                }
            }

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("生成配置代码", "生成配置代码成功\n\n【请把焦点切换到别的应用，然后再切回来，以触发代码编译，否则配置表导出会不正确！】", "我知道了");
        }

        /// <summary>
        /// 导出配置表为二进制
        /// </summary>
        [MenuItem("XConfig/GenerateBin")]
        public static void GenerateBin()
        {
            if (EditorApplication.isCompiling)
            {
                EditorUtility.DisplayDialog("配置生成失败", "正在编译c#代码，请等待编译完再生成配置", "确认");
                return;
            }

            ClearConsole();

            // 导出所有配置
            ExportAllConfig(false, true);
        }

        /// <summary>
        /// 导出所有配置
        /// </summary>
        /// <param name="isFullExport">是否全量</param>
        /// <param name="isNeedNotify">是否需要通知弹窗</param>
        /// <returns>Csv配置信息</returns>
        /// <author>小智</author>
        public static Config ExportAllConfig(bool isFullExport = false, bool isNeedNotify = true)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            Config config = RealExportConfig(isFullExport);
            if (config != null)
            {
                sw.Stop();

                if (isNeedNotify)
                {
                    EditorUtility.DisplayDialog("配置生成",
                    string.Format("配置生成成功，耗时：{0:N1}秒", sw.ElapsedMilliseconds / 1000),
                    "确认");
                }
            }
            return config;
        }

        static public Config RealExportConfig(bool isFullExport = false)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            CsvInherit.Init();
            if (!Directory.Exists(CONFIG_ASSETS_OUTPUT_PATH))
                Directory.CreateDirectory(CONFIG_ASSETS_OUTPUT_PATH);
            //计算需要重新导出的配置数组
            string[] filePaths = FileUtil.GetFiles(CsvFileImporter.CONFIG_PATH, new string[] { "*.bytes", "*.server", "*.web" },
                SearchOption.AllDirectories);
            List<NeedRecordTable> allNeedRecordFiles = new List<NeedRecordTable>();
            List<NeedRecordTable> changedFiles;
            Dictionary<string, NeedRecordTable> filePath2RecordDic = new Dictionary<string, NeedRecordTable>();
            foreach (string filePath in filePaths)
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                if (fileName != "csv_template") //csv_template是模板配置，忽略
                {
                    string classFileName = ConvertUtil.Convert2HumpNamed(fileName) + "Table";
                    string classFilePath = GENERATE_CLASS_PATH + classFileName + ".cs";
                    string binFilePath = CONFIG_ASSETS_OUTPUT_PATH + fileName + ".bytes";
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
                    List<string> inheritList = CsvInherit.GetInheritTree(file);
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
            CsvFileContext context = new CsvFileContext(filePaths, true);
            Debug.LogFormat("create context cost:【{0:N2}】", (float)sw.ElapsedMilliseconds / 1000);
            sw.Reset();
            sw.Start();
            BytesBuffer buffer = new BytesBuffer(2 * 1024);
            foreach (NeedRecordTable recordFile in changedFiles)
            {
                string inputFilePath = recordFile.csvFilePath;
                string fileName = Path.GetFileNameWithoutExtension(inputFilePath);
                if (string.IsNullOrEmpty(tableNameFilter) || fileName == tableNameFilter) //添加导出过滤支持，方便出问题时对某个表进行单独导出
                {
                    string outputFilePath = CONFIG_ASSETS_OUTPUT_PATH + fileName + ".bytes";
                    new Csv2BinAdapter(inputFilePath, outputFilePath, fileName, context).Convert(buffer);
                    float costTime = (float)sw.ElapsedMilliseconds / 1000;
                    if (costTime > 0.1)
                        Debug.LogFormat("{0}:【{1:N2}】", fileName, costTime);
                    sw.Reset();
                    sw.Start();
                }
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
                config.CheckPath(CsvFileImporter.CONFIG_PATH);
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