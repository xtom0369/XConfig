using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Networking;
using System;

public class FileUtil
{
    /// <summary>
    /// 此接口适用读取的文件处于只读目录下或可读写目录
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string ReadAllText(string path)
    {
        //编辑器下可以直接用File.ReadAllText，IOS下也可以，但是android不行，得用AssetBundle.LoadFromFile或WWW,这里使用www
#if UNITY_ANDROID && !UNITY_EDITOR
        if (path.IndexOf("file://") == -1)
                path = string.Format("file://{0}", path);
        using (WWW www = new WWW(path))
        {
            while (!www.isDone)
                System.Threading.Thread.Sleep(1);
            return www.text;
        }
#else
        if (File.Exists(path))
            return File.ReadAllText(path);
        return "";
#endif
    }
    /// <summary>
    /// 此接口适用读取的文件处于只读目录下或可读写目录
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static byte[] ReadAllBytes(string path)
    {
        //编辑器下可以直接用File.ReadAllText，IOS下也可以，但是android不行，得用AssetBundle.LoadFromFile或WWW,这里使用www
#if UNITY_ANDROID && !UNITY_EDITOR
        if (path.IndexOf("file://") == -1)
                path = string.Format("file://{0}", path);
        using (WWW www = new WWW(path))
        {
            while (!www.isDone)
                System.Threading.Thread.Sleep(1);
            return www.bytes;
        }
#else
        if (File.Exists(path))
            return File.ReadAllBytes(path);
        return null;
#endif
    }
    /// <summary>
    /// 可以指定多个匹配模式获取文件列表
    /// </summary>
    /// <param name="folder">目录绝对路径，获取绝对路径的方式通过PathUtil.GetCrossPlatformFullPath获得</param>
    /// <param name="searchPatterns"></param>
    /// <param name="isRecursionDirectory"></param>
    /// <returns></returns>
    public static List<FileInfo> GetFiles(DirectoryInfo folder, string[] searchPatterns, bool isRecursionDirectory = true)
    {
        List<FileInfo> files = new List<FileInfo>();
        SearchOption option = isRecursionDirectory ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        for (int i = 0; i < searchPatterns.Length; i++)
        {
            FileInfo[] fileInfos = folder.GetFiles(searchPatterns[i], option);
            files.AddRange(fileInfos);
        }
        return files;
    }
    //获取文件大小，以B为单位，注意，如果文件位于手机上的只读目录下，FileInfo不可用
    static public long GetFileSize(string filePath)
    {
        using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))  // 不会锁死, 允许其它程序打开
        {
            return fileStream.Length;
        }
    }
    //以相对路径获取资源的绝对路径
    //此接口会先检查该文件在热更目录是否存在，然后检查streamingAssets目录
    static public string GetResourcesPath(string relativeName)
    {
        string fullName = Path.Combine(PathUtil.hotfixResourcesPath, relativeName);
        if (!File.Exists(fullName))
        {
            fullName = Path.Combine(Application.streamingAssetsPath, relativeName);
#if UNITY_ANDROID && !UNITY_EDITOR
            using (UnityWebRequest request = new UnityWebRequest(fullName))
            {
                request.SendWebRequest();
                while (!request.isDone)
                    System.Threading.Thread.Sleep(1);
                if (request.responseCode == 404)
                {
                    DebugUtil.LogWarning(LogMask.COMMON, "没找到资源 {0}", relativeName);
                    return null;
                }
            }
#else

            if (!File.Exists(fullName))
            {
                DebugUtil.LogWarning(LogMask.COMMON, "没找到资源 {0}", relativeName);
                return null;
            }
#endif
        }

        return fullName;
    }
    static public bool FileExists(string relativeName)
    {
        string fullName = Path.Combine(PathUtil.hotfixResourcesPath, relativeName);
        if (!File.Exists(fullName))
        {
            fullName = Path.Combine(PathUtil.streamingAssetsResourcesPath, relativeName);
#if UNITY_ANDROID && !UNITY_EDITOR
            using (UnityWebRequest request = new UnityWebRequest(fullName))
            {
                request.SendWebRequest();
                while (!request.isDone)
                    System.Threading.Thread.Sleep(1);
                if (request.responseCode == 404)
                    return false;
            }
#else
            if (!File.Exists(fullName))
                return false;
#endif
        }
        return true;
    }

    /// 复制文件夹（及文件夹下所有子文件夹和文件）
    /// </summary>
    /// <param name="sourcePath">待复制的文件夹路径</param>
    /// <param name="destinationPath">目标路径</param>
    public static void CopyDirectory(String sourcePath, String destinationPath)
    {
        DirectoryInfo info = new DirectoryInfo(sourcePath);
        Directory.CreateDirectory(destinationPath);
        foreach (FileSystemInfo fsi in info.GetFileSystemInfos())
        {
            String destName = Path.Combine(destinationPath, fsi.Name);

            if (fsi is System.IO.FileInfo)          //如果是文件，复制文件
            {
                File.Copy(fsi.FullName, destName);
            }
            else                                    //如果是文件夹，新建文件夹，递归
            {
                Directory.CreateDirectory(destName);
                CopyDirectory(fsi.FullName, destName);
            }
        }
    }
}
