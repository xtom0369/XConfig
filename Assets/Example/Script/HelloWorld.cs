using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

public class HelloWorld : MonoBehaviour 
{
    void Start()
    {
        // 配置表的初始化，只需要执行一次
        Config.Inst.Init();

        // 打印每一行的Field字段配置
        var rows = Config.Inst.helloWorldTable.rows;
        foreach (var row in rows)
            Debug.Log(row.Field);

        // 打印主键为1的Field字段配置
        Debug.Log(Config.Inst.helloWorldTable.GetRow(1).Field);
    }
}
