using System;
using System.Collections.Generic;

namespace XConfig
{
    public class LongType : ConfigType<Int64>
    {
        public override string configTypeName => "long";

        public override string defaultValue => "0";

        public static void ReadFromBytes(BytesBuffer buffer, out long value)
        {
            value = buffer.ReadLong();
        }

        public override void WriteToBytes(BytesBuffer buffer, string content)
        {
            buffer.WriteLong(long.Parse(content));
        }
    }
}
