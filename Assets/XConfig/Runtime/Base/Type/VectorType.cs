using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

namespace XConfig
{
    public abstract class VectorType<T> : ConfigType<T>
    {
        public sealed override string defaultValue => $"{configTypeName}.zero";

        public abstract int count { get; }

        public sealed override string ParseDefaultValue(string content)
        {
            string[] strs = ParseMultiParam(content);
            StringBuilder sb = new StringBuilder();
            sb.Append($"new {configTypeName}(");
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

        public override void CheckConfigFormat(string content)
        {
            AssertMultiParamFormat(content);

            AssertParamCount(content, count);

            AssertParamsType(content, typeof(float));
        }
    }
}
