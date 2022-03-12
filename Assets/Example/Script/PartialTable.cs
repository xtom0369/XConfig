using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartialTable : MonoBehaviour 
{
    void Start()
    {
        // 配置表的初始化，只需要执行一次
        Config.Inst.Init();
    }

    public void IsPathUsed() 
    {
        Debug.Log($"【IsPathUsed】");
        string path = GameObject.Find("Canvas/InputField").GetComponent<InputField>().text;
        Debug.Log($"Is path {path} exists? 【{Config.Inst.partialTableTable.IsPathUsed(path)}】");
    }
}
