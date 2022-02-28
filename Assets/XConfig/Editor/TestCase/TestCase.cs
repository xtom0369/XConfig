using UnityEngine;
using System.Text;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace XConfig.Editor
{
    public abstract class TestCase
    {
        public abstract void Run();


        public static void RunAllTestCase() 
        {
            var assembly = Assembly.Load("Assembly-CSharp-Editor");
            var types = assembly.GetTypes().Where(x => typeof(TestCase).IsAssignableFrom(x) && !x.IsAbstract);

            foreach (var type in types) 
            {
                DebugUtil.Log($"开始执行测试用例 : {type.Name}");

                try
                {
                    var testcase = Activator.CreateInstance(type) as TestCase;
                    testcase.Run();
                    DebugUtil.Log($"<color=#00ff00>执行测试用例 {type.Name} 成功</color>");
                }
                catch (Exception e) 
                {
                    DebugUtil.LogError($"<color=red>开始执行测试用例 {type.Name} 失败\n</color>{e}");
                }

            }
        }
    }
}
