
public class IUserDefineType
{
    /// <summary>
    /// 将当前实例中的数据，写入字节缓冲区中
    /// </summary>
    /// <param name="buffer">字节缓冲区</param>
    /// <author>jms</author>
    public virtual void WriteToBytes(BytesBuffer buffer)
    {}

    /// <summary>
    /// 从字节缓冲区中，读取数据写入当前实例
    /// </summary>
    /// <param name="buffer">字节缓冲区</param>
    /// <author>jms</author>
    public virtual void ReadFromBytes(BytesBuffer buffer)
    {}

    /// <summary>
    /// 用户根据自身需求，将传入的字符串，做处理后填充当前实例
    /// </summary>
    /// <param name="content">配置表中配置的字符串 例如：ItemConsume类型 对应的：(1#7)</param>
    /// <author>jms</author>
    public virtual void ReadFromString(string content)
    {}

    /// <summary>
    /// 用户根据自身需求，将当前实例的数据处理为字符串后返回
    /// </summary>
    /// <returns>返回类似配置表中配置的字符串，并且该字符串将被写入到配置表中 例如：ItemConsume类型 对应的：(1#7)</returns>
    /// <author>jms</author>
    public virtual string WriteToString()
    {
        return null;
    }

    /// <summary>
    /// 用户根据自身需求，对传入的字符串, 做格式合法性检查
    /// 此接口只会在配置表导出阶段被执行
    /// </summary>
    /// <param name="content">配置表中配置的字符串 例如：ItemConsume类型 对应的：(1#7)</param>
    /// <returns>如果有错误返回报错信息，没有错误则返回空</returns>
    /// <author>jms</author>
    public virtual string CheckConfigFormat(string content)
    {
        return null;
    }

    /// <summary>
    /// 用户根据自身需求，对类型解析出来的数据，做合法性检查
    /// 此接口只会在配置表导出阶段被执行
    /// </summary>
    /// <returns>如果有错误返回报错信息，没有错误则返回空</returns>
    /// <author>hjg</author>
    public virtual string CheckConfigValid()
    {
        return null;
    }
}

