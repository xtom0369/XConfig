using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace XConfig
{
    public class ColorType : ConfigType
    {
        public override string Name => nameof(Color);

        public override string DefaultValue => $"{Name}.clear";

        public override string ParseDefaultValue(string content)
        {
            content = base.ParseDefaultValue(content);
            string[] strs = ParseMultiContent(content);
            StringBuilder sb = new StringBuilder();
            sb.Append($"new {Name}(");
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

        public static Color ReadFromBytes(BytesBuffer buffer)
        {
            return buffer.ReadColor();
        }

        public override void WriteToBytes(BytesBuffer buffer, string content)
        {
            string[] param = ParseMultiContent(content);
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

        public override bool CheckConfigFormat(string content, out string error)
        {
            if (!content.StartsWith("(") || !content.EndsWith(")"))
            {
                error = $"{Name}类型的值不是以左括号开始右括号结束，当前为 : {content}";
                return false;
            }

            string[] strs = ParseMultiContent(content);
            if (strs.Length < 3 || strs.Length > 4)
            {
                error = $"{Name}只支持3或4个参数，当前为参数数量为 : {strs.Length}";
                return false;
            }

            foreach (var str in strs)
            {
                if (!float.TryParse(str, out var value))
                {
                    error = $"{Name}类型的参数只能为整数或浮点数，当前为 : {content}";
                    return false;
                }
            }

            error = string.Empty;
            return true;
        }
    }
}
