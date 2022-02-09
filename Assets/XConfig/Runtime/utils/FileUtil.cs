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
        if (File.Exists(path))
            return File.ReadAllText(path);
        return "";
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
    public static string[] GetFiles(string folder, string[] searchPatterns, SearchOption searchOption)
    {
        List<string> files = new List<string>();
        for (int i = 0; i < searchPatterns.Length; i++)
            files.AddRange(Directory.GetFiles(folder, searchPatterns[i], searchOption));
        return files.ToArray();
    }

    public static bool IsFileInUse(string fileName)
    {
        bool isUse = true;
        FileStream fs = null;
        try
        {
            fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
            isUse = false;
        }
        catch
        {
        }
        finally
        {
            if (fs != null)
            {
                fs.Dispose();
                fs.Close();
            }
        }
        return isUse; //true表示正在使用,false没有使用  
    }
}
