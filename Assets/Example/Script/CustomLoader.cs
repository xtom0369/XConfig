using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XConfig;
using System.IO;

public class CustomLoader : MonoBehaviour 
{
    AssetBundle assetBundle { get { return _assetBundle ?? (_assetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "config"))); } }
    AssetBundle _assetBundle;

    void Start()
    {
        ConfigBase.customLoader += ReadAllBytes;
        // 配置表的初始化，只需要执行一次
        Config.Inst.Init();
    }

    void OnDestroy()
    {
        _assetBundle?.Unload(true);
    }

    internal byte[] ReadAllBytes(string binFileName)
    {
        string binFilePath = Path.Combine(Settings.Inst.GenerateBinPath, $"{binFileName}.{Settings.Inst.OutputFileExtend}");
        var asset = assetBundle.LoadAsset<TextAsset>(binFilePath);
        DebugUtil.Assert(asset != null, $"assetbundle中不存在配置表 {binFilePath}, 请尝试重新导出ab");
        return asset.bytes;
    }

    public void GetAllRows()
    {
        var rows = Config.Inst.baseTypeTable.rows;
        Debug.Log($"【GetAllRows】");
        foreach (var row in rows)
            Debug.Log(row);
    }
}
