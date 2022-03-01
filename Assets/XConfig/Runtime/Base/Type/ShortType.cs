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
            buffer.WriteInt32(short.Parse(content));
        }

        public override bool CheckConfigFormat(string content, out string error)
        {
            if (!short.TryParse(content, out var value))
            {
                error = $"{configTypeName}类型的值只能为16位整数，当前为 : \"{content}\"";
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
