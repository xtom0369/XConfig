using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XConfig
{
    public class BoolType : ConfigTypeBase<bool>
    {
        public override string Name => "bool";

        public override bool DefaultValue => false;

        public static bool ReadFromBytes(BytesBuffer buffer)
        {
            return buffer.ReadBool();
        }

        public override void WriteToBytes(BytesBuffer buffer, string value)
        {
            buffer.WriteBool(value == "1");
        }

        public override bool CheckConfigFormat(string content, out string errorMsg)
        {
            if (content != "0" && content != "1")
            { 
                errorMsg = $"{Name}类型输入只能是0或者1";
                return false;
            }
            else
            {
                errorMsg = string.Empty;
                return true;
            }
        }
    }
}
