using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace XConfig
{
    public abstract class ReferenceType : ConfigType
    {
        public override string ConfigTypeName => nameof(ReferenceType);

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
    /// <typeparam name="T"></typeparam>
    public class ReferenceType<T> : ReferenceType where T : XRow
    {
        public override string TypeName => typeof(T).Name;

        public override string DefaultValue => mainKeyConfigType.DefaultValue;

        List<PropertyInfo> mainKeyInfos;
        Type mainKeyType;

        public ReferenceType() 
        {
            var propInfos = typeof(T).GetProperties();
            mainKeyInfos = propInfos.Where(x => x.GetCustomAttribute<ConfigMainKeyAttribute>() != null).ToList();
            DebugUtil.Assert(mainKeyInfos.Count > 0, $"类型 {typeof(T).Name} 中缺少主键");
            mainKeyType = mainKeyInfos[0].PropertyType;
            bool result = TryGetConfigType(mainKeyType.Name, out mainKeyConfigType);
            DebugUtil.Assert(result, $"不支持的数据类型 : {mainKeyType.Name}");
        }

        public override string ParseDefaultValue(string content)
        {
            return mainKeyConfigType.ParseDefaultValue(content);
        }

        public override void WriteToBytes(BytesBuffer buffer, string content)
        {
            mainKeyConfigType.WriteToBytes(buffer, content);
        }

        public override bool CheckConfigFormat(string content, out string error)
        {
            error = string.Empty;
            return true;
        }
    }
}
