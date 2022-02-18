using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XConfig
{
    public class BoolType : ConfigType
    {
        public override string Name => "bool";

        public override string DefaultValue => "0";

        public static bool ReadFromBytes(BytesBuffer buffer)
        {
            return buffer.ReadBool();
        }

        public override void WriteToBytes(BytesBuffer buffer, string content)
        {
            buffer.WriteBool(content == "1");
        }

        public override string ParseDefaultValue(string content)
        {
            content = base.ParseDefaultValue(content);
            return content == "1" ? "true" : "false";
        }

        public override bool CheckConfigFormat(string content, out string error)
        {
            if (content != "0" && content != "1")
            { 
                error = $"{Name}类型的值只能是0或者1，当前为 : {content}";
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
