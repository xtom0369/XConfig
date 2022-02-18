using System;
using System.Reflection;
using UnityEngine;

namespace XConfig
{
    public class XRow
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
            Debug.AssertFormat(isValid, string.Format("\n{0} 第{1}行有问题:{2}", fileName, rowIndex, logStr));
        }

        protected void AssertFloat(float f)
        {
            string str = f.ToString();
            int dotIdx = str.IndexOf(".");
            if (dotIdx >= 0)
            {
                int decimalDigits = str.Length - dotIdx - 1;
                if (str[dotIdx] == '-')//要忽略负号
                    decimalDigits -= 1;
                Assert(decimalDigits <= 4, "float类型最多只能配4位小数:{0}", str);
            }
        }
        //检查字串中是否包含大写字母
        protected void CheckIsContainUpperChar(string str)
        {
            if (string.IsNullOrEmpty(str))
                return;
            char[] chars = str.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                char ch = chars[i];
                if (ch >= 'A' && ch <= 'Z')
                    Assert(false, "资源路径包括资源名不能为大写：{0}", str);
            }
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