using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XConfig
{
    public abstract class ListType : ConfigType
    {
        public override string RawTypeName => $"List<{configType.RawTypeName}>";

        public override string DefaultValue => ParseDefaultValue("0");

        public override bool NeedExplicitCast => true;

        public ConfigType configType;

        public ListType(ConfigType configType) 
        {
            this.configType = configType;
        }

        public override string ParseDefaultValue(string content)
        {
            return $"{content}";
        }

        public static List<> ReadFromBytes(BytesBuffer buffer)
        {
            return buffer.ReadInt16();
        }

        public static void ReadFromBytes(BytesBuffer buffer, out short value)
        {
            value = ReadFromBytes(buffer);
        }

        public override void WriteToBytes(BytesBuffer buffer, string content)
        {
            string[] items = content.Split('|');
            buffer.WriteUInt16((ushort)items.Length);//先写入数组长度
            foreach (var item in items)
            {
                if (!configType.CheckConfigFormat(item, out var error))
                    DebugUtil.Assert(false, error);

                configType.WriteToBytes(buffer, item);
            }
        }

        public override bool CheckConfigFormat(string content, out string error)
        {
            string[] items = content.Split('|');
            foreach (var item in items)
            {
                if (string.IsNullOrEmpty(item))
                {
                    error = $"列表元素不能为空 : {item}";
                    return false;
                }
            }

            error = string.Empty;
            return true;
        }
    }
}
