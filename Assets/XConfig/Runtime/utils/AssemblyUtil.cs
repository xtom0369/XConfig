using System;
using System.Collections.Generic;
using System.Reflection;

namespace XConfig 
{
    public class AssemblyUtil
    {
        private static string _assembly = Assembly.Load("Assembly-CSharp").FullName;
#if UNITY_EDITOR
        private static string _editorAssembly = Assembly.Load("Assembly-CSharp-Editor").FullName;
#endif
        /// <summary>
        /// 获取配置表相关的类型、获取状态机Action相关的类型、获取状态机事件相关的类型
        /// </summary>
        /// <returns></returns>
        static public Type GetType(string type)
        {
            Type t = Type.GetType(type + "," + _assembly);
#if UNITY_EDITOR
            if (t == null)
                t = Type.GetType(type + "," + _editorAssembly);
#endif
            return t;
        }
    }
}
