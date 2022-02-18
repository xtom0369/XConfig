using System;
using System.Collections.Generic;

namespace XConfig
{
    public class StringType : ConfigType
    {
        public override string Name => "string";

        public override string DefaultValue => "string.Empty";

        public override string ParseDefaultValueContent(string content)
        {
            content = base.ParseDefaultValueContent(content);
            if (content != DefaultValue)
            {
                if (!content.StartsWith("\""))
                    content = "\"" + content;

                if (!content.EndsWith("\""))
                    content = content + "\"";
            }

            return content;
        }

        public static string ReadFromBytes(BytesBuffer buffer)
        {
            return buffer.ReadString();
        }

        public override void WriteToBytes(BytesBuffer buffer, string content)
        {
            buffer.WriteString(content.Replace("\\n", "\n")); // 解决表中含有一个换行符，但读取到代码中会出现两个的问题
        }

        public override bool CheckConfigFormat(string content, out string error)
        {
            error = string.Empty;
            return true;
        }
    }
}
