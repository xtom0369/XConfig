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
        Config.Inst = new Config();
        Config.Inst.Init();
    }

    public void Print() 
    {
        var rows = Config.Inst.baseTypeTable.rows;
        foreach (var row in rows)
            Debug.Log(row);
    }
}
