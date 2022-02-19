using System;
using System.Collections.Generic;

namespace XConfig
{
    public class FloatType : ConfigType
    {
        public override string RawTypeName => "float";

        public override string DefaultValue => "0f";

        public override string ParseDefaultValue(string content)
        {
            return $"{content}f";
        }

        public static float ReadFromBytes(BytesBuffer buffer)
        {
            return buffer.ReadFloat();
        }

        public override void WriteToBytes(BytesBuffer buffer, string content)
        {
            buffer.WriteFloat(float.Parse(content));
        }

        public override bool CheckConfigFormat(string content, out string error)
        {
            if (!float.TryParse(content, out var value))
            {
                error = $"{RawTypeName}类型的值只能为浮点，当前为 : {content}";
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
