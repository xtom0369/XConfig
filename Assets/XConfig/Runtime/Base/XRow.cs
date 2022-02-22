using System;
using System.Reflection;
using UnityEngine;

namespace XConfig
{
    public abstract class XRow<TKey> : XRow 
    {
        public abstract TKey mainKey1 { get; }
    }

    public abstract class XRow<TKey1, TKey2> : XRow
    {
        public abstract TKey1 mainKey1 { get; }
        public abstract TKey2 mainKey2 { get; }
    }


    public abstract class XRow
    {
        public string fileName;
        public int rowIndex;

        public abstract void ReadFromBytes(BytesBuffer buffer);

        public virtual void OnAfterInit()
        {
        }

        public virtual void OnCheckWhenExport()
        {
        }

        protected void Assert(bool isValid, string msg, params object[] args)
        {
            string logStr = string.Format(msg, args);
            DebugUtil.Assert(isValid, $"\n{fileName} 第{rowIndex}行异常 : {logStr}");
        }

        public override string ToString()
        {
            FieldInfo[] fileds = this.GetType().GetFields();
            string result = this.GetType().Name + " ";
            foreach (FieldInfo field in fileds)
                result += field.Name + "=" + field.GetValue(this) + " ";
            return result;
        }
    }
}