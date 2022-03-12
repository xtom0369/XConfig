using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEditor;

namespace XConfig
{
    [CreateAssetMenu(fileName = nameof(Settings), menuName = "XConfig/Setting", order = 100)]
    public class Settings : ScriptableObject
    {
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
        /// 配置表文件夹
        /// </summary>
        public string ConfigPath;

        /// <summary>
        /// 代码生成文件夹
        /// </summary>
        public string GenerateCodePath;

        /// <summary>
        /// 配置导出二进制路径
        /// </summary>
        public string GenerateBinPath;

        /// <summary>
        /// 自定义文件后缀，支持多种后缀
        /// </summary>
        public string[] SourceFilePatterns = { "*.bytes" };

        /// <summary>
        /// 自定义文件导出后缀
        /// </summary>
        public string OutputFileExtend = "bytes";

        /// <summary>
        /// 自定义文件后缀，支持多种后缀
        /// </summary>
        [Tooltip("排除文件")]
        public string[] ExcludeFile = { };
        
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