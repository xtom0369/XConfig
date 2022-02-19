using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XConfig
{
    public abstract class ReferenceType : ConfigType
    {
        public override string TypeName => nameof(ReferenceType);

        public override string ParseDefaultValue(string content)
        {
            if (!content.StartsWith("\""))
                content = "\"" + content;

            if (!content.EndsWith("\""))
                content = content + "\"";

            return content;
        }

        public static string ReadFromBytes(BytesBuffer buffer)
        {
            return buffer.ReadString();
        }

        public override void WriteToBytes(BytesBuffer buffer, string content)
        {
            buffer.WriteString(content);
        }
    }

    public class ReferenceType<T> : ReferenceType where T : XRow
    {
        public override string RawTypeName => typeof(T).Name;

        public override string DefaultValue => "null";

        public override bool CheckConfigFormat(string content, out string error)
        {
            error = string.Empty;
            return true;
        }
    }
}
