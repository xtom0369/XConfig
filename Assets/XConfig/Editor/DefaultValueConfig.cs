using System;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using XConfig;

[System.Serializable]
public class DefaultValueConfig : ScriptableObject
{

    protected static DefaultValueConfig Instance;
    public static DefaultValueConfig Config
    {
        get
        {
            if (Instance != null) return Instance;

            var guids = AssetDatabase.FindAssets("t:DefaultValueConfig");
            if (guids.Length > 0)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                Instance = AssetDatabase.LoadAssetAtPath<DefaultValueConfig>(path);

                return Instance;
            }

            return null;
        }
    }

    public DefaultValuePair[] pairs;

    public string GetDefaultValue(string fileName, string type, string fieldName, string flag)
    {
        foreach (var pair in pairs)
        {
            if (type.StartsWith(pair.type))
                return ParseDefaultValue(fileName, type, fieldName, flag, pair.value);
        }
        return null;
    }

    public static string ParseDefaultValue(string fileName, string type, string filedName, string flag, string defaultVaule)
    {
        Type t = AssemblyUtil.GetType(type);
        if (type.Trim().ToLower() == "bool")
        {
            defaultVaule = defaultVaule.Trim().ToLower();
            return defaultVaule == "1" || defaultVaule == "true" ? "true" : "false";
        }
        else if (type == "Vector2")
        {
            Vector3 v = XRow.ParseVector2(defaultVaule);
            return string.Format("new Vector2({0}f, {1}f)", v.x, v.y);
        }
        else if (type == "Vector3")
        {
            Vector3 v = XRow.ParseVector3(defaultVaule);
            return string.Format("new Vector3({0}f, {1}f, {2}f)", v.x, v.y, v.z);
        }
        else if (type == "Vector4")
        {
            Vector4 v = XRow.ParseVector4(defaultVaule);
            return string.Format("new Vector4({0}f, {1}f, {2}f, {3}f)", v.x, v.y, v.z, v.w);
        }
        else if (type == "Color")
        {
            Color c = XRow.ParseColor(defaultVaule);
            return string.Format("new Color({0}f, {1}f, {2}f, {3}f)", c.r, c.g, c.b, c.a);
        }
        else if (type == "string" && defaultVaule != "null" && !defaultVaule.StartsWith("\"") && !defaultVaule.EndsWith("\""))
        {
            return "\"" + defaultVaule + "\"";
        }
        else if (type == "int")
        {
            int resultInt;
            DebugUtil.Assert(int.TryParse(defaultVaule, out resultInt), "{0}表{1}字段默认值格式不对:{2}", fileName, filedName, defaultVaule);
            return defaultVaule;
        }
        else if (type == "float")//浮点类型默认值需要加个'f'后缀
        {
            float resultFloat;
            DebugUtil.Assert(float.TryParse(defaultVaule, out resultFloat), "{0}表{1}字段默认值格式不对:{2}", fileName, filedName, defaultVaule);
            if (defaultVaule != null && defaultVaule != "0")
                return defaultVaule + "f";
            else
                return defaultVaule;
        }
        else if (type.StartsWith("List<"))//列表默认值为空列表，不为null，不然做检验的时候不好处理
        {
            string resultSt = "new " + type.ToString() + "()";
            if (!string.IsNullOrEmpty(defaultVaule) && defaultVaule != "null")
            {
                resultSt += "{";
                string[] items = defaultVaule.Split('|');
                for (int i = 0; i < items.Length; i++)
                {
                    if (i > 0)
                        resultSt += ",";
                    resultSt += items[i];
                }
                resultSt += "}";
            }
            return resultSt;
        }
        if (flag.Contains("R")) return "\"" + defaultVaule + "\"";//引用类型，缺省值都为字串
        if (t == null || !t.IsEnum || !EnumIsDefined(t, defaultVaule)) return defaultVaule;
        try
        {
            return type + "." + Enum.Parse(t, defaultVaule);
        }
        catch
        {
            DebugUtil.LogError("不能解析的枚举类{0}，请检查拼写或添加定义", type);
            return "0";
        }
    }

    public static bool EnumIsDefined(Type t, string v)
    {
        int i;
        if (int.TryParse(v, out i))
            return Enum.IsDefined(t, i);
        return Enum.IsDefined(t, v);
    }
}

[System.Serializable]
public struct DefaultValuePair
{
    public string type;
    public string value;
}
