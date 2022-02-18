using System;
using System.Collections.Generic;
using System.Reflection;

namespace XConfig 
{
    public class AssemblyUtil
    {
        static Assembly[] assemblies = new Assembly[]
        {
            Assembly.Load("Assembly-CSharp"),
        };

        /// <summary>
        /// 获取配置表相关的类型、获取状态机Action相关的类型、获取状态机事件相关的类型
        /// </summary>
        /// <returns></returns>
        public static Type GetType(string type)
        {
            foreach (var assemblie in assemblies) 
            {
                Type t = assemblie.GetType(type);
                if (t != null)
                    return t;
            }
            return null;
        }
    }
}
