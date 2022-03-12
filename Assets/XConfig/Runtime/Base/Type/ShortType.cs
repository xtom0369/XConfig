using System;
using System.Collections.Generic;

namespace XConfig
{
    public class ShortType : ConfigType<Int16>
    {
        public override string configTypeName => "short";

        public override string defaultValue => "0";

        public static short ReadFromBytes(BytesBuffer buffer)
        {
            return buffer.ReadInt16();
        }

        public static void ReadFromBytes(BytesBuffer buffer, out short value)
        {
            value = ReadFromBytes(buffer);
        }

        public override void WriteToBytes(BytesBuffer buffer, string content)
        {
            buffer.WriteInt16(short.Parse(content));
        }
    }
}
