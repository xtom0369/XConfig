using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XConfig
{
    public abstract class EnumType : ConfigType
    {
        public override string DefaultValue => "0";

        public static short ReadFromBytes(BytesBuffer buffer)
        {
            return buffer.ReadInt16();
        }

        public override void WriteToBytes(BytesBuffer buffer, string content)
        {
            buffer.WriteInt16(short.Parse(content));
        }
    }

    public class EnumType<T> : EnumType where T : Enum
    {
        public override string Name => typeof(T).Name;

        public override string ParseDefaultValueContent(string content)
        {
            content = base.ParseDefaultValueContent(content);
            return  $"{Name}.{Enum.Parse(typeof(T), content)}";
        }

        public override bool CheckConfigFormat(string content, out string error)
        {
            if (!short.TryParse(content, out var value))
            {
                error = $"{Name}类型的值只能是整型，当前为 : {content}";
                return false;
            }

            if (!Enum.IsDefined(typeof(T), (int)value))
            {
                error = $"{Name}枚举类型的值不存在 : {content}";
                return false;
            }

            error = string.Empty;
            return true;
        }
    }
}
