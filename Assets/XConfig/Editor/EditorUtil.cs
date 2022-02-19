using System;
using XConfig;

namespace XConfig.Editor 
{
    public class EditorUtil
    {
        static public string GetBufferReadStr(string type, Flag flag)
        {
            string getFuncStr = "buffer.";
            switch (type)
            {
                case "List<bool>":
                    getFuncStr += "ReadBoolList";
                    break;
                case "List<int>":
                    getFuncStr += "ReadIntList";
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

                    DebugUtil.Assert(false, "不支持的数据类型：" + type);
                    break;
            }
            return getFuncStr + "()";//返回值形如：buffer.ReadInt32()
        }
    }
}
