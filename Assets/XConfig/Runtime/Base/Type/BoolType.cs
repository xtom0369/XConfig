using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XConfig
{
    public class BoolType : IConfigType<bool>
    {
        public string Name => "bool";

        public bool DefaultValue => false;

        public bool ReadFromBytes(BytesBuffer buffer)
        {
            throw new NotImplementedException();
        }

        public void WriteToBytes(BytesBuffer buffer, string value)
        {
            buffer.WriteBool(value == "1");
        }

        public bool CheckConfigFormat(string content, out string errorMsg)
        {
            errorMsg = string.Empty;
            return true;
        }

        public bool CheckConfigValid(out string errorMsg)
        {
            errorMsg = string.Empty;
            return true;
        }
    }
}
