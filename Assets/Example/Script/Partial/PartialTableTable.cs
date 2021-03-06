//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using XConfig;

public partial class PartialTableTable
{
    Dictionary<string, byte> hasPathDic = new Dictionary<string, byte>();

    public override void OnAfterInit()
    {
        hasPathDic.Clear();
        foreach (var row in rows)
            hasPathDic[row.Path] = 1;
    }

    public override void OnCheckWhenExport()
    {
    }

    public bool IsPathUsed(string path) 
    {
        return hasPathDic.ContainsKey(path);
    }
}

public partial class PartialTableRow
{
    public override void OnAfterInit()
    {
    }

    public override void OnCheckWhenExport()
    {
        string path = System.IO.Path.Combine(Application.dataPath, Path);
        DebugUtil.Assert(File.Exists(path), $"??????·?? {path}");
    }
}
