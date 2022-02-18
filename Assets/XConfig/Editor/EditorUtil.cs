using System;
using XConfig;

namespace XConfig.Editor 
{
    public class EditorUtil
    {
        static public string GetBufferReadStr(string type, Flag flag)
        {
            string getFuncStr = "buffer.";
            if (flag.IsReference)//如果是引用类型，则type为string
                type = "string";
            switch (type)
            {
                case "short":
                    getFuncStr += "ReadInt16";
                    break;
                case "ushort":
                    getFuncStr += "ReadUInt16";
                    break;
                case "long":
                    getFuncStr += "ReadLong";
                    break;
                case "string":
                    getFuncStr += "ReadString";
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
