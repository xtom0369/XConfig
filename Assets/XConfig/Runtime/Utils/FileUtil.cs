using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Networking;
using System;

namespace XConfig 
{
    public class FileUtil
    {
        public static string[] GetFiles(string folder, string[] searchPatterns, SearchOption searchOption)
        {
            List<string> files = new List<string>();
            for (int i = 0; i < searchPatterns.Length; i++)
                files.AddRange(Directory.GetFiles(folder, searchPatterns[i], searchOption));
            return files.ToArray();
        }
    }
}

