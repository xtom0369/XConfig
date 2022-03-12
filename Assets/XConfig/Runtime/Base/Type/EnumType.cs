using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XConfig
{
    public abstract class EnumType : ConfigType
    {
        public override string readByteClassName => nameof(EnumType);

        public override string writeByteTypeName => "short";

        public static void ReadFromBytes(BytesBuffer buffer, out short value)
        {
            value = buffer.ReadInt16();
        }

        public override void WriteToBytes(BytesBuffer buffer, string content)
        {
            buffer.WriteInt16(short.Parse(content));
        }
    }

    public class EnumType<T> : EnumType where T : Enum
    {
        public override string typeName => typeof(T).Name;

        public override string defaultValue => ParseDefaultValue("0");

        public override string ParseDefaultValue(string content)
        {
            return  $"{configTypeName}.{Enum.Parse(typeof(T), content)}";
        }

        public override void CheckConfigFormat(string content)
        {
            AssertParamType(content, typeof(short));

            short value = short.Parse(content);
            DebugUtil.Assert(Enum.IsDefined(typeof(T), (int)value), $"{configTypeName} 枚举类型的值不存在 : {content}");
        }
    }
}
