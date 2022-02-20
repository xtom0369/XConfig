using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using XConfig;

public class Example01 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Type type = typeof(bool);
        TypeInfo typeInfo = type.GetTypeInfo();
        var info = type.Name;
        int a = 0;

        //Config.Inst = new Config();
        //Config.Inst.Init();

        //var rows = Config.Inst.itemTypeTable.rows;
        //foreach (var row in rows)
        //{
        //    //DebugUtil.LogError(row.t1);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
