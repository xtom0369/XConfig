using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XConfig
{
    public class BoolType : ConfigType<Boolean>
    {
        public override string configTypeName => "bool";

        public override string defaultValue => "false";

        public static void ReadFromBytes(BytesBuffer buffer, out bool value)
        {
            value = buffer.ReadBool();
        }

        public override void WriteToBytes(BytesBuffer buffer, string content)
        {
            buffer.WriteBool(content == "1");
        }

        public override string ParseDefaultValue(string content)
        {
            return content == "1" ? "true" : "false";
        }

        public override bool CheckConfigFormat(string content, out string error)
        {
            if (content != "0" && content != "1")
            { 
                error = $"{configTypeName}类型的值只能是0或者1，当前为 : \"{content}\"";
                return false;
            }
            else
            {
                error = string.Empty;
                return true;
            }
        }
    }
}
