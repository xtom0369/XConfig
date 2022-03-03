using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

public class ParentChildren : MonoBehaviour 
{
    void Start()
    {
        // 配置表的初始化，只需要执行一次
        Config.Inst.Init();
    }

    public void GetAllParentRows() 
    {
        var rows = Config.Inst.doubleKeyTable.rows;
        Debug.Log($"【GetAllRows】");
        foreach (var row in rows)
            Debug.Log(row);
    }

    public void GetAllChild1Rows()
    {
        var rows = Config.Inst.doubleKeyTable.rows;
        Debug.Log($"【GetAllRows】");
        foreach (var row in rows)
            Debug.Log(row);
    }

    public void GetAllChild2Rows()
    {
        var rows = Config.Inst.doubleKeyTable.rows;
        Debug.Log($"【GetAllRows】");
        foreach (var row in rows)
            Debug.Log(row);
    }

    public void GetRows()
    {
        int key1 = 1;
        var rows = Config.Inst.doubleKeyTable.GetRows(key1);
        Debug.Log($"【GetRows】key1 = {key1}");
        foreach (var row in rows)
            Debug.Log(row);
    }

    public void GetRow()
    {
        int key1 = 1;
        int key2 = 1;
        var row = Config.Inst.doubleKeyTable.GetRow(key1, key2);
        Debug.Log($"【GetRow】key1 = {key1}, key2 = {key2}, row = {row}");

        key1 = 1;
        key2 = 3;
        row = Config.Inst.doubleKeyTable.GetRow(key1, key2);
        Debug.Log($"【GetRow】key1 = {key1}, key2 = {key2}, row = {row}");

        try
        {
            key1 = 1;
            key2 = 11;
            row = Config.Inst.doubleKeyTable.GetRow(key1, key2);
            Debug.Log(row);
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }

        try
        {
            key1 = 3;
            key2 = 4;
            row = Config.Inst.doubleKeyTable.GetRow(key1, key2);
            Debug.Log(row);
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    public void TryGetRow()
    {
        int key1 = 1;
        int key2 = 1;
        bool result = Config.Inst.doubleKeyTable.TryGetRows(key1, out var rows);
        Debug.Log($"【TryGetValue】key1 = {key1}, result = {result}");
        foreach (var item in rows)
            Debug.Log(item);

        key1 = 1;
        key2 = 11;
        result = Config.Inst.doubleKeyTable.TryGetRow(key1, key2, out var row);
        Debug.Log($"【TryGetValue】key1 = {key1}, key2 = {key2}, result = {result}");
        Debug.Log(row);
    }

    public void ContainsKey()
    {
        int key1 = 1;
        int key2 = 2;
        bool result = Config.Inst.doubleKeyTable.ContainsKey(key1);
        Debug.Log($"【ContainsKey】key1 = {key1}, result = {result}");

        key1 = 3;
        result = Config.Inst.doubleKeyTable.ContainsKey(key1, key2);
        Debug.Log($"【ContainsKey】key1 = {key1}, key2 = {key2}, result = {result}");
    }
}
