using System;
using System.Collections.Generic;

namespace XConfig
{
    public class StringType : ConfigType<String>
    {
        public override string configTypeName => "string";

        public override string defaultValue => "string.Empty";

        public override string ParseDefaultValue(string content)
        {
            if (!content.StartsWith("\""))
                content = "\"" + content;

            if (!content.EndsWith("\""))
                content = content + "\"";

            return content;
        }

        public static void ReadFromBytes(BytesBuffer buffer, out string value)
        {
            value = buffer.ReadString();
        }

        public override void WriteToBytes(BytesBuffer buffer, string content)
        {
            buffer.WriteString(content.Replace("\\n", "\n")); // 解决表中含有一个换行符，但读取到代码中会出现两个的问题
        }
    }
}
