using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class PathUtil
{
/*
    PC：
    Application.dataPath : /Assets
    Application.streamingAssetsPath : /Assets/StreamingAssets
    Application.persistentDataPath : C:/Users/xxxx/AppData/LocalLow/CompanyName/ProductName
    Application.temporaryCachePath : C:/Users/xxxx/AppData/Local/Temp/CompanyName/ProductName

    Android:
    Application.dataPath : /data/app/xxx.xxx.xxx.apk
    Application.streamingAssetsPath : jar:file:///data/app/xxx.xxx.xxx.apk/!/assets
    Application.persistentDataPath : /data/data/xxx.xxx.xxx/files
    Application.temporaryCachePath : /data/data/xxx.xxx.xxx/cache

    IOS：
    Application.dataPath : Application/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx/xxx.app/Data
    Application.streamingAssetsPath : Application/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx/xxx.app/Data/Raw
    Application.persistentDataPath : Application/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx/Documents
    Application.temporaryCachePath : Application/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx/Library/Caches

    Mac:
    Application.dataPath : /Assets
    Application.streamingAssetsPath : /Assets/StreamingAssets
    Application.persistentDataPath : /Users/xxxx/Library/Caches/CompanyName/Product Name
    Application.temporaryCachePath : /var/folders/57/6b4_9w8113x2fsmzx_yhrhvh0000gn/T/CompanyName/Product Name
*/
    //热更新资源根目录，包括资源列表
    static string _hotfixRootPath;
    static public string hotfixRootPath
    {
        get
        {
            if (_hotfixRootPath == null)
                _hotfixRootPath = Path.Combine(PathUtil.GetPersistentDataPath(), "update");
            return _hotfixRootPath;
        }
    }
    //热更新资源目录
    static string _hotfixResourcesPath;
    static public string hotfixResourcesPath {
        get
        {
            if (_hotfixResourcesPath == null)
                _hotfixResourcesPath = Path.Combine(PathUtil.GetPersistentDataPath(), Path.Combine("update", platform));
            return _hotfixResourcesPath;
        } }
    //原资源目录
    static string _streamingAssetsResourcesPath;
    static public string streamingAssetsResourcesPath {
        get
        {
            if (_streamingAssetsResourcesPath == null)
                _streamingAssetsResourcesPath = Path.Combine(Application.streamingAssetsPath, platform);
            return _streamingAssetsResourcesPath;
        } }
    //运行平台，"windows""mac" "android" "ios"
    static string _platform;
    static public string platform {
        get {
            if (_platform == null)
                _platform = PathUtil.GetRuntimePlatform();
            return _platform;
        } }
    //此目录不再用于读取AB，仅供设置资源导出路径，获取资源路径统一使用FileUtil.GetResourcesPath方法
    static string _assetbundleOutPutDirectory;
    static public string assetbundleDirectory {
        get
        {
            if (_assetbundleOutPutDirectory == null)
                _assetbundleOutPutDirectory = Path.Combine(Application.streamingAssetsPath, platform);
            return _assetbundleOutPutDirectory;
        } }

    static public void SetAssetbundleOutPutDirectory(string path)
    {
        _assetbundleOutPutDirectory = Path.Combine(path, platform);
    }

    /// <summary>
    /// 传入相对于存档目录的子路径，返回全路径
    /// 存档目录针对不同运行平台，位置不一样，存档目录是可读写的
    /// PC：Application.dataPath : /Assets/../
    /// Mac：Application.dataPath : /Assets/../
    /// ANDROID：Application.persistentDataPath : /data/data/xxx.xxx.xxx/files
    /// IOS：Application.persistentDataPath : Application/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx/Documents
    /// </summary>
    /// <param name="fileSubPath">传入子路径，譬如 logs/game.log</param>
    /// <returns></returns>
    static public string GetArchiveFullPath(string fileSubPath)
    {
#if SERVER_ENABLE
        return fileSubPath;
#elif UNITY_EDITOR || UNITY_STANDALONE
        return Path.Combine(Application.dataPath.Replace("/Assets", ""), fileSubPath);
#else
        return string.Format("{0}/{1}", PathUtil.GetPersistentDataPath(), fileSubPath);
#endif
    }

    /// <summary>
    /// 用于区分服务器路径
    /// </summary>
    /// <param name="fileSubPath"></param>
    /// <returns></returns>
    static public string GetActualPath(string fileSubPath)
    {
#if SERVER_ENABLE
        fileSubPath = $"../../../../client/{fileSubPath}";
#endif
        return fileSubPath;
    }

    static string _persistentDataPath = null;
    static public string GetPersistentDataPath()
    {
        if (string.IsNullOrEmpty(_persistentDataPath))
        {
            if (IsDirectoryWritable(Application.persistentDataPath))
                _persistentDataPath = Application.persistentDataPath;
            else
                DebugUtil.Assert(false, "{0} 目录没有写权限", Application.persistentDataPath);
        }
        return _persistentDataPath;
    }
    public static string GetRuntimePlatform()
    {
        string platform = null;
#if UNITY_EDITOR
        switch (UnityEditor.EditorUserBuildSettings.activeBuildTarget)
        {
            case UnityEditor.BuildTarget.StandaloneWindows:
            case UnityEditor.BuildTarget.StandaloneWindows64:
                platform = "windows";
                break;

            case UnityEditor.BuildTarget.StandaloneOSX:
                platform = "mac";
                break;

            case UnityEditor.BuildTarget.Android:
                platform = "android";
                break;

            case UnityEditor.BuildTarget.iOS:
                platform = "ios";
                break;

            default:
#if ASSERT_ENABLE
                DebugUtil.Assert(false);
#endif
                break;
        }
#else
                switch (Application.platform)
		{
			case RuntimePlatform.WindowsPlayer:
				platform = "windows";
				break;

			case RuntimePlatform.OSXPlayer:
                platform = "mac";
				break;

			case RuntimePlatform.Android:
                platform = "android";
				break;

			case RuntimePlatform.IPhonePlayer:
                platform = "ios";
				break;

			default:
                DebugUtil.Assert(false);
				break;
		}
#endif
        return platform;
    }
    static bool IsDirectoryWritable(string path)
    {
        try
        {
            if (!Directory.Exists(path)) return false;
            string file = Path.Combine(path, Path.GetRandomFileName());
            using (FileStream fileStream = File.Create(file, 1))
            {
            }
            File.Delete(file);
            return true;
        }
        catch
        {
#if LOG_ENABLE
            DebugUtil.Log(LogMask.COMMON, "PathUtil.IsDirectoryWritable=false");
#endif
            return false;
        }
    }
}
