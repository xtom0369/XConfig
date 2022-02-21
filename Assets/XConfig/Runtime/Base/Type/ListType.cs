using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XConfig
{
    public abstract class ListType : ConfigType
    {
        public override string RawTypeName => $"List<{itemConfigType.RawTypeName}>";

        public override string DefaultValue => $"new {RawTypeName}()";

        public override bool NeedExplicitCast => true;

        /// <summary>
        /// 列表项为枚举则为枚举
        /// </summary>
        public override bool IsEnum => itemConfigType.IsEnum;

        /// <summary>
        /// 列表项为引用则为引用
        /// </summary>
        public override bool IsReference => itemConfigType.IsReference;

        public ConfigType itemConfigType;

        StringBuilder _sb = new StringBuilder();
        public ListType(ConfigType itemConfigType) 
        {
            this.itemConfigType = itemConfigType;
        }

        public override string ParseDefaultValue(string content)
        {
            _sb.Clear();
            _sb.Append($"new {RawTypeName}() {{ ");
            string[] items = content.Split('|');
            for (int i = 0; i < items.Length; i++)
            {
                if (i > 0) _sb.Append(",");
                _sb.Append($"{items[i]}");
            }
            _sb.Append("}");
            return _sb.ToString();
        }

        public override void WriteToBytes(BytesBuffer buffer, string content)
        {
            string[] items = content.Split('|');
            buffer.WriteByte((byte)items.Length); // 先写入数组长度, 短整型存储
            foreach (var item in items)
            {
                if (!itemConfigType.CheckConfigFormat(item, out var error))
                    DebugUtil.Assert(false, error);

                itemConfigType.WriteToBytes(buffer, item);
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

    public sealed class ListType<T> : ListType
    {
        public ListType(ConfigType configType) : base(configType)
        {
        }
    }
}
