using System;
using System.Collections.Generic;

namespace XConfig
{
    public class ULongType : ConfigType
    {
        public override string RawTypeName => "ulong";

        public override string DefaultValue => "0";

        public static ulong ReadFromBytes(BytesBuffer buffer)
        {
            return buffer.ReadULong();
        }

        public override void WriteToBytes(BytesBuffer buffer, string content)
        {
            buffer.WriteULong(ulong.Parse(content));
        }

        public override bool CheckConfigFormat(string content, out string error)
        {
            if (!ulong.TryParse(content, out var value))
            {
                error = $"{RawTypeName}类型的值只能为64位正整数，当前为 : {content}";
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
