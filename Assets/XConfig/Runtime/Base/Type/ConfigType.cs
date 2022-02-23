﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace XConfig
{
    public abstract class ConfigType : IConfigType
    {
        public abstract string TypeName { get; }

        public virtual string ConfigTypeName => TypeName;

        public virtual string ReadByteClassName => _type.Name;

        public abstract string WriteByteTypeName { get; }

        /// <summary>
        /// 默认值，当配置字段没有设置默认值时，取DefaultValue为默认值
        /// </summary>
        public abstract string DefaultValue { get; }

        public virtual bool IsEnum => _type.BaseType == typeof(EnumType);

        public virtual bool IsList => _type.BaseType == typeof(ListType);

        public virtual bool IsReference => _type.BaseType == typeof(ReferenceType);


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
        public abstract bool CheckConfigFormat(string content, out string error);

        /// <summary>
        /// 用户根据自身需求，对类型解析出来的数据，做合法性检查
        /// 此接口只会在配置表导出阶段被执行
        /// </summary>
        public virtual bool CheckConfigValid(out string error)
        {
            error = string.Empty;
            return true;
        }

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
            Assembly assembly = Assembly.Load("Assembly-CSharp");
            Type[] types = assembly.GetTypes();

            // 所有支持的数据类型名字 => ConfigType，比如bool => BoolType
            var configTypes = types.Where(t => t.GetInterface(nameof(IConfigType)) != null && !t.IsAbstract && !t.IsGenericType)
                .Select(t => Activator.CreateInstance(t) as IConfigType);
            _configTypeDic = configTypes.ToDictionary(x => x.ConfigTypeName, x => x);

            foreach (var configType in configTypes) 
            {
                Type t = typeof(ListType<>).MakeGenericType(configType.GetType());
                var inst = Activator.CreateInstance(t, configType) as IConfigType;
                _configTypeDic.Add(inst.ConfigTypeName, inst);
            }

            // 所有支持的数据类型别名 => ConfigType，比如Boolean => BoolType
            foreach (var inst in configTypes)
                _configTypeDic[inst.TypeName] = inst;

            // 所有支持的枚举名 => ConfigType，比如FlagType => EnumType<FlagType>
            var enumTypes = types.Where(t => t.IsEnum);
            foreach (var enumType in enumTypes)
            {
                Type t = typeof(EnumType<>).MakeGenericType(enumType);
                var inst = Activator.CreateInstance(t) as IConfigType;
                _configTypeDic.Add(inst.ConfigTypeName, inst);

                t = typeof(ListType<>).MakeGenericType(t);
                inst = Activator.CreateInstance(t, inst) as IConfigType;
                _configTypeDic.Add(inst.ConfigTypeName, inst);
            }

            // 所有引用类型名 => ConfigType，比如ItemsRow => ReferenceType<ItemsRow>
            var rowTypes = types.Where(t => typeof(XRow).IsAssignableFrom(t) && !t.IsAbstract);
            foreach (var rowType in rowTypes)
            {
                Type t = typeof(ReferenceType<>).MakeGenericType(rowType);
                var inst = Activator.CreateInstance(t) as IConfigType;
                _configTypeDic.Add(inst.ConfigTypeName, inst); 

                t = typeof(ListType<>).MakeGenericType(t);
                inst = Activator.CreateInstance(t, inst) as IConfigType;
                _configTypeDic.Add(inst.ConfigTypeName, inst);
            }
        }

        /// <summary>
        /// 解析Vector格式
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        protected string[] ParseMultiContent(string content)
        {
            content = content.Substring(1, content.Length - 2); // 去除前后空格
            return content.Split('#');
        }
    }

    public abstract class ConfigType<T> : ConfigType
    {
        public override string TypeName => typeof(T).Name;

        public override string WriteByteTypeName => TypeName;
    }
}

