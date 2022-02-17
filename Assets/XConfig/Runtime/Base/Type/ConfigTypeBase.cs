using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace XConfig
{
    public abstract class ConfigTypeBase
    {
        static Dictionary<string, IConfigType> _configTypeDic;
        public static bool TryGetConfigType(string typeName, out IConfigType type)
        {
            if (_configTypeDic == null)
            {
                Assembly assembly = Assembly.Load("Assembly-CSharp");
                _configTypeDic = assembly.GetTypes()
                    .Where(t => t.GetInterface(nameof(IConfigType)) != null && !t.IsAbstract)
                    .Select(t => Activator.CreateInstance(t) as IConfigType)
                    .ToDictionary(x => x.Name, x => x);
            }

            return _configTypeDic.TryGetValue(typeName, out type);
        }
    }

    public abstract class ConfigTypeBase<T> : ConfigTypeBase, IConfigType
    {
        /// <summary>
        /// 类型名，如int, bool等
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// 类型名，如int, bool等
        /// </summary>
        public abstract T DefaultValue { get; }

        /// <summary>
        /// 将当前实例中的数据，写入字节缓冲区中
        /// </summary>
        /// <param name="buffer">字节缓冲区</param>
        public abstract void WriteToBytes(BytesBuffer buffer, string value);

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
    }
}

