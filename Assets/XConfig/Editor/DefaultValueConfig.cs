using System;
using UnityEditor;
using UnityEngine;

namespace XConfig.Editor 
{
    public class DefaultValueConfig : ScriptableObject
    {
        protected static DefaultValueConfig Instance;
        public static DefaultValueConfig Config
        {
            get
            {
                if (Instance != null) return Instance;

                var guids = AssetDatabase.FindAssets("t:DefaultValueConfig");
                if (guids.Length > 0)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    Instance = AssetDatabase.LoadAssetAtPath<DefaultValueConfig>(path);

                    return Instance;
                }

                return null;
            }
        }

        public DefaultValuePair[] pairs;

        public string GetDefaultValue(string fileName, string type, string fieldName, Flag flag)
        {
            foreach (var pair in pairs)
            {
                if (type.StartsWith(pair.type))
                    return ParseDefaultValue(fileName, type, fieldName, flag, pair.value);
            }
            return null;
        }

        public static string ParseDefaultValue(string fileName, string type, string filedName, Flag flag, string defaultVaule)
        {
            if (ConfigType.TryGetConfigType(type, out var configType))
            {
                if(!configType.CheckConfigFormat(defaultVaule, out var error))
                    DebugUtil.Assert(false, $"{fileName}.bytes 中 {filedName} 字段默认值异常, {error}");

                return configType.ParseDefaultValue(defaultVaule);
            }

            Type t = AssemblyUtil.GetType(type);
            if (type.StartsWith("List<"))//列表默认值为空列表，不为null，不然做检验的时候不好处理
            {
                string resultSt = "new " + type.ToString() + "()";
                if (!string.IsNullOrEmpty(defaultVaule) && defaultVaule != "null")
                {
                    resultSt += "{";
                    string[] items = defaultVaule.Split('|');
                    for (int i = 0; i < items.Length; i++)
                    {
                        if (i > 0)
                            resultSt += ",";
                        resultSt += items[i];
                    }
                    resultSt += "}";
                }
                return resultSt;
            }
            if (flag.IsReference) return "\"" + defaultVaule + "\"";//引用类型，缺省值都为字串
            if (t == null || !t.IsEnum || !EnumIsDefined(t, defaultVaule)) return defaultVaule;
            try
            {
                return type + "." + Enum.Parse(t, defaultVaule);
            }
            catch
            {
                DebugUtil.LogError("不能解析的枚举类{0}，请检查拼写或添加定义", type);
                return "0";
            }
        }

        public static bool EnumIsDefined(Type t, string v)
        {
            int i;
            if (int.TryParse(v, out i))
                return Enum.IsDefined(t, i);
            return Enum.IsDefined(t, v);
        }
    }

    [System.Serializable]
    public struct DefaultValuePair
    {
        public string type;
        public string value;
    }

}
