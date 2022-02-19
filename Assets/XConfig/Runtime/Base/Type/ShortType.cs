using System;
using System.Collections.Generic;

namespace XConfig
{
    public class ShortType : ConfigType
    {
        public override string RawTypeName => "short";

        public override string DefaultValue => "0";

        public static int ReadFromBytes(BytesBuffer buffer)
        {
            return buffer.ReadInt16();
        }

        public override void WriteToBytes(BytesBuffer buffer, string content)
        {
            buffer.WriteInt32(short.Parse(content));
        }

        public override bool CheckConfigFormat(string content, out string error)
        {
            if (!short.TryParse(content, out var value))
            {
                error = $"{RawTypeName}类型的值只能为16位整数，当前为 : {content}";
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
