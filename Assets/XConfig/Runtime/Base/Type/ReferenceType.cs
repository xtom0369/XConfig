using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace XConfig
{
    public abstract class ReferenceType : ConfigType
    {
        public override string readByteClassName => nameof(ReferenceType);

        /// <summary>
        /// 主键类型
        /// </summary>
        public IConfigType mainKeyConfigType;

        public static void ReadFromBytes(BytesBuffer buffer, out string value)
        {
            value = buffer.ReadString();
        }

        public static void ReadFromBytes(BytesBuffer buffer, out int value)
        {
            value = buffer.ReadInt32();
        }
    }

    /// <summary>
    /// 表引用类型
    /// </summary>
    /// <typeparam name="T">表类型，比如ItemsRow</typeparam>
    public class ReferenceType<T> : ReferenceType where T : XRow
    {
        /// <summary>
        /// ItemsRow
        /// </summary>
        public override string typeName => typeof(T).Name;

        /// <summary>
        /// items
        /// </summary>
        public override string configTypeName => $"{typeof(T).GetCustomAttribute<BindConfigFileNameAttribute>().configName}";

        public override string defaultValue => mainKeyConfigType.defaultValue;

        public override string writeByteTypeName => mainKeyType.Name;

        public override string referenceFileName => configTypeName;


        List<PropertyInfo> mainKeyInfos;
        Type mainKeyType;

        public ReferenceType() 
        {
            var propInfos = typeof(T).GetProperties();
            mainKeyInfos = propInfos.Where(x => x.Name == "mainKey1").ToList();
            DebugUtil.Assert(mainKeyInfos.Count > 0, $"类型 {typeof(T).Name} 中缺少主键");
            mainKeyType = mainKeyInfos[0].PropertyType;
            bool result = TryGetConfigType(mainKeyType.Name, out mainKeyConfigType);
            DebugUtil.Assert(result, $"不支持的数据类型 : {mainKeyType.Name}");
        }

        public override string ParseDefaultValue(string content)
        {
            return mainKeyConfigType.ParseDefaultValue(content);
        }

        public override string ParseKeyName(string key) 
        { 
            return $"{key}Id"; 
        }

        public override void WriteToBytes(BytesBuffer buffer, string content)
        {
            mainKeyConfigType.WriteToBytes(buffer, content);
        }

        public override void CheckConfigFormat(string content)
        {
            // 检查主键类型
            AssertParamType(content, mainKeyType);
        }
    }
}
