using System;
using System.Collections.Generic;

namespace XConfig
{
    public class UShortType : ConfigType
    {
        public override string RawTypeName => "ushort";

        public override string DefaultValue => "0";

        public static ushort ReadFromBytes(BytesBuffer buffer)
        {
            return buffer.ReadUInt16();
        }

        public override void WriteToBytes(BytesBuffer buffer, string content)
        {
            buffer.WriteInt32(ushort.Parse(content));
        }

        public override bool CheckConfigFormat(string content, out string error)
        {
            if (!ushort.TryParse(content, out var value))
            {
                error = $"{RawTypeName}类型的值只能为16位正整数，当前为 : {content}";
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
