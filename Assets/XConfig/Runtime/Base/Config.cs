using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using XConfig;

public partial class Config
{
    static public Config Inst;

    /// <summary>
    /// 配置表热重置，游戏运行期间重新读取修改后的配置到内存，方便策划调试
    /// </summary>
    public void HotReload()
    {
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        List<XTable> tables = new List<XTable>();
        BytesBuffer buffer = new BytesBuffer(2 * 1024);
        FieldInfo[] configFields = GetType().GetFields();
        foreach (FieldInfo tableField in configFields)
        {
            object[] attributes = tableField.GetCustomAttributes(typeof(BindCsvPathAttribute), false);
            if (attributes.Length > 0)//排除像Inst这样的字段
            {
                string binFileName = ConvertUtil.CamelToUnderscore(tableField.Name.Replace("Table", ""));
                string binFilePath = "Assets/art/config/" + binFileName + ".bytes";
                byte[] bytes = FileUtil.ReadAllBytes(binFilePath);
#if ASSERT_ENABLE
            DebugUtil.Assert(bytes != null, "找不到文件：{0}", binFilePath);
#endif
                buffer.Clear();
                buffer.WriteBytes(bytes, 0, bytes.Length);
                buffer.ReadString();
                XTable tbl = tableField.GetValue(this) as XTable;
                tbl.FromBytes(buffer);
                tables.Add(tbl);
            }
        }

        foreach (XTable tbl in tables)
            tbl.AllTableInitComplete();

        sw.Stop();
        Debug.LogFormat("配置表热加载成功，耗时:{0:N2} 秒", (float)sw.ElapsedMilliseconds / 1000);
    }
    /// <summary>
    /// 调用所有配置表的Init函数
    /// isFromGenerateConfig:是否来自导出配置表时的调用
    /// </summary>
    public void Init(bool isFromGenerateConfig = false)
    {
        string binFileSubPath = "Assets/XConfig/Example/GenerateBin/";
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        List<XTable> tables = new List<XTable>();
        BytesBuffer buffer = new BytesBuffer(2 * 1024);
        FieldInfo[] configFields = GetType().GetFields();
        foreach (FieldInfo tableField in configFields)
        {
            object[] attributes = tableField.GetCustomAttributes(typeof(BindCsvPathAttribute), false);
            if (attributes.Length > 0) // 排除像Inst这样的字段
            {
                string binFileName = ConvertUtil.CamelToUnderscore(tableField.Name.Replace("Table", ""));
                string binFilePath = binFileSubPath + binFileName + ".bytes";
                byte[] bytes = File.ReadAllBytes(binFilePath);
                DebugUtil.Assert(bytes != null, "找不到文件：{0}", binFilePath);
                buffer.Clear();
                buffer.WriteBytes(bytes, 0, bytes.Length);
                buffer.ReadString();
                XTable tbl = tableField.GetValue(this) as XTable;
                tbl.name = binFileName;
                tbl.FromBytes(buffer);
                tbl.InitRows();
                tables.Add(tbl);
            }
        }
        foreach (XTable tbl in tables)
        {
            tbl.AllTableInitComplete();
            if (isFromGenerateConfig)
                tbl.AllTableInitComplete();//为了检测AllTableInitComplete实现中是否造成了hotreload失效
        }
        sw.Stop();
        //Debug.LogFormat("配置表初始化耗时:{0} 秒", (float)sw.ElapsedMilliseconds / 1000);
    }

#if UNITY_EDITOR
    /// <summary>
    /// 在所有表导出完之后进行合法性检测，只会在editor模式下执行
    /// </summary>
    public void CheckConfigAfterAllExport()
    {
        bool isNoError = true;
        FieldInfo[] configFields = GetType().GetFields();
        foreach (FieldInfo configField in configFields)
        {
            object[] attributes = configField.GetCustomAttributes(typeof(BindCsvPathAttribute), false);
            if (attributes.Length > 0)//排除像Inst这样的字段
            {
                XTable tbl = configField.GetValue(this) as XTable;
                FieldInfo tableField = tbl.GetType().GetField("rows");
                if (tableField != null)
                {
                    object rows = tableField.GetValue(tbl);
                    if (CheckRowReferenceFieldIsValid(tbl.name, tableField, rows) == false)
                        isNoError = false;
                    CheckRowInExportTime(tbl, tableField, rows);
                    CheckRowFieldInExportTime(tbl, tableField, rows);
                }
                CheckTableInExportTime(tbl);
            }
        }
        DebugUtil.Assert(isNoError, "请解决完上述报错再重新导出");
    }
    /// <summary>
    /// 根据不同表定制化的检测
    /// </summary>
    /// <param name="tableField"></param>
    /// <param name="rows"></param>
    void CheckRowInExportTime(XTable tbl, FieldInfo tableField, object rows)
    {
        Type rowType = tableField.FieldType.GetGenericArguments()[0];
        if (typeof(ICheckTableRowExportTime).IsAssignableFrom(rowType))
        {
            Type rowsType = typeof(List<>).MakeGenericType(tableField.FieldType.GetGenericArguments()[0]);
            int rowCount = Convert.ToInt32(rowsType.GetProperty("Count").GetValue(rows, null));
            for (int i = 0; i < rowCount; i++)
            {
                XRow row = rowsType.GetProperty("Item").GetValue(rows, new object[] { i }) as XRow;
                MethodInfo checkRowConfig = rowType.GetMethod("CheckRowInExportTime", BindingFlags.Instance | BindingFlags.Public);
                row.fileName = tbl.name;
                row.rowIndex = i + 5;//前4行是格式行，第5行开始才是真正的内容
                checkRowConfig.Invoke(row, null);
            }
        }
    }
    void CheckRowFieldInExportTime(XTable tbl, FieldInfo tableField, object rows)
    {
        Type rowType = tableField.FieldType.GetGenericArguments()[0];
        Type rowsType = typeof(List<>).MakeGenericType(tableField.FieldType.GetGenericArguments()[0]);
        int rowCount = Convert.ToInt32(rowsType.GetProperty("Count").GetValue(rows, null));
        FieldInfo[] rowFields = rowType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (FieldInfo rowField in rowFields)
        {
            if (typeof(IUserDefineType).IsAssignableFrom(rowField.FieldType))
            {
                MethodInfo checkFiedMethod = rowField.FieldType.GetMethod("CheckConfigValid", BindingFlags.Instance | BindingFlags.Public);
                for (int i = 0; i < rowCount; i++)
                {
                    XRow row = rowsType.GetProperty("Item").GetValue(rows, new object[] { i }) as XRow;
                    IUserDefineType userDefineType = rowField.GetValue(row) as IUserDefineType;
                    if (userDefineType != null)
                    {
                        string result = userDefineType.CheckConfigValid();
                        DebugUtil.Assert(string.IsNullOrEmpty(result), $"表={tbl.name} 行={row.rowIndex} 列={rowField.FieldType.Name} {result}");
                    }
                }
            }
        }
    }
    void CheckTableInExportTime(XTable tbl)
    {
        if (typeof(ICheckTableExportTime).IsAssignableFrom(tbl.GetType()))
        {
            MethodInfo method = tbl.GetType().GetMethod("CheckTableInExportTime", BindingFlags.Instance | BindingFlags.Public);
#if ASSERT_ENABLE
        DebugUtil.Assert(method != null, "表 {0} 找不到CheckTableInExportTime函数", tbl.name);
#endif
            method.Invoke(tbl, null);
        }
    }
    /// <summary>
    /// 检测引用字段引用的值是否合法
    /// </summary>
    bool CheckRowReferenceFieldIsValid(string tableName, FieldInfo tableField, object rows)
    {
        bool isNoError = true;
        DebugUtil.Assert(rows != null, "tableName={0} filedName={1}", tableName, tableField.Name);
        Type rowsType = typeof(List<>).MakeGenericType(tableField.FieldType.GetGenericArguments()[0]);
        object value = rowsType.GetProperty("Count").GetValue(rows, null);
        int count = Convert.ToInt32(value);
        if (count > 0)
        {
            XRow row = rowsType.GetProperty("Item").GetValue(rows, new object[] { 0 }) as XRow;
            List<PropertyInfo> referenceProperties = GetRowReferenceProperties(row);
            if (referenceProperties.Count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    row = rowsType.GetProperty("Item").GetValue(rows, new object[] { i }) as XRow;
                    if (CheckRow(tableName, row, referenceProperties) == false)
                        isNoError = false;
                }
            }
        }
        return isNoError;
    }
    bool CheckRow(string tableName, XRow row, List<PropertyInfo> referenceProperties)
    {
        bool isNoError = true;
        foreach (PropertyInfo property in referenceProperties)
        {
            try
            {
                property.GetValue(row, null);
            }
            catch (Exception e)
            {
                isNoError = false;
                DebugUtil.LogError("{0} 表 {1} 列 引用的字段值不存在\n{2}", tableName, property.Name, e.ToString());
            }
        }
        return isNoError;
    }
    List<PropertyInfo> GetRowReferenceProperties(XRow row)
    {
        List<PropertyInfo> referenceProperties = new List<PropertyInfo>();
        Type rowType = row.GetType();
        FieldInfo[] rowFields = rowType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (FieldInfo rowField in rowFields)
        {
            object[] attributes = rowField.GetCustomAttributes(typeof(CsvReferenceAttribute), false);
            if (attributes.Length > 0)
            {
                CsvReferenceAttribute ca = attributes[0] as CsvReferenceAttribute;
                PropertyInfo property = rowType.GetProperty(ca.property, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                referenceProperties.Add(property);
            }
        }
        return referenceProperties;
    }

    /// <summary>
    /// 检查配置文件路径的合法性
    /// </summary>
    public void CheckPath(string configPath)
    {
        string[] files = Directory.GetFiles(configPath, "*.bytes", SearchOption.AllDirectories);
        Dictionary<string, string> fileName2Path = new Dictionary<string, string>();//用于检测配置表不能重名

        for (int i = 0; i < files.Length; i++)//判断config目录下是否直接存放了配置表
        {
            string[] formatFile = files[i].Replace("\\", "/").Split('/');
            string fileName = formatFile[formatFile.Length - 1];
            if (fileName2Path.ContainsKey(fileName))
                DebugUtil.Assert(false, "配置表不能重名：{0} 与 {1}", fileName2Path[fileName], files[i]);
            fileName2Path.Add(fileName, files[i]);
            if (formatFile[formatFile.Length - 2] == "config")
            {
                if (fileName == "csv_template.bytes")
                    continue;
                DebugUtil.Assert(false, "配置表不能直接存放在config目录下");
            }
        }
    }
#endif
}



