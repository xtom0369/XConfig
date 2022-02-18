using System;
using System.Collections.Generic;

namespace XConfig
{
    public class UIntType : ConfigType
    {
        public override string Name => "uint";

        public override string DefaultValue => "0";

        public static uint ReadFromBytes(BytesBuffer buffer)
        {
            return buffer.ReadUInt32();
        }

        public override void WriteToBytes(BytesBuffer buffer, string content)
        {
            buffer.WriteUInt32(uint.Parse(content));
        }

        public override bool CheckConfigFormat(string content, out string error)
        {
            if (!uint.TryParse(content, out var value))
            {
                error = $"{Name}类型的值只能为正整数，当前为 : {content}";
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
