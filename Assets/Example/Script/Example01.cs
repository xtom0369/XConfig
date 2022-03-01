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

public class Example01 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Config.Inst = new Config();
        Config.Inst.Init();
    }

    public void Print() 
    {
        var rows = Config.Inst.example01Table.rows;
        foreach (var row in rows)
            Debug.Log(row);
    }
}
