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
        var rows = Config.Inst.parentTable.rows;
        Debug.Log($"【GetAllParentRows】");
        foreach (var row in rows)
            Debug.Log(row);
    }

    public void GetAllChild1Rows()
    {
        var rows = Config.Inst.child1Table.rows;
        Debug.Log($"【GetAllChild1Rows】");
        foreach (var row in rows)
            Debug.Log(row);
    }

    public void GetAllChild2Rows()
    {
        var rows = Config.Inst.child2Table.rows;
        Debug.Log($"【GetAllChild2Rows】");
        foreach (var row in rows)
            Debug.Log(row);
    }
}
