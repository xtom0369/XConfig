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

public class Example : MonoBehaviour
{
    void Start()
    {
        // 配置表的初始化，只需要执行一次
        Config.Inst.Init();
    }

    public void Print() 
    {
        var rows = Config.Inst.baseTypeTable.rows;
        foreach (var row in rows)
            Debug.Log(row);
    }
}
