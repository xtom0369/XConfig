using System;
using System.Collections.Generic;

namespace XConfig
{
    public class UIntType : ConfigType<UInt32>
    {
        public override string configTypeName => "uint";

        public override string defaultValue => "0";

        public static void ReadFromBytes(BytesBuffer buffer, out uint value)
        {
            value = buffer.ReadUInt32();
        }

        public override void WriteToBytes(BytesBuffer buffer, string content)
        {
            buffer.WriteUInt32(uint.Parse(content));
        }
    }
}
