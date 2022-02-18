using System;

namespace XConfig
{
    public interface IConfigType 
    {
        /// <summary>
        /// 类型名，如int, bool等
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 解析默认值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        string ParseDefaultValueContent(string value);

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

