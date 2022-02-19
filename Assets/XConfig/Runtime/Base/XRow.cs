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

        virtual public void FromBytes(BytesBuffer buffer)
        {
        }

        virtual public void OnAfterInit()
        {
        }

        virtual public void OnCheckWhenExport()
        {
        }

        protected void Assert(bool isValid, string msg, params object[] args)
        {
            string logStr = string.Format(msg, args);
            DebugUtil.Assert(isValid, string.Format("\n{0} 第{1}行有问题:{2}", fileName, rowIndex, logStr));
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