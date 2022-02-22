using System;

namespace XConfig
{
    public interface IConfigType 
    {
        /// <summary>
        /// 原始类型名，如Int32, String，List<ItemsRow>，ItemsRow等
        /// </summary>
        string TypeName { get; }

        /// <summary>
        /// 配置类型名，如int, bool，List<items>，items等
        /// </summary>
        string ConfigTypeName { get; }

        /// <summary>
        /// 派生ConfigType的类型名，比如BoolType，IntType，因为引用类型的类型为ReferenceType<>，不是ReferenceType
        /// </summary>
        string ReadByteClassName { get; }

        /// <summary>
        /// 存储二进制时的类型名，如果是列表项则为单项存储类型
        /// </summary>
        string WriteByteTypeName { get; }

        /// <summary>
        /// 默认值，用于导出时的填充类型默认值
        /// </summary>
        string DefaultValue { get; }

        /// <summary>
        /// 是否为枚举类型
        /// </summary>
        bool IsEnum { get; }

        /// <summary>
        /// 是否为列表类型
        /// </summary>
        bool IsList { get; }

        /// <summary>
        /// 是否为引用类型
        /// </summary>
        bool IsReference { get; }

        /// <summary>
        /// 解析默认值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        string ParseDefaultValue(string value);

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
        bool CheckConfigFormat(string content, out string error);

        /// <summary>
        /// 用户根据自身需求，对类型解析出来的数据，做合法性检查
        /// 此接口只会在配置表导出阶段被执行
        /// </summary>
        bool CheckConfigValid(out string erroe);
    }
}

