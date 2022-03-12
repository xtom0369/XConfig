using System;
using System.Collections.Generic;

namespace XConfig
{
    public class ByteType : ConfigType<byte>
    {
        public override string configTypeName => "byte";

        public override string defaultValue => "0";

        public static void ReadFromBytes(BytesBuffer buffer, out byte value)
        {
            value = buffer.ReadByte();
        }

        public override void WriteToBytes(BytesBuffer buffer, string content)
        {
            buffer.WriteByte(byte.Parse(content));
        }
    }
}
