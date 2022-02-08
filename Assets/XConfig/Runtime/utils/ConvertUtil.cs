using UnityEngine;
using System.Collections;
using System.Text;
using System;
using System.Collections.Generic;

public class ConvertUtil
{
    /// <summary>
    /// 转换成首字母小写
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    static public string ToFirstCharlower(string name)
    {
        return char.ToLower(name[0]) + name.Substring(1);
    }
    /// <summary>
    /// 转换成驼峰命名
    /// </summary>
    /// <param name="oldName"></param>
    /// <returns></returns>
    static public string Convert2HumpNamed(string oldName)
    {
        string[] subNames = oldName.Split('_');
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < subNames.Length; i++)
        {
            sb.Append(Char.ToUpper(subNames[i][0]));
            sb.Append(subNames[i].Substring(1));
        }
        return sb.ToString();
    }
    /// <summary>
    /// 驼峰命名法转换为下划线命名法
    /// </summary>
    static public string CamelToUnderscore(string oldName)
    {
        string newName;
        newName = oldName.Substring(0, 1).ToLower();
        for (int i = 1; i < oldName.Length; i++)
        {
            char ch = oldName.Substring(i, 1).ToCharArray()[0];
            if (ch >= 'A' && ch <= 'Z')
                newName += "_" + oldName.Substring(i, 1).ToLower();
            else
                newName += oldName.Substring(i, 1);
        }
        return newName;
    }
}
