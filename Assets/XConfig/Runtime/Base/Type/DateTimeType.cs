using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace XConfig
{
    public class DateTimeType : ConfigType<DateTime>
    {
        public override string defaultValue => "DateTime.MinValue";

        public override string ParseDefaultValue(string content)
        {
            string[] strs = ParseMultiParam(content);
            StringBuilder sb = new StringBuilder();
            sb.Append($"new {configTypeName}(");
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
            int year = buffer.ReadInt32();
            int month = buffer.ReadInt32();
            int day = buffer.ReadInt32();
            int hour = buffer.ReadInt32();
            int minute = buffer.ReadInt32();
            int second = buffer.ReadInt32();
            return new DateTime(year, month, day, hour, minute, second);
        }

        public static void ReadFromBytes(BytesBuffer buffer, out DateTime value)
        {
            value = ReadFromBytes(buffer);
        }

        public override void WriteToBytes(BytesBuffer buffer, string content)
        {
            int[] values = new int[6];
            string[] strs = ParseMultiParam(content);
            for (int i = 0; i < strs.Length; i++)
                values[i] = int.Parse(strs[i]);

            foreach (var value in values)
                buffer.WriteInt32(value);
        }

        public override void CheckConfigFormat(string content)
        {
            AssertMultiParamFormat(content);

            AssertParamCount(content, new int[] { 3, 6 });

            AssertParamsType(content, typeof(int));
        }
    }
}
