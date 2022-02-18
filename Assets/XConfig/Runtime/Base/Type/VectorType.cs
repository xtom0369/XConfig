using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

namespace XConfig
{
    public abstract class VectorType : ConfigType
    {
        public sealed override string DefaultValue => $"{Name}.zero";

        public abstract int Count { get; }

        public sealed override string ParseDefaultValue(string content)
        {
            content = base.ParseDefaultValue(content);
            string[] strs = ParseMultiContent(content);
            StringBuilder sb = new StringBuilder();
            sb.Append($"new {Name}(");
            for (int i = 0; i < strs.Length; i++) 
            {
                sb.Append($"{strs[i]}f");

                if(i != strs.Length-1)
                    sb.Append(", ");
                else
                    sb.Append(")");
            }
            return sb.ToString();
        }

        public override bool CheckConfigFormat(string content, out string error)
        {
            return CheckVectorFormat(content, Count, out error);
        }

        bool CheckVectorFormat(string content, int num, out string error)
        {
            if (!content.StartsWith("(") || !content.EndsWith(")"))
            {
                error = $"{Name}类型的值不是以左括号开始右括号结束，当前为 : {content}";
                return false;
            }

            string[] strs = ParseMultiContent(content);
            if (strs.Length != num)
            {
                error = $"{Name} 类型的值长度不对，当前为 : {content}，{strs.Length} != {num}";
                return false;
            }

            foreach (var str in strs)
            {
                if (!float.TryParse(str, out var value))
                {
                    error = $"{Name}类型的值只能为整数或浮点数，当前为 : {content}";
                    return false;
                }
            }

            error = string.Empty;
            return true;
        }
    }
}
