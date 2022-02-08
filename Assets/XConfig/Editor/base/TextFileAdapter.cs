using UnityEngine;
using System.Collections;

/// <summary>
/// Importer跟对应的Exporter的适配器
/// 可以适配多种情况
/// 1、一个Importer=》1个Exporter
/// 2、一个Importer=》多个Exporter
/// 3、多个Importer=》1个Exporter
/// 4、多个Importer=》多个Exporter
/// </summary>
public abstract class TextFileAdapter
{
    public abstract void Convert();
}
