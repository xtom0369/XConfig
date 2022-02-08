using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public interface IRowInitComplete
{
    /// <summary>
    /// 只限CsvRow的子类继承此接口
    /// 所有表初始化完毕后，用于二次处理的函数，譬如建立更多表之间的关联或者增加一些新的字段给表行
    /// 在editor下和游戏运行时都会被调用到，并且会在AfterTableInitComplete之前调用
    /// 【千万注意！】不要在里面去写Assert的检测代码，
    /// 要写就写在CheckRowInExportTime或CheckTableInExportTime，特殊情况除外
    /// </summary>
    void AfterRowInitComplete();
}
public interface ITableInitComplete
{
    /// <summary>
    /// 只限CsvTable的子类继承此接口
    /// 所有表初始化完毕后，用于二次处理的函数，譬如建立不同于默认字典的集合来关联表行
    /// 在editor下和游戏运行时都会被调用到
    /// 【千万注意！】不要在里面去写Assert的检测代码，
    /// 要写就写在CheckRowInExportTime或CheckTableInExportTime，特殊情况除外
    /// 配置表热加载时会调用此方法，所以要先清掉现有数据
    /// </summary>
    void AfterTableInitComplete();
}
public interface ICheckTableRowExportTime
{
    /// <summary>
    /// 只限CsvRow的子类继承此接口
    /// 生成配置时会被调用到，时机是在所有表都导出完之后才进行的合法性检测
    /// 只会在editor模式下执行，时机在CheckTableInExportTime之前
    /// </summary>
    void CheckRowInExportTime();
}
public interface ICheckTableExportTime
{
    /// <summary>
    /// 只限CsvTable的子类继承此接口
    /// 生成配置时会被调用到，时机是在所有表都导出完之后才进行的合法性检测
    /// 只会在editor模式下执行
    /// </summary>
    void CheckTableInExportTime();
}

public class CsvTable
{
    static public System.Text.Encoding ENCODING;
    static public char[] SEPARATOR = { '\t' };

    public string name;

    public string keys;
    public string comments;
    public string types;
    public string flags;

    virtual public void FromBytes(BytesBuffer buffer)
    {
    }
    virtual public void InitRows()
    {
    }
    virtual public void AllTableInitComplete()
    {
    }
    virtual public void ExportCsv()
    {

    }

    virtual protected bool IsOverrideSort()
    {
        return false;
    }

    virtual protected int Sort(CsvRow left, CsvRow right)
    {
        return 0;
    }
    /*
    public List<string> keys;
    public List<string> majorKeys;
    public List<string> majorTypes;
    [NonSerialized]
    public string curParseFilePath;

#if UNITY_EDITOR
    public void Load(string path)
    {
        //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        //sw.Start();
        curParseFilePath = path;
        string str = GetFileStr();
        //Debug.LogFormat("{0} GetFileStr cost:【{1:N2}】", curParseFilePath, sw.ElapsedMilliseconds / 1000);
        //sw.Reset();
        //sw.Start();
        ParseString2RowList(str);
        //Debug.LogFormat("{0} ParseString2RowList cost:【{1:N2}】", curParseFilePath, sw.ElapsedMilliseconds / 1000);
    }
    string GetFileStr()
    {
        //不会锁死, 允许其它程序打开
        FileStream fileStream = new FileStream(curParseFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Dispose();
        return ENCODING.GetString(data);
    }
    void ParseString2RowList(string fileStr)
    {
        Type rowsType = null;
        Type rowType = null;
        object rows = null;
        FieldInfo[] fields = this.GetType().GetFields();
        foreach (FieldInfo field in fields)
            if (field.Name == "rows")
            {
                rowType = field.FieldType.GetGenericArguments()[0];
                Type generic = typeof(List<>);
                rowsType = generic.MakeGenericType(rowType);
                rows = Activator.CreateInstance(rowsType);
                field.SetValue(this, rows);
                break;
            }
        DebugUtil.Assert(rows != null, "配置表子类需要定义rows变量 {0}", curParseFilePath);

        using (StringReader oReader = new StringReader(fileStr))
        {
            name = curParseFilePath.Replace("../config/", "");
            //第一行为key
            // 跳过BOM头
            string rowStr = oReader.ReadLine();
            byte[] bLine = ENCODING.GetBytes(rowStr);
            if (bLine[0] == 0xEF)
                rowStr = ENCODING.GetString(bLine, 3, rowStr.Length - 3);
            if (rowStr == null)
            {
                Debug.Assert(false, curParseFilePath + " 表内容不能为空,请先alt+r刷新配置试试！");
            }
            keys = new List<string>(rowStr.Split(SEPARATOR));
            CheckColumnNotEmpty();
            //第二行为备注
            oReader.ReadLine();
            //第三行列的数据类型和默认值
            rowStr = oReader.ReadLine();
            string[] types = rowStr.Split(SEPARATOR);
            //第四行为列标识
            rowStr = oReader.ReadLine();
            //if (string.IsNullOrEmpty(rowStr)) return;
            Debug.Assert(!string.IsNullOrEmpty(rowStr), curParseFilePath);
            string[] keyFlags = rowStr.Split(SEPARATOR);
            GetMajorKeys(keyFlags, keys, types, out majorKeys, out majorTypes);
            FilterNotUseColoum(keyFlags, keys);//把不需要的列名设置为空串
            //第五行开始为真正的内容
            int rowIndex = 4;
            while ((rowStr = oReader.ReadLine()) != null)
            {
                rowIndex++;
                if (!string.IsNullOrEmpty(rowStr) && !IsEmptyLineOrCommentLine(rowStr))//跳过空行
                {
                    try
                    {
                    //DebugUtil.Log("{0} {1}", curParseFilePath, rowStr);
                        CsvRow rowObj = ParseStr2Row(name, rowIndex, keys, types, rowStr, majorKeys, rowType, keyFlags);
                        rowsType.InvokeMember("Add", BindingFlags.Default | BindingFlags.InvokeMethod, null, rows, new object[]{rowObj});
                    }
                    catch (Exception e)
                    {
                        Debug.LogFormat("{0} {1}行有问题:{2}\n{3}", curParseFilePath, rowIndex, rowStr, e);
                        throw;
                    }
                }
            }
            DeleteNotUseKeys(keys);
        }
    }
    //不能包含有空列
    void CheckColumnNotEmpty()
    {
        for (int i = 0; i < keys.Count; i++)
        {
            DebugUtil.Assert(!string.IsNullOrEmpty(keys[i]), "{0} 表的第 {1} 列字段名不能为空！", name, i + 1);
            DebugUtil.Assert(keys[i].IndexOf(" ") == -1, "{0} 表的第 {1} 列字段名 {2} 带有空格！", name, i + 1, keys[i]);
        } 
    }
    bool IsEmptyLineOrCommentLine(string rowStr)
    {
        string[] values = rowStr.Split(SEPARATOR);
        if (values.Length > 0 && values[0] == "N")
            return true;
        for (int i = 0; i < values.Length; i++)
            if (values[i].Length > 0)
                return false;
        return true;
    }
    void GetMajorKeys(string[] keyFlags, List<string> keys, string[] types, out List<string> majorKeys, out List<string> majorTypes)
    {
        majorKeys = new List<string>();
        majorTypes = new List<string>();
        for (int i = 0; i < keyFlags.Length; i++)
        {
            if (keyFlags[i].IndexOf("M") >= 0)
            {
                majorKeys.Add(keys[i]);
                majorTypes.Add(types[i]);
            }
        }
        if (majorKeys.Count == 0 && keys.Count > 0)//容错，未设置则视为第一列是主键
        {
            majorKeys.Add(keys[0]);
            majorTypes.Add(types[0]);
        }
    }
    CsvRow ParseStr2Row(string fileName, int rowIndex, List<string> keys, string[] types, string rowStr, List<string> majorKeys, Type rowType, string[] flags)
    {
        string[] values = rowStr.Split(SEPARATOR);
        DebugUtil.Assert(values.Length == keys.Count,
            curParseFilePath + " 下面这行很可能是少了或多了一个列 {0} != {1} \n {2}", values.Length, keys.Count, rowStr);
        UpperValue(ref values, flags);
        CsvRow row = Activator.CreateInstance(rowType) as CsvRow;
        row.fileName = fileName;
        row.rowIndex = rowIndex;
        row.SetPropValues(rowType, keys, values, types, majorKeys);
        return row;
    }

    void UpperValue(ref string[] values, string[] flags)
    {
        for (int i = 0; i < values.Length; i++)
        {
            if (flags[i].Contains("U"))
            {
                values[i] = values[i].ToUpper();
            }
        }
    }
    void FilterNotUseColoum(string[] keyFlags, List<string> keys)
    {
        for (int i = 0; i < keyFlags.Length; i++)
        {
            string flag = keyFlags[i];
            if (flag == "R" || flag == "L" || flag.Contains("U")) continue;
            if ((!string.IsNullOrEmpty(flag) && flag != "M" && flag.IndexOf("C") == -1) ||
                flag == "N")
                keys[i] = string.Empty;//空串表示过滤掉的列
        }
    }
    void DeleteNotUseKeys(List<string> keys)
    {
        for (int i = keys.Count - 1; i >= 0; i--)
        {
            if (string.IsNullOrEmpty(keys[i]))
                keys.RemoveAt(i);
        }
    }
#endif
    */
}