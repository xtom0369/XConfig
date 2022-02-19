using System;
using System.Collections.Generic;

namespace XConfig
{
    public class LongType : ConfigType
    {
        public override string RawTypeName => "long";

        public override string DefaultValue => "0";

        public static long ReadFromBytes(BytesBuffer buffer)
        {
            return buffer.ReadLong();
        }

        public override void WriteToBytes(BytesBuffer buffer, string content)
        {
            buffer.WriteLong(long.Parse(content));
        }

        public override bool CheckConfigFormat(string content, out string error)
        {
            if (!long.TryParse(content, out var value))
            {
                error = $"{RawTypeName}类型的值只能为64位整数，当前为 : {content}";
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
