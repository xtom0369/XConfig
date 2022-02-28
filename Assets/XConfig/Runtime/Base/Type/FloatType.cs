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

        public static float ReadFromBytes(BytesBuffer buffer)
        {
            return buffer.ReadFloat();
        }

        public static void ReadFromBytes(BytesBuffer buffer, out float value)
        {
            value = ReadFromBytes(buffer);
        }

        public override void WriteToBytes(BytesBuffer buffer, string content)
        {
            buffer.WriteFloat(float.Parse(content));
        }

        public override bool CheckConfigFormat(string content, out string error)
        {
            if (!float.TryParse(content, out var value))
            {
                error = $"{configTypeName}类型的值只能为浮点，当前为 : {content}";
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
