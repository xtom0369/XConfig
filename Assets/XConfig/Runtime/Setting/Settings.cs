using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace XConfig
{
    [CreateAssetMenu(fileName = nameof(Settings), menuName = "XConfig/Setting", order = 100)]
    public class Settings : ScriptableObject
    {
        /// <summary>
        /// 配置表文件夹
        /// </summary>
        public Object ConfigFolder;

        /// <summary>
        /// 代码生成文件夹
        /// </summary>
        public Object GenerateCodeFolder;

        /// <summary>
        /// 代码生成二进制文件夹
        /// </summary>
        public Object GenerateBinFolder;

        public string ConfigPath
        {
            get 
            {
                DebugUtil.Assert(ConfigFolder != null, $"{AssetDatabase.GetAssetPath(this)} 中缺少 {nameof(ConfigFolder)} 配置，需配置文件夹路径");
                return AssetDatabase.GetAssetPath(ConfigFolder);
            }
        } 

        /// <summary>
        /// 配置代码生成路径
        /// </summary>
        public string GenerateCodePath
        {
            get
            {
                DebugUtil.Assert(GenerateCodeFolder != null, $"{AssetDatabase.GetAssetPath(this)} 中缺少 {nameof(GenerateCodeFolder)}  配置，需代码生成文件夹路径");
                return AssetDatabase.GetAssetPath(GenerateCodeFolder);
            }
        }

        /// <summary>
        /// 配置导出二进制路径
        /// </summary>
        public string GenerateBinPath
        {
            get
            {
                DebugUtil.Assert(GenerateBinFolder != null, $"{AssetDatabase.GetAssetPath(this)} 中缺少 {nameof(GenerateBinFolder)}  配置，需导出二进制文件夹路径");
                return AssetDatabase.GetAssetPath(GenerateBinFolder);
            }
        }

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