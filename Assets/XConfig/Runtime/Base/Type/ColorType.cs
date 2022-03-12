using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace XConfig
{
    public class ColorType : ConfigType<Color>
    {
        public override string defaultValue => $"{configTypeName}.clear";

        public override string ParseDefaultValue(string content)
        {
            if (string.IsNullOrEmpty(content))
                return defaultValue;

            string[] strs = ParseMultiParam(content);
            StringBuilder sb = new StringBuilder();
            sb.Append($"new {configTypeName}(");
            for (int i = 0; i < strs.Length; i++)
            {
                sb.Append($"{strs[i]}f");

                if (i != strs.Length - 1)
                    sb.Append(", ");
                else
                    sb.Append(")");
            }
            return sb.ToString();
        }

        public static void ReadFromBytes(BytesBuffer buffer, out Color value)
        {
            value = buffer.ReadColor();
        }

        public override void WriteToBytes(BytesBuffer buffer, string content)
        {
            string[] param = ParseMultiParam(content);
            Color color = Color.clear;
            if (param.Length == 3)
            { 
                color = new Color(float.Parse(param[0]) / 255, float.Parse(param[1]) / 255, float.Parse(param[2]) / 255);
            }
            if (param.Length == 4)
            {
                color = new Color(float.Parse(param[0]) / 255, float.Parse(param[1]) / 255, float.Parse(param[2]) / 255, float.Parse(param[3]));
            }
            buffer.WriteColor(color);
        }

        public override void CheckConfigFormat(string content)
        {
            AssertMultiParamFormat(content);

            AssertParamCount(content, new int[] { 3, 4 });

            AssertParamsType(content, typeof(Color));
        }
    }
}
