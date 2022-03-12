using System;
using System.Collections.Generic;

namespace XConfig
{
    public class FloatType : ConfigType<float>
    {
        public override string configTypeName => "float";

        public override string defaultValue => "0f";

        public override string ParseDefaultValue(string content)
        {
            return $"{content}f";
        }

        public static void ReadFromBytes(BytesBuffer buffer, out float value)
        {
            value = buffer.ReadFloat();
        }

        public override void WriteToBytes(BytesBuffer buffer, string content)
        {
            buffer.WriteFloat(float.Parse(content));
        }
    }
}
