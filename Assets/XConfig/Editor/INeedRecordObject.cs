public interface INeedRecordObject
{
    IRecord ToRecord();
    bool isChange { get; set; }
    bool IsChanged(IRecord record);
    string ToString();
}
