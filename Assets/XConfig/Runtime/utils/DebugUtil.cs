using UnityEngine;
using System;
using System.Reflection;
using UnityEditor;
using System.Diagnostics;

namespace XConfig 
{
    public class DebugUtil
    {
        static public bool isLog = true;

        public static void Log(object log, params object[] args)
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
        public static void LogError(object log, params object[] args)
        {
            LogError(log, null, args);
        }
        public static void LogError(object log, UnityEngine.Object context, params object[] args)
        {
            UnityLog(string.Format(log.ToString(), args), context, LogType.Error);
        }
        public static void Assert(bool result, object log)
        {
            if (result)
                return;

            string logStr = $"<Assert :{DateTime.Now.ToString("HH:mm:ss.fff")}>{log}";
            throw new Exception(logStr);
        }
        public static void Assert(bool result, object log, object arg1)
        {
            if (result)
                return;

            string logStr = log.ToString();
            logStr = string.Format(logStr, arg1);
            logStr = string.Format("<Assert :{0}>{1}", DateTime.Now.ToString("HH:mm:ss.fff"), logStr);
            throw new Exception(logStr);
        }
        public static void Assert(bool result, object log, object arg1, object arg2)
        {
            if (result)
                return;

            string logStr = log.ToString();
            logStr = string.Format(logStr, arg1, arg2);
            logStr = string.Format("<Assert :{0}>{1}", DateTime.Now.ToString("HH:mm:ss.fff"), logStr);
            throw new Exception(logStr);
        }
        public static void Assert(bool result, object log, object arg1, object arg2, object arg3)
        {
            if (result)
                return;

            string logStr = log.ToString();
            logStr = string.Format(logStr, arg1, arg2, arg3);
            logStr = string.Format("<Assert :{0}>{1}", DateTime.Now.ToString("HH:mm:ss.fff"), logStr);
            throw new Exception(logStr);
        }

        static void UnityLog(string log, UnityEngine.Object context, LogType emType)
        {
            log = string.Format("<{0}#{1}>{2}\n", DateTime.Now.ToString("HH:mm:ss.fff"), Time.frameCount, log);
            switch (emType)
            {
                case LogType.Log:
                    if (context != null)
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
    }

}
