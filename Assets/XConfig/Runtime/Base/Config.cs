using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using XConfig;

public partial class Config
{
    static public Config Inst;

    /// <summary>
    /// 自定义加载器
    /// </summary>
    public static Func<string, byte[]> customLoader { get; set; } = ReadAllBytes;

    /// <summary>
    /// 配置表热重置，游戏运行期间重新读取修改后的配置到内存
    /// </summary>
    public void HotReload()
    {
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        List<XTable> tables = new List<XTable>();
        BytesBuffer buffer = new BytesBuffer(2 * 1024);
        FieldInfo[] configFields = GetType().GetFields();
        foreach (FieldInfo tableField in configFields)
        {
            var attribute = tableField.GetCustomAttribute<BindConfigFileNameAttribute>(false);
            if (attribute != null)//排除像Inst这样的字段
            {
                string binFileName = attribute.configName;
                byte[] bytes = customLoader(binFileName);
                DebugUtil.Assert(bytes != null, "找不到二进制文件：{0}", binFileName);
                buffer.Clear();
                buffer.WriteBytes(bytes, 0, bytes.Length);
                buffer.ReadString();
                XTable tbl = tableField.GetValue(this) as XTable;
                tbl.ReadFromBytes(buffer);
                tables.Add(tbl);
            }
        }

        foreach (XTable tbl in tables)
            tbl.OnInit();

        sw.Stop();
        DebugUtil.Log($"配置表热加载成功，耗时:{(float)sw.ElapsedMilliseconds/1000:N2} 秒");
    }

    /// <summary>
    /// 调用所有配置表的Init函数
    /// isFromGenerateConfig:是否来自导出配置表时的调用
    /// </summary>
    public void Init(bool isFromGenerateConfig = false)
    {
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        List<XTable> parentTable = new List<XTable>();
        List<XTable> tables = new List<XTable>();
        BytesBuffer buffer = new BytesBuffer(2 * 1024);
        FieldInfo[] configFields = GetType().GetFields();
        foreach (FieldInfo tableField in configFields)
        {
            var attribute = tableField.GetCustomAttribute<BindConfigFileNameAttribute>(false);
            if (attribute != null)
            {
                string binFileName = attribute.configName;
                byte[] bytes = customLoader(binFileName);
                DebugUtil.Assert(bytes != null, "找不到二进制文件：{0}", binFileName);
                buffer.Clear();
                buffer.WriteBytes(bytes, 0, bytes.Length);
                buffer.ReadString();
                XTable tbl = tableField.GetValue(this) as XTable;
                tbl.name = binFileName;
                tbl.ReadFromBytes(buffer);
                tables.Add(tbl);

                if (attribute.isParent) // 父类延迟初始化
                    parentTable.Add(tbl);
                else
                    tbl.Init();
            }
        }
        // 父表需要等子表初始化完再初始化
        foreach (XTable tbl in parentTable)
            tbl.Init();
        foreach (XTable tbl in tables)
        {
            tbl.OnInit();

            if (isFromGenerateConfig)
                tbl.OnInit(); // 为了检测OnInit实现中是否造成了hotreload失效
        }
        sw.Stop();
    }

    internal static byte[] ReadAllBytes(string binFileName)
    {
        string binFilePath = Path.Combine(Settings.Inst.GenerateBinPath, $"{binFileName}.{Settings.Inst.OutputFileExtend}");
        return File.ReadAllBytes(binFilePath);
    }
}
