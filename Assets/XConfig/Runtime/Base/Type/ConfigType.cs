﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace XConfig
{
    public abstract class ConfigType : IConfigType
    {
        /// <summary>
        /// 类型名，如int, bool等
        /// </summary>
        public virtual string Name { get; }

        /// <summary>
        /// 默认值，当配置字段没有设置默认值时，取DefaultValue为默认值
        /// </summary>
        public abstract string DefaultValue { get; }

        
        static Dictionary<string, IConfigType> _configTypeDic;

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

        /// <summary>
        /// 解析默认值
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public virtual string ParseDefaultValueContent(string content)
        {
            return string.IsNullOrEmpty(content) ? DefaultValue : content;
        }

        public static bool TryGetConfigType(string typeName, out IConfigType type)
        {
            if (_configTypeDic == null)
            {
                Assembly assembly = Assembly.Load("Assembly-CSharp");
                _configTypeDic = assembly.GetTypes()
                    .Where(t => t.GetInterface(nameof(IConfigType)) != null && !t.IsAbstract && !t.IsGenericType)
                    .Select(t => Activator.CreateInstance(t) as IConfigType)
                    .ToDictionary(x => x.Name, x => x);

                var enumTypes = assembly.GetTypes().Where(t => t.IsEnum);
                foreach (var enumType in enumTypes) 
                {
                    Type t = typeof(EnumType<>).MakeGenericType(enumType);
                    var inst = Activator.CreateInstance(t) as IConfigType;
                    _configTypeDic.Add(inst.Name, inst); 
                }
            }

            return _configTypeDic.TryGetValue(typeName, out type);
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
}
