using System;
using System.Collections.Generic;

namespace XConfig
{
    public class ULongType : ConfigType<UInt64>
    {
        public override string configTypeName => "ulong";

        public override string defaultValue => "0";

        public static void ReadFromBytes(BytesBuffer buffer, out ulong value)
        {
            value = buffer.ReadULong();
        }

        public override void WriteToBytes(BytesBuffer buffer, string content)
        {
            buffer.WriteULong(ulong.Parse(content));
        }
    }
}
