﻿using System;
using System.Collections.Generic;

namespace XConfig
{
    public class ByteType : ConfigType
    {
        public override string Name => "byte";

        public override string DefaultValue => "0";

        public static byte ReadFromBytes(BytesBuffer buffer)
        {
            return buffer.ReadByte();
        }

        public override void WriteToBytes(BytesBuffer buffer, string content)
        {
            buffer.WriteByte(byte.Parse(content));
        }

        public override bool CheckConfigFormat(string content, out string error)
        {
            if (!byte.TryParse(content, out var value))
            {
                error = $"{Name}类型的值只能为字节，当前为 : {content}";
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