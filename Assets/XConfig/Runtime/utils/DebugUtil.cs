using UnityEngine;
using System;
using System.Reflection;
using UnityEditor;
using System.Diagnostics;

//日志掩码类型
//新增新的日志显示掩码，步骤：1在log_mask表中添加新行，2在下面的枚举类中添加新的枚举类型
public enum LogMask
{
    COMMON,//公共
    HJG,//逍遥
    NETWORK,//网络相关
    XT,//Michael
    XXX,//小星星
    LCH,//Fox
    CC,//璀璨
    TESTER,//测试
    HJD,//达达
    WK,//悟空
    XJ,//小姜
    TEST_CASE,//测试用例
    BATTLE_NUMERICAL,//战斗内数值
    ZR,//阿瑞
    JXL,//小龙
    PZH,//老虎
    CH,//策划
    BUILDIN_SERVER,//客户端中启动的服务器日志
    HCY,//应子
    XZ,//轩之
    FSM,//状态机
}

public class DebugUtil
{
    static public uint CUR_BATTLE_LOGIC_FRAME = 0;//当前逻辑帧序号，未进入战斗则为0
    static public bool isLog = true;
    static public int showMask = 0;//0表示所有类型的log都不打印
    static public int svnVersion;//svn版本号
    public delegate string GenLog();
    [Conditional("LOG_ENABLE")]
    public static void LogWithTag(LogMask mask, object tag, object log, params object[] args)
    {
        Log(mask, "[" + tag + "]" + log, args);
    }
    [Conditional("LOG_ENABLE")]
    public static void Log(LogMask mask, object log, params object[] args)
    {
        if (showMask > 0 && (showMask & (1 << (int)mask)) > 0)
            Log(log, args);
    }

    //这个是私有方法，不能对外暴露
    static void Log(object log, params object[] args)
    {
        if (isLog)
        {
            string logStr = log.ToString();
            if (args.Length > 0)
                UnityLog(string.Format(logStr, args), null, LogType.Log);
            else
                UnityLog(log.ToString(), null, LogType.Log);
        }
    }
    [Conditional("LOG_ENABLE")]
    public static void LogWarning(LogMask mask, object log, params object[] args)
    {
        if (showMask > 0 && (showMask & (1 << (int)mask)) > 0)
            LogWithTag(mask, "警告", log, args);
    }
	public static void LogError(object log, params object[] args)
	{
        LogError(log, null, args);
    }
	public static void LogError(object log, UnityEngine.Object context, params object[] args)
	{
		UnityLog(string.Format(log.ToString(), args), context, LogType.Error);
	}
    public static void LogErrorThread(object log, params object[] args)
    {
        UnityLogThread(string.Format(log.ToString(), args), LogType.Error);
    }
    [Conditional("ASSERT_ENABLE")]
    public static void Assert(bool result)
    {
        if (result)
            return;
#if UNITY_EDITOR
        int frameCount = Time.frameCount;
        string logStr = string.Format("<Assert :{0}#{1}#{2}>", svnVersion, DateTime.Now.ToString("HH:mm:ss.fff"), frameCount);
#else
        string logStr = string.Format("<Assert :{0}#{1}>", svnVersion, DateTime.Now.ToString("HH:mm:ss.fff"));
#endif
        throw new Exception(logStr);
    }
    [Conditional("ASSERT_ENABLE")]
    public static void Assert(bool result, GenLog genLog)
    {
        if (result)
            return;
#if UNITY_EDITOR
        int frameCount = Time.frameCount;

        string logStr = string.Format("<Assert :{0}#{1}#{2}>{3}", svnVersion, DateTime.Now.ToString("HH:mm:ss.fff"), frameCount, genLog());
#else
        string logStr = string.Format("<Assert :{0}#{1}>{2}", svnVersion, DateTime.Now.ToString("HH:mm:ss.fff"), genLog());
#endif
        throw new Exception(logStr);
    }    
    [Conditional("ASSERT_ENABLE")]
    public static void Assert(bool result, object log)
    {
        if (result)
            return;
#if UNITY_EDITOR
        int frameCount = Time.frameCount;
        string logStr = string.Format("<Assert :{0}#{1}#{2}>{3}", svnVersion, DateTime.Now.ToString("HH:mm:ss.fff"), frameCount, log);
#else
        string logStr = string.Format("<Assert :{0}#{1}>{2}", svnVersion, DateTime.Now.ToString("HH:mm:ss.fff"), log);
#endif
        throw new Exception(logStr);
    }
    [Conditional("ASSERT_ENABLE")]
    public static void Assert(bool result, object log, object arg1)
    {
        if (result)
            return;
#if UNITY_EDITOR
        int frameCount = Time.frameCount;
        string logStr = log.ToString();
        logStr = string.Format(logStr, arg1);
        logStr = string.Format("<Assert :{0}#{1}#{2}>{3}", svnVersion, DateTime.Now.ToString("HH:mm:ss.fff"), frameCount, logStr);
#else
        string logStr = log.ToString();
        logStr = string.Format(logStr, arg1);
        logStr = string.Format("<Assert :{0}#{1}>{2}", svnVersion, DateTime.Now.ToString("HH:mm:ss.fff"), logStr);
#endif
        throw new Exception(logStr);
    }
    [Conditional("ASSERT_ENABLE")]
    public static void Assert(bool result, object log, object arg1, object arg2)
    {
        if (result)
            return;
#if UNITY_EDITOR
        int frameCount = Time.frameCount;
        string logStr = log.ToString();
        logStr = string.Format(logStr, arg1, arg2);
        logStr = string.Format("<Assert :{0}#{1}#{2}>{3}", svnVersion, DateTime.Now.ToString("HH:mm:ss.fff"), frameCount, logStr);
#else
        string logStr = log.ToString();
        logStr = string.Format(logStr, arg1, arg2);
        logStr = string.Format("<Assert :{0}#{1}>{2}", svnVersion, DateTime.Now.ToString("HH:mm:ss.fff"), logStr);
#endif
        throw new Exception(logStr);
    }
    [Conditional("ASSERT_ENABLE")]
    public static void Assert(bool result, object log, object arg1, object arg2, object arg3)
    {
        if (result)
            return;
#if UNITY_EDITOR
        int frameCount = Time.frameCount;
        string logStr = log.ToString();
        logStr = string.Format(logStr, arg1, arg2, arg3);
        logStr = string.Format("<Assert :{0}#{1}#{2}>{3}", svnVersion, DateTime.Now.ToString("HH:mm:ss.fff"), frameCount, logStr);
#else
        string logStr = log.ToString();
        logStr = string.Format(logStr, arg1, arg2, arg3);
        logStr = string.Format("<Assert :{0}#{1}>{2}", svnVersion, DateTime.Now.ToString("HH:mm:ss.fff"), logStr);
#endif
        throw new Exception(logStr);
    }
    [Conditional("ASSERT_ENABLE")]
    public static void Assert(bool result, object log, object arg1, object arg2, object arg3, object arg4)
    {
        if (result)
            return;
#if UNITY_EDITOR
        int frameCount = Time.frameCount;
        string logStr = log.ToString();
        logStr = string.Format(logStr, arg1, arg2, arg3, arg4);
        logStr = string.Format("<Assert :{0}#{1}#{2}>{3}", svnVersion, DateTime.Now.ToString("HH:mm:ss.fff"), frameCount, logStr);
#else
        string logStr = log.ToString();
        logStr = string.Format(logStr, arg1, arg2, arg3, arg4);
        logStr = string.Format("<Assert :{0}#{1}>{2}", svnVersion, DateTime.Now.ToString("HH:mm:ss.fff"), logStr);
#endif
        throw new Exception(logStr);
    }
    [Conditional("ASSERT_ENABLE")]
    public static void Assert(bool result, object log, object arg1, object arg2, object arg3, object arg4, object arg5)
    {
        if (result)
            return;
#if UNITY_EDITOR
        int frameCount = Time.frameCount;
        string logStr = log.ToString();
        logStr = string.Format(logStr, arg1, arg2, arg3, arg4, arg5);
        logStr = string.Format("<Assert :{0}#{1}#{2}>{3}", svnVersion, DateTime.Now.ToString("HH:mm:ss.fff"), frameCount, logStr);
#else
        string logStr = log.ToString();
        logStr = string.Format(logStr, arg1, arg2, arg3, arg4, arg5);
        logStr = string.Format("<Assert :{0}#{1}>{2}", svnVersion, DateTime.Now.ToString("HH:mm:ss.fff"), logStr);
#endif
        throw new Exception(logStr);
    }
    [Conditional("ASSERT_ENABLE")]
    public static void Assert(bool result, object log, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
    {
        if (result)
            return;
#if UNITY_EDITOR
        int frameCount = Time.frameCount;
        string logStr = log.ToString();
        logStr = string.Format(logStr, arg1, arg2, arg3, arg4, arg5, arg6);
        logStr = string.Format("<Assert :{0}#{1}#{2}>{3}", svnVersion, DateTime.Now.ToString("HH:mm:ss.fff"), frameCount, logStr);
#else
        string logStr = log.ToString();
        logStr = string.Format(logStr, arg1, arg2, arg3, arg4, arg5, arg6);
        logStr = string.Format("<Assert :{0}#{1}#{2}>{3}", svnVersion, DateTime.Now.ToString("HH:mm:ss.fff"), Time.frameCount, logStr);
#endif
        throw new Exception(logStr);
    }

	static void UnityLog(string log, UnityEngine.Object context, LogType emType)
	{
		log = string.Format("<{0}#{1}#{2}#{3}>{4}\n", svnVersion, DateTime.Now.ToString("HH:mm:ss.fff"), Time.frameCount, CUR_BATTLE_LOGIC_FRAME, log);
		switch (emType)
		{
			case LogType.Log:
                if(context != null)
				    UnityEngine.Debug.Log(log, context);
                else
					UnityEngine.Debug.Log(log);
				break;

			case LogType.Warning:
				if (context != null)
					UnityEngine.Debug.LogWarning(log, context);
				else
					UnityEngine.Debug.LogWarning(log);
				break;

			case LogType.Error:
				if (context != null)
					UnityEngine.Debug.LogError(log, context);
				else
					UnityEngine.Debug.LogError(log);
				break;
		}
	}

    static void UnityLogThread(string log, LogType emType)
    {
        log = string.Format("<{0}>{1}\n", DateTime.Now.ToString("HH:mm:ss.fff"), log);
        switch (emType)
        {
            case LogType.Log:
                UnityEngine.Debug.Log(log);
                break;

            case LogType.Warning:
                UnityEngine.Debug.LogWarning(log);
                break;

            case LogType.Error:
                UnityEngine.Debug.LogError(log);
                break;
        }
    }
}
