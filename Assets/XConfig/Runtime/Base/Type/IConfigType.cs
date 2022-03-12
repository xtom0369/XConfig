using System;

namespace XConfig
{
    public interface IConfigType 
    {
        /// <summary>
        /// 原始类型名，如Int32, String，List<ItemsRow>，ItemsRow等
        /// </summary>
        string typeName { get; }

        /// <summary>
        /// 配置类型名，如int, bool，List<items>，items等
        /// </summary>
        string configTypeName { get; }

        /// <summary>
        /// 派生ConfigType的类型名，比如BoolType，IntType，引用类型的类型为ReferenceType<>，不是ReferenceType
        /// </summary>
        string readByteClassName { get; }

        /// <summary>
        /// 存储二进制时的类型名，如果是列表项则为单项存储类型
        /// </summary>
        string writeByteTypeName { get; }

        /// <summary>
        /// 默认值，用于导出时的填充类型默认值
        /// </summary>
        string defaultValue { get; }

        /// <summary>
        /// 是否为枚举类型
        /// </summary>
        bool isEnum { get; }

        /// <summary>
        /// 是否为列表类型
        /// </summary>
        bool isList { get; }

        /// <summary>
        /// 是否为引用类型
        /// </summary>
        bool isReference { get; }

        /// <summary>
        /// 引用文件，对应的配置表名
        /// </summary>
        string referenceFileName { get; }

        /// <summary>
        /// 解析默认值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        string ParseDefaultValue(string value);

        /// <summary>
        /// 解析表key值，引用类型后需要加Id，引用且为列表时加Ids
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string ParseKeyName(string key);

        /// <summary>
        /// 将当前实例中的数据，写入字节缓冲区中
        /// </summary>
        /// <param name="buffer">字节缓冲区</param>
        void WriteToBytes(BytesBuffer buffer, string value);

        /// <summary>
        /// 格式合法性检查
        /// </summary>
        /// <param name="content">配置表中配置的字符串</param>
        /// </summary>
        void CheckConfigFormat(string content);

        /// <summary>
        /// 根据自身需求，对类型解析出来的数据，做合法性检查
        /// 此接口只会在配置表导出阶段被执行
        /// </summary>
        void CheckConfigValid(IConfigType configType);
    }
}

