using UnityEngine;
using System.Collections;
using System.IO;

/// <summary>
/// 文本格式文件导入器
/// </summary>
public abstract class TextFileImporter
{
    public TextFileImporter()
    {
    }
    public abstract void Import(StreamReader reader);
}
