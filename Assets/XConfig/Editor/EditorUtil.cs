using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.Reflection;

public class EditorUtil
{
    static public string GetBufferReadStr(string type, string flag = null)
    {
        if (type.StartsWith("Enum"))//枚举类型,当做int16处理
            return string.Format("({0})buffer.{1}()", type, "ReadInt16");//形如：(EnumResultType)buffer.ReadInt32()
        else
        {
            string getFuncStr = "buffer.";
            if (!string.IsNullOrEmpty(flag) && flag.Contains("R"))//如果是引用类型，则type为string
                type = "string";
            switch (type)
            {
                case "bool":
                    getFuncStr += "ReadBool";
                    break;
                case "byte":
                    getFuncStr += "ReadByte";
                    break;
                case "short":
                    getFuncStr += "ReadInt16";
                    break;
                case "ushort":
                    getFuncStr += "ReadUInt16";
                    break;
                case "int":
                    getFuncStr += "ReadInt32";
                    break;
                case "uint":
                    getFuncStr += "ReadUInt32";
                    break;
                case "long":
                    getFuncStr += "ReadLong";
                    break;
                case "float":
                    getFuncStr += "ReadFloat";
                    break;
                case "FixFloat":
                    getFuncStr += "ReadFixFloat";
                    break;
                case "string":
                case "DateTime":
                case "date":
                    getFuncStr += "ReadString";
                    break;
                case "Vector2":
                    getFuncStr += "ReadVector2";
                    break;
                case "Vector3":
                    getFuncStr += "ReadVector3";
                    break;
                case "FixVector2":
                    getFuncStr += "ReadFixVector2";
                    break;
                case "FixVector3":
                    getFuncStr += "ReadFixVector3";
                    break;
                case "Vector4":
                    getFuncStr += "ReadVector4";
                    break;
                case "Color":
                    getFuncStr += "ReadColor";
                    break;
                case "List<bool>":
                    getFuncStr += "ReadBoolList";
                    break;
                case "List<int>":
                    getFuncStr += "ReadIntList";
                    break;
                case "IReadOnlyList<int>":
                    getFuncStr += "ReadIReadOnlyIntList";
                    break;
                case "List<uint>":
                    getFuncStr += "ReadUIntList";
                    break;
                case "List<long>":
                    getFuncStr += "ReadLongList";
                    break;
                case "List<string>":
                    getFuncStr += "ReadStringList";
                    break;
                default:
                    Type t = AssemblyUtil.GetType(type);
                    if (t != null && t.IsEnum)
                    {
                        DebugUtil.Assert(false, "枚举{0}命名不规范, 请详读doc\\coder\\规范文档\\客户端代码编程规范.txt", type);
                    }

                    DebugUtil.Assert(false, "不支持的数据类型：" + type);
                    break;
            }
            return getFuncStr + "()";//返回值形如：buffer.ReadInt32()
        }
    }
    static public string GetBufferWriteStr(string name, string type)
    {
        if (type.StartsWith("Enum"))//枚举类型,当做int16处理
            return string.Format("buffer.WriteInt16((short){1});", type, name);//形如：buffer.WriteInt16((EnumResultType)name);
        else
        {
            string funcStr = "";
            switch (type)
            {
                case "bool":
                    funcStr = "WriteBool";
                    break;
                case "byte":
                    funcStr = "WriteByte";
                    break;
                case "short":
                    funcStr = "WriteInt16";
                    break;
                case "ushort":
                    funcStr = "WriteUInt16";
                    break;
                case "int":
                    funcStr = "WriteInt32";
                    break;
                case "uint":
                    funcStr = "WriteUInt32";
                    break;
                case "long":
                    funcStr = "WriteLong";
                    break;
                case "float":
                    funcStr = "WriteFloat";
                    break;
                case "FixFloat":
                    funcStr = "WriteFixFloat";
                    break;
                case "string":
                case "DateTime":
                case "date":
                    funcStr = "WriteString";
                    break;
                case "Vector2":
                    funcStr = "WriteVector2";
                    break;
                case "Vector3":
                    funcStr = "WriteVector3";
                    break;
                case "FixVector2":
                    funcStr = "WriteFixVector2";
                    break;
                case "FixVector3":
                    funcStr = "WriteFixVector3";
                    break;
                case "Vector4":
                    funcStr = "WriteVector4";
                    break;
                case "Color":
                    funcStr = "WriteColor";
                    break;
                case "List<bool>":
                    funcStr = "WriteBoolList";
                    break;
                case "IReadOnlyList<int>":
                    funcStr = "WriteIReadOnlyIntList";
                    break;
                case "List<int>":
                    funcStr = "WriteIntList";
                    break;
                case "List<uint>":
                    funcStr = "WriteUIntList";
                    break;
                case "List<long>":
                    funcStr = "WriteLongList";
                    break;
                case "List<string>":
                    funcStr = "WriteStringList";
                    break;
                default:
                    DebugUtil.Assert(false, "不支持的数据类型：" + type);
                    break;
            }
            return string.Format("buffer.{0}({1});", funcStr, name);//返回值形如：buffer.WriteInt32(name);
        }
    }

    /// <summary>
    /// 转换到大写加下划线形式的字串
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    static public string GetUpperCaseName(string name)
    {
        StringBuilder sb = new StringBuilder();

        bool lastIsUpper = false;

        for (int i = 0; i != name.Length; i++)
        {
            char c = name[i];

            if (c >= 'A' && c <= 'Z')
            {
                if (!lastIsUpper)
                {
                    sb.Append("_");
                }

                sb.Append(c.ToString());
                lastIsUpper = true;
            }
            else
            {
                sb.Append(c.ToString().ToUpper());
                lastIsUpper = false;
            }
        }

        string ret = sb.ToString();

        if (ret.Length > 0)
        {
            if (ret.Substring(0, 1) == "_")
                ret = ret.Substring(1, ret.Length - 1);
        }

        return ret;
    }

    static public string GetLowerCaseName(string name)
    {
        StringBuilder sb = new StringBuilder();

        bool lastIsUpper = false;

        for (int i = 0; i != name.Length; i++)
        {
            char c = name[i];

            if (c >= 'A' && c <= 'Z')
            {
                if (!lastIsUpper)
                {
                    sb.Append("_");
                }

                sb.Append(c.ToString().ToLower());
                lastIsUpper = true;
            }
            else
            {
                sb.Append(c.ToString());
                lastIsUpper = false;
            }
        }

        string ret = sb.ToString();

        if (ret.Length > 0)
        {
            if (ret.Substring(0, 1) == "_")
                ret = ret.Substring(1, ret.Length - 1);
        }

        return ret;
    }

    static MethodInfo TagManager_GetDefinedLayers;
    public static void GetDefinedLayers(object[] array)
    {
        if (TagManager_GetDefinedLayers == null)
        {
            var type = typeof(Editor).Assembly.GetType("UnityEditor.TagManager");
            TagManager_GetDefinedLayers = type.GetMethod("GetDefinedLayers", BindingFlags.Static | BindingFlags.NonPublic);
        }
        TagManager_GetDefinedLayers.Invoke(null, array);
    }

    public static void ChangeListCount<T>(List<T> list, T defaultValue, int count)
    {
        if (list.Count != count)
        {
            if (count < list.Count)
            {
                for (int i = list.Count - 1; i >= count && i >= 0; i--)
                {
                    list.RemoveAt(i);
                }
            }
            else if (count > list.Count)
            {
                for (int i = list.Count; i < count; i++)
                {
                    list.Add(defaultValue);
                }
            }
        }
    }

    public static void RemoveListIndexs<T>(List<T> list, List<int> indexs)
    {
        if (indexs.Count > 0)
        {
            List<T> list2 = new List<T>();
            for (int i = 0; i < list.Count; i++)
            {
                if (indexs.Contains(i))
                    continue;
                list2.Add(list[i]);
            }
            list.Clear();
            list.AddRange(list2);
        }
    }
}
