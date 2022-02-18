using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace XConfig
{
    public class DateTimeType : ConfigType
    {
        public override string Name => "DateTime";

        public override string DefaultValue => "DateTime.MinValue";

        public override string ParseDefaultValue(string content)
        {
            content = base.ParseDefaultValue(content);
            if (content == DefaultValue)
                return content;

            string[] strs = ParseMultiContent(content);
            StringBuilder sb = new StringBuilder();
            sb.Append($"new {Name}(");
            for (int i = 0; i < 6; i++)
            {
                if(i < strs.Length)
                    sb.Append($"{strs[i]}");
                else
                    sb.Append($"0");

                if (i != 5)
                    sb.Append(", ");
                else
                    sb.Append(")");
            }
            return sb.ToString();
        }

        public static DateTime ReadFromBytes(BytesBuffer buffer)
        {
            byte have = buffer.ReadByte();
            if (have == 0)
                return DateTime.MinValue;

            int year = buffer.ReadInt32();
            int month = buffer.ReadInt32();
            int day = buffer.ReadInt32();
            int hour = buffer.ReadInt32();
            int minute = buffer.ReadInt32();
            int second = buffer.ReadInt32();
            return new DateTime(year, month, day, hour, minute, second);
        }

        public override void WriteToBytes(BytesBuffer buffer, string content)
        {
            if (content == DefaultValue)
            {
                buffer.WriteByte(0);
            }
            else
            {
                buffer.WriteByte(1);

                int[] values = new int[6];
                string[] strs = ParseMultiContent(content);
                for (int i = 0; i < strs.Length; i++)
                    values[i] = int.Parse(strs[i]);

                foreach (var value in values)
                    buffer.WriteInt32(value);
            }
        }

        public override bool CheckConfigFormat(string content, out string error)
        {
            if (content == DefaultValue)
            {
                error = string.Empty;
                return true;
            }

            if (!content.StartsWith("(") || !content.EndsWith(")"))
            {
                error = $"{Name}类型的值不是以左括号开始右括号结束，当前为 : {content}";
                return false;
            }

            string[] strs = ParseMultiContent(content);
            if (strs.Length != 3 && strs.Length != 6)
            {
                error = $"{Name} 类型的值长度只能为3或6，当前为 : {content}";
                return false;
            }

            foreach (var str in strs)
            {
                if (!int.TryParse(str, out var value))
                {
                    error = $"{Name}类型的值只能为整数，当前为 : {content}";
                    return false;
                }
            }

            error = string.Empty;
            return true;
        }
    }
}
