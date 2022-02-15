using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using XConfig;

public class EditorUtil
{
    static public string GetBufferReadStr(string type, Flag flag)
    {
        if (type.StartsWith("Enum"))//枚举类型,当做int16处理
            return string.Format("({0})buffer.{1}()", type, "ReadInt16");//形如：(EnumResultType)buffer.ReadInt32()
        else
        {
            string getFuncStr = "buffer.";
            if (flag.IsReference)//如果是引用类型，则type为string
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
}
