using System;
using System.Collections.Generic;

namespace XConfig
{
    public class ULongType : ConfigType<UInt64>
    {
        public override string configTypeName => "ulong";

        public override string defaultValue => "0";

        public static ulong ReadFromBytes(BytesBuffer buffer)
        {
            return buffer.ReadULong();
        }

        public static void ReadFromBytes(BytesBuffer buffer, out ulong value)
        {
            value = ReadFromBytes(buffer);
        }

        public override void WriteToBytes(BytesBuffer buffer, string content)
        {
            buffer.WriteULong(ulong.Parse(content));
        }

        public override bool CheckConfigFormat(string content, out string error)
        {
            if (!ulong.TryParse(content, out var value))
            {
                error = $"{configTypeName}类型的值只能为64位正整数，当前为 : \"{content}\"";
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
