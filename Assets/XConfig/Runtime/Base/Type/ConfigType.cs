using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace XConfig
{
    public abstract class ConfigType : IConfigType
    {
        public abstract string typeName { get; }

        public virtual string configTypeName => typeName;

        public virtual string readByteClassName => _type.Name;

        public abstract string writeByteTypeName { get; }

        /// <summary>
        /// 默认值，当配置字段没有设置默认值时，取DefaultValue为默认值
        /// </summary>
        public abstract string defaultValue { get; }

        public virtual bool isEnum => _type.BaseType == typeof(EnumType);

        public virtual bool isList => _type.BaseType == typeof(ListType);

        public virtual bool isReference => _type.BaseType == typeof(ReferenceType);

        public virtual string referenceFileName => throw new NotImplementedException();

        static Dictionary<string, IConfigType> _configTypeDic;
        Type _type;

        public ConfigType() 
        {
            _type = GetType();
        }

        /// <summary>
        /// 解析默认值
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public virtual string ParseDefaultValue(string content) { return content; }

        public virtual string ParseKeyName(string key) { return key; }


        /// <summary>
        /// 将当前实例中的数据，写入字节缓冲区中
        /// </summary>
        /// <param name="buffer">字节缓冲区</param>
        public abstract void WriteToBytes(BytesBuffer buffer, string content);

        /// <summary>
        /// 格式合法性检查
        /// </summary>
        /// <param name="content">配置表中配置的字符串</param>
        /// </summary>
        public abstract void CheckConfigFormat(string content);

        /// <summary>
        /// 用户根据自身需求，对类型解析出来的数据，做合法性检查
        /// 此接口只会在配置表导出阶段被执行
        /// </summary>
        public virtual void CheckConfigValid(IConfigType configType) { }

        public static bool TryGetConfigType(string typeName, out IConfigType configType)
        {
            if (_configTypeDic == null)
                BuildConfigTypeDic();

            bool result = _configTypeDic.TryGetValue(typeName, out configType);
            if (!result) _configTypeDic = null; // 避免出错时不会重复构建，主动清除类型字典
            return result;
        }

        static void BuildConfigTypeDic() 
        {
            var types = AssemblyUtil.GetTypes();

            // 所有支持的数据类型名字 => ConfigType，比如bool => BoolType
            var configTypes = types.Where(t => t.GetInterface(nameof(IConfigType)) != null && !t.IsAbstract && !t.IsGenericType)
                .Select(t => Activator.CreateInstance(t) as IConfigType);
            _configTypeDic = configTypes.ToDictionary(x => x.configTypeName, x => x);

            foreach (var configType in configTypes) 
            {
                Type t = typeof(ListType<>).MakeGenericType(configType.GetType());
                var inst = Activator.CreateInstance(t, configType) as IConfigType;
                _configTypeDic.Add(inst.configTypeName, inst);
            }

            // 所有支持的数据类型别名 => ConfigType，比如Boolean => BoolType
            foreach (var inst in configTypes)
                _configTypeDic[inst.typeName] = inst;

            // 所有支持的枚举名 => ConfigType，比如FlagType => EnumType<FlagType>
            var enumTypes = types.Where(t => t.IsEnum);
            foreach (var enumType in enumTypes)
            {
                Type t = typeof(EnumType<>).MakeGenericType(enumType);
                var inst = Activator.CreateInstance(t) as IConfigType;
                _configTypeDic.Add(inst.configTypeName, inst);

                t = typeof(ListType<>).MakeGenericType(t);
                inst = Activator.CreateInstance(t, inst) as IConfigType;
                _configTypeDic.Add(inst.configTypeName, inst);
            }

            // 所有引用类型名 => ConfigType，比如ItemsRow => ReferenceType<ItemsRow>
            var rowTypes = types.Where(t => typeof(XRow).IsAssignableFrom(t) && !t.IsAbstract);
            foreach (var rowType in rowTypes)
            {
                Type t = typeof(ReferenceType<>).MakeGenericType(rowType);
                var inst = Activator.CreateInstance(t) as IConfigType;
                _configTypeDic.Add(inst.configTypeName, inst); 

                t = typeof(ListType<>).MakeGenericType(t);
                inst = Activator.CreateInstance(t, inst) as IConfigType;
                _configTypeDic.Add(inst.configTypeName, inst);
            }
        }

        /// <summary>
        /// 解析多参数格式
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        protected string[] ParseMultiParam(string content)
        {
            content = content.Substring(1, content.Length - 2); // 去除前后空格
            return content.Split('#');
        }

        #region Check Method

        /// <summary>
        /// 检测多参数的格式，必须为左右括号开头
        /// </summary>
        /// <param name="content"></param>
        protected void AssertMultiParamFormat(string content) 
        {
            DebugUtil.Assert(content.StartsWith("(") && content.EndsWith(")"), $"{configTypeName} 类型的值不是以左括号开始右括号结束，当前为 \"{content}\"");
        }

        /// <summary>
        /// 检测参数数量限制
        /// </summary>
        /// <param name="content"></param>
        /// <param name="count"></param>
        protected void AssertParamCount(string content, int count) 
        {
            string[] strs = ParseMultiParam(content);
            DebugUtil.Assert(strs.Length == count, $"{configTypeName} 类型长度必须为 {count}，当前为\"{content}\"，{strs.Length} != {count}");
        }

        /// <summary>
        /// 检测参数数量限制，允许多个
        /// </summary>
        /// <param name="content"></param>
        /// <param name="count"></param>
        protected void AssertParamCount(string content, int[] counts)
        {
            string[] strs = ParseMultiParam(content);
            DebugUtil.Assert(counts.Contains(strs.Length), $"{configTypeName} 类型长度必须为 {string.Join(",", counts)}，当前为\"{content}\"，{strs.Length} != {string.Join(",", counts)}");
        }

        /// <summary>
        /// 检查所有参数类型，所有的参数类型一致
        /// </summary>
        /// <param name="content"></param>
        /// <param name="type"></param>
        protected void AssertParamsType(string content, Type type)
        {
            string[] strs = ParseMultiParam(content);
            foreach (var str in strs)
                AssertParamType(str, type);
        }

        /// <summary>
        /// 检查所有参数类型，参数类型不一致
        /// </summary>
        /// <param name="content"></param>
        /// <param name="types"></param>
        protected void AssertParamsType(string content, Type[] types)
        {
            string[] strs = ParseMultiParam(content);
            DebugUtil.Assert(strs.Length == types.Length, $"配置长度与类型长度不一致, 当前为 \"{content}\", {strs.Length} != {types.Length}");
            for (int i = 0; i < strs.Length; i++)
            {
                var str = strs[i];
                var type = types[i];
                AssertParamType(str, type);
            }
        }

        /// <summary>
        /// 检查单个参数类型
        /// </summary>
        /// <param name="content"></param>
        /// <param name="type"></param>
        protected void AssertParamType(string content, Type type)
        {
            bool result = true;

            if (type == typeof(byte))
            {
                result = byte.TryParse(content, out var value);
            }
            else if (type == typeof(int))
            {
                result = int.TryParse(content, out var value);
            }
            else if (type == typeof(uint))
            {
                result = uint.TryParse(content, out var value);
            }
            else if (type == typeof(long))
            {
                result = long.TryParse(content, out var value);
            }
            else if (type == typeof(ulong))
            {
                result = ulong.TryParse(content, out var value);
            }
            else if (type == typeof(short))
            {
                result = short.TryParse(content, out var value);
            }
            else if (type == typeof(ushort))
            {
                result = ushort.TryParse(content, out var value);
            }
            else if (type == typeof(float))
            {
                result = float.TryParse(content, out var value);
            }

            DebugUtil.Assert(result, $"{content} 无法解析为类型 {type.Name}");
        }

        #endregion
    }

    public abstract class ConfigType<T> : ConfigType
    {
        public override string typeName => typeof(T).Name;

        public override string writeByteTypeName => typeName;

        public override void CheckConfigFormat(string content)
        {
            AssertParamType(content, typeof(T));
        }
    }
}

