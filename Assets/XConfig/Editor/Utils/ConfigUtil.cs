using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using XConfig;

public static class ConfigUtil
{
    /// <summary>
    /// 在所有表导出完之后进行合法性检测
    /// </summary>
    public static void CheckConfigAfterExport(this Config config)
    {
        FieldInfo[] configFields = typeof(Config).GetFields();
        foreach (FieldInfo configField in configFields)
        {
            var attribute = configField.GetCustomAttribute<BindConfigFileNameAttribute>(false);
            if (attribute != null)
            {
                XTable tbl = configField.GetValue(config) as XTable;
                CheckRows(tbl);
                CheckTable(tbl);
            }
        }
    }

    static void CheckRows(XTable tbl) 
    {
        Type type = tbl.GetType();
        PropertyInfo rowsProperty = type.GetProperty("rows");
        if (rowsProperty != null)
        {
            var rows = rowsProperty.GetValue(tbl) as IList;
            Type rowType = rowsProperty.PropertyType.GetGenericArguments()[0];
            
            // 所有引用属性
            var refProperties = rowType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.GetCustomAttribute<ConfigReferenceAttribute>(false) != null);
            
            // 所有自定义类型变量
            var customTypeFields = rowType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(x => typeof(IConfigType).IsAssignableFrom(x.FieldType));

            for (int i = 0; i < rows.Count; i++)
            {
                var row = rows[i] as XRow;
                int rowIndex = i + 5; // 前4行是格式行，第5行开始才是真正的内容
                try
                {
                    // 检查表中引用
                    CheckRowRefPropertiesValid(tbl, refProperties, row);

                    // 自定义类型的检查
                    CheckRowCustomTypeFieldWhenExport(tbl, customTypeFields, rowIndex, row);

                    // partial类自定义行检测
                    row.OnCheckWhenExport();
                }
                catch (Exception e)
                {
                    DebugUtil.LogError($"表 = {tbl.name} 行 {rowIndex} 异常, \n{e}");
                }
            }
        }
    }

    /// <summary>
    /// 检测引用字段引用的值是否合法
    /// </summary>
    static void CheckRowRefPropertiesValid(XTable tbl, IEnumerable<PropertyInfo> refProperties, XRow row)
    {
        foreach (var property in refProperties)
        {
            try { property.GetValue(row, null); }
            catch (Exception e)
            {
                DebugUtil.Assert(false, $"表 = {tbl.name} 列 = {property.Name} 引用的字段值不存在, {e}");
            }
        }
    }

    static void CheckRowCustomTypeFieldWhenExport(XTable tbl, IEnumerable<FieldInfo> customTypeFields, int rowIndex, XRow row)
    {
        foreach (var rowField in customTypeFields)
        {
            var value = rowField.GetValue(row);
            if (value is IConfigType configType)
            {
                try { configType.CheckConfigValid(configType); }
                catch (Exception e)
                {
                    DebugUtil.LogError($"表 = {tbl.name}, 列 = {rowField.FieldType.Name}, 行 = {rowIndex} 异常, \n{e}");
                }
            }
        }
    }

    static void CheckTable(XTable tbl)
    {
        try
        {
            tbl.OnCheckWhenExport();
        }
        catch (Exception e)
        {
            DebugUtil.LogError($"表 = {tbl.name} 异常, \n{e}");
        }
    }

    /// <summary>
    /// 检查配置文件路径的合法性
    /// </summary>
    public static void CheckPath(string configPath)
    {
        string[] files = FileUtil.GetFiles(configPath, Settings.Inst.SourceFilePatterns, SearchOption.AllDirectories);
        Dictionary<string, string> fileName2Path = new Dictionary<string, string>();//用于检测配置表不能重名

        for (int i = 0; i < files.Length; i++)//判断config目录下是否直接存放了配置表
        {
            string filePath = files[i];
            string fileName = Path.GetFileNameWithoutExtension(filePath);

            if (!Settings.Inst.IsFileExclude(fileName))
                continue;

            if (fileName2Path.ContainsKey(fileName))
                DebugUtil.Assert(false, "配置表不能重名：{0} 与 {1}", fileName2Path[fileName], files[i]);
            fileName2Path.Add(fileName, files[i]);
        }
    }
}
