using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

public enum EnumExample01 
{ 
    Value0,
    Value1,
    Value2,
    Value3,
}

public class BaseType : MonoBehaviour 
{
    void Start()
    {
        // 配置表的初始化，只需要执行一次
        Config.Inst.Init();
    }

    public void GetAllRows() 
    {
        var rows = Config.Inst.baseTypeTable.rows;
        Debug.Log($"【GetAllRows】");
        foreach (var row in rows)
            Debug.Log(row);
    }

    public void GetRow()
    {
        int key = 1;
        var row = Config.Inst.baseTypeTable.GetRow(key);
        Debug.Log($"【GetRow】key = {key}, row = {row}");

        try
        {
            key = 3;
            row = Config.Inst.baseTypeTable.GetRow(key);
            Debug.Log($"【GetRow】key = {key}, row = {row}");
        }
        catch (Exception e) 
        {
            Debug.LogError(e.ToString());
        }
    }

    public void TryGetRow()
    {
        int key = 1;
        bool result = Config.Inst.baseTypeTable.TryGetRow(key, out var row);
        Debug.Log($"【TryGetRow】key = {key}, result = {result}, row = {row}");

        key = 3;
        result = Config.Inst.baseTypeTable.TryGetRow(key, out row);
        Debug.Log($"【TryGetRow】key = {key}, result = {result}, row = {row}");
    }

    public void ContainsKey()
    {
        int key = 1;
        bool result = Config.Inst.baseTypeTable.ContainsKey(key);
        Debug.Log($"【ContainsKey】key = {key}, result = {result}");

        key = 3;
        result = Config.Inst.baseTypeTable.ContainsKey(key);
        Debug.Log($"【ContainsKey】key = {key}, result = {result}");
    }
}
