using System;
using System.Collections.Generic;

namespace XConfig
{
    public class LongType : ConfigType<Int64>
    {
        public override string configTypeName => "long";

        public override string defaultValue => "0";

        public static long ReadFromBytes(BytesBuffer buffer)
        {
            return buffer.ReadLong();
        }

        public static void ReadFromBytes(BytesBuffer buffer, out long value)
        {
            value = ReadFromBytes(buffer);
        }

        public override void WriteToBytes(BytesBuffer buffer, string content)
        {
            buffer.WriteLong(long.Parse(content));
        }

        public override bool CheckConfigFormat(string content, out string error)
        {
            if (!long.TryParse(content, out var value))
            {
                error = $"{configTypeName}类型的值只能为64位整数，当前为 : \"{content}\"";
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
