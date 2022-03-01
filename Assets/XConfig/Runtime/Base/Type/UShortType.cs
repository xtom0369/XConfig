using System;
using System.Collections.Generic;

namespace XConfig
{
    public class UShortType : ConfigType<UInt16>
    {
        public override string configTypeName => "ushort";

        public override string defaultValue => "0";

        public static ushort ReadFromBytes(BytesBuffer buffer)
        {
            return buffer.ReadUInt16();
        }

        public static void ReadFromBytes(BytesBuffer buffer, out ushort value)
        {
            value = ReadFromBytes(buffer);
        }

        public override void WriteToBytes(BytesBuffer buffer, string content)
        {
            buffer.WriteInt32(ushort.Parse(content));
        }

        public override bool CheckConfigFormat(string content, out string error)
        {
            if (!ushort.TryParse(content, out var value))
            {
                error = $"{configTypeName}类型的值只能为16位正整数，当前为 : \"{content}\"";
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
