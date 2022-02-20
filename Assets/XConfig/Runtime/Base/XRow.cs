using System;
using System.Reflection;
using UnityEngine;

namespace XConfig
{
    public abstract class XRow
    {
        [NonSerialized]
        public string fileName;
        [NonSerialized]
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