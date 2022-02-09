using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace XConfig
{
    //[CreateAssetMenu(fileName = nameof(Settings), menuName = "XConfig/Setting", order = 100)]
    public class Settings : ScriptableObject
    {
        /// <summary>
        /// 原配置路径
        /// </summary>
        public string CONFIG_PATH = "config/";

        /// <summary>
        /// 配置代码生成路径
        /// </summary>
        public string GENERATE_CODE_PATH = "Assets/XConfig/GenerateCode/";

        /// <summary>
        /// 配置导出二进制路径
        /// </summary>
        public string CONFIG_BYTES_OUTPUT_PATH = "Assets/XConfig/GenerateBin/";

        /// <summary>
        /// 自定义文件后缀，支持多种后缀
        /// </summary>
        public string[] FilePatterns = { "*.bytes" };

        /// <summary>
        /// 自定义文件后缀，支持多种后缀
        /// </summary>
        [Tooltip("排除文件")]
        public string[] ExcludeFile = { };
        
        static Settings _inst;
        public static Settings Inst
        {
            get
            {
                if (_inst != null) return _inst;

                _inst = Resources.Load<Settings>(nameof(Settings));
                if (_inst == null)
                    _inst = ScriptableObject.CreateInstance<Settings>();

                return _inst;
            }
        }

        /// <summary>
        /// 是否排除文件
        /// </summary>
        /// <param name="fileNameWithoutExtension"></param>
        /// <returns></returns>
        public bool IsFileExclude(string fileNameWithoutExtension) 
        {
            return ExcludeFile.Contains(fileNameWithoutExtension);
        }
    }
}