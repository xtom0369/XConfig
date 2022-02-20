using System;
using System.Collections.Generic;

namespace XConfig
{
    public class IntType : ConfigType
    {
        public override string RawTypeName => "int";

        public override string AliasRawTypeName => nameof(Int32);

        public override string DefaultValue => "0";

        public static int ReadFromBytes(BytesBuffer buffer)
        {
            return buffer.ReadInt32();
        }

        public static void ReadFromBytes(BytesBuffer buffer, out int value)
        {
            value = ReadFromBytes(buffer);
        }

        public override void WriteToBytes(BytesBuffer buffer, string content)
        {
            buffer.WriteInt32(int.Parse(content));
        }

        public override bool CheckConfigFormat(string content, out string error)
        {
            if (!int.TryParse(content, out var value))
            {
                error = $"{RawTypeName}类型的值只能为整数，当前为 : {content}";
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
