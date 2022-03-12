using System;
using System.Collections.Generic;

namespace XConfig
{
    public class IntType : ConfigType<Int32>
    {
        public override string configTypeName => "int";

        public override string defaultValue => "0";

        public static void ReadFromBytes(BytesBuffer buffer, out int value)
        {
            value = buffer.ReadInt32();
        }

        public override void WriteToBytes(BytesBuffer buffer, string content)
        {
            buffer.WriteInt32(int.Parse(content));
        }
    }
}
