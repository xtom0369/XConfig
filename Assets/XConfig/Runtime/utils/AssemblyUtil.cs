using System;
using System.Collections.Generic;
using System.Reflection;

public class AssemblyUtil
{
#if SERVER_ENABLE
    private static string _battleCoreAssembly = Assembly.Load("battle_core").FullName;
#else
    private static string _assembly = Assembly.Load("Assembly-CSharp").FullName;
    private static string _battleCoreAssembly = Assembly.Load("Yhkt.Game.BattleCore").FullName;
    private static string _commonAssembly = Assembly.Load("Yhkt.Game.Common").FullName;
#if UNITY_EDITOR
    private static string _editorAssembly = Assembly.Load("Assembly-CSharp-Editor").FullName;
#endif
#endif
    /// <summary>
    /// 获取配置表相关的类型、获取状态机Action相关的类型、获取状态机事件相关的类型
    /// </summary>
    /// <returns></returns>
    static public Type GetType(string type)
    {
#if SERVER_ENABLE
        Type t = Type.GetType(type + "," + _battleCoreAssembly);
#else
        Type t = Type.GetType(type + "," + _assembly);
        if (t == null)
            t = Type.GetType(type + "," + _battleCoreAssembly);
        if (t == null)
            t = Type.GetType(type + "," + _commonAssembly);
#if UNITY_EDITOR
        if (t == null)
            t = Type.GetType(type + "," + _editorAssembly);
#endif
#endif
        return t;
    }
}