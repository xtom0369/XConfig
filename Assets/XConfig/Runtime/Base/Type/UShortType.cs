using System;
using System.Collections.Generic;

namespace XConfig
{
    public class UShortType : ConfigType<UInt16>
    {
        public override string configTypeName => "ushort";

        public override string defaultValue => "0";

        public static void ReadFromBytes(BytesBuffer buffer, out ushort value)
        {
            value = buffer.ReadUInt16();
        }

        public override void WriteToBytes(BytesBuffer buffer, string content)
        {
            buffer.WriteInt32(ushort.Parse(content));
        }
    }
}
