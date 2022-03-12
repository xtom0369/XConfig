using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomConfigType : MonoBehaviour 
{
    void Start()
    {
        // 配置表的初始化，只需要执行一次
        Config.Inst.Init();
    }

    public void GetItemName() 
    {
        Debug.Log($"【GetItemName】");

        var rows = Config.Inst.customConfigTypeTable.rows;
        foreach (var row in rows)
            Debug.Log(row.Item.config.Name);
    }
}
