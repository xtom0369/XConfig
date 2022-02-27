﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XConfig
{
    public abstract class ListType : ConfigType
    {
        public override string TypeName => $"List<{ItemConfigType.TypeName}>";

        public override string ConfigTypeName => $"List<{ItemConfigType.ConfigTypeName}>";

        public override string DefaultValue => $"new {TypeName}()";

        /// <summary>
        /// 列表项为枚举则为枚举
        /// </summary>
        public override bool IsEnum => ItemConfigType.IsEnum;

        /// <summary>
        /// 列表项为引用则为引用
        /// </summary>
        public override bool IsReference => ItemConfigType.IsReference;

        public override string ReferenceFileName => ItemConfigType.ReferenceFileName;

        /// <summary>
        /// 列表项写入二进制类型
        /// </summary>
        public override string WriteByteTypeName => ItemConfigType.WriteByteTypeName;

        /// <summary>
        /// 列表项类型
        /// </summary>
        public ConfigType ItemConfigType { get; private set; }

        StringBuilder _sb = new StringBuilder();
        public ListType(ConfigType itemConfigType) 
        {
            this.ItemConfigType = itemConfigType;
        }

        public override string ParseDefaultValue(string content)
        {
            _sb.Clear();
            _sb.Append($"new {ConfigTypeName}() {{ ");
            string[] items = content.Split('|');
            for (int i = 0; i < items.Length; i++)
            {
                if (i > 0) _sb.Append(",");
                _sb.Append($"{items[i]}");
            }
            _sb.Append("}");
            return _sb.ToString();
        }

        public override string ParseKeyName(string key)
        {
            return IsReference ? $"{key}Ids" : key;
        }

        public override void WriteToBytes(BytesBuffer buffer, string content)
        {
            string[] items = content.Split('|');
            buffer.WriteByte((byte)items.Length); // 先写入数组长度, 短整型存储
            foreach (var item in items)
            {
                if (!ItemConfigType.CheckConfigFormat(item, out var error))
                    DebugUtil.Assert(false, error);

                ItemConfigType.WriteToBytes(buffer, item);
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
        public ListType(ConfigType configType) : base(configType) { }
    }
}
