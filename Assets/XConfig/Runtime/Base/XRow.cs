using System;
using System.Collections;
using System.Text;
using UnityEngine;

namespace XConfig
{
    public abstract class XRow<TKey> : XRow 
    {
        public abstract TKey mainKey1 { get; }
        protected void Assert(bool isValid, string msg)
        {
            DebugUtil.Assert(isValid, $"表 = {fileName} 主键 {mainKey1} 的行异常，{msg}");
        }
    }

    public abstract class XRow<TKey1, TKey2> : XRow
    {
        public abstract TKey1 mainKey1 { get; }
        public abstract TKey2 mainKey2 { get; }
    }


    public abstract class XRow
    {
        public string fileName { get; set; }

        StringBuilder _sb;

        public abstract void ReadFromBytes(BytesBuffer buffer);

        public virtual void OnAfterInit() { }

        public virtual void OnCheckWhenExport() { }

        public override string ToString()
        {
            if (_sb == null) _sb = new StringBuilder();
            else _sb.Clear();

            var fileds = this.GetType().GetProperties();
            _sb.Append(this.GetType().Name);
            _sb.Append(" {\n");
            foreach (var field in fileds)
            {
                var value = field.GetValue(this);
                if (value is IList list)
                {
                    _sb.Append(field.Name);
                    _sb.Append(" {\n");
                    for (int i = 0; i < list.Count; i++) 
                        _sb.AppendLine($"{i} = {list[i]}");
                    _sb.AppendLine("}");
                }
                else
                    _sb.AppendLine($"{field.Name} = {value}");
            }
            _sb.AppendLine("}");
            return _sb.ToString();
        }
    }
}