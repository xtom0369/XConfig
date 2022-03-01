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

        public static List<Type> GetTypes()
        {
            List<Type> result = new List<Type>();
            foreach (var assemblie in assemblies)
            {
                Type[] ts = assemblie.GetTypes();
                result.AddRange(ts);
            }
            return result;
        }
    }
}
