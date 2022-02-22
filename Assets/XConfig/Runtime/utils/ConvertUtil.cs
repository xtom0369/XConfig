using UnityEngine;
using System.Text;
using System;
using System.Collections.Generic;

namespace XConfig 
{
    public class ConvertUtil
    {
        /// <summary>
        /// 转换成首字母小写
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        static public string ToFirstCharLower(string name)
        {
            return char.ToLower(name[0]) + name.Substring(1);
        }
        /// <summary>
        /// 转换成驼峰命名
        /// </summary>
        /// <param name="oldName"></param>
        /// <returns></returns>
        static public string UnderscoreToCamel(string oldName)
        {
            string[] subNames = oldName.Split('_');
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < subNames.Length; i++)
            {
                sb.Append(Char.ToUpper(subNames[i][0]));
                sb.Append(subNames[i].Substring(1));
            }
            return sb.ToString();
        }
    }

}
