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
        /// ԭ����·��
        /// </summary>
        public string CONFIG_PATH = "config/";

        /// <summary>
        /// ���ô�������·��
        /// </summary>
        public string GENERATE_CODE_PATH = "Assets/XConfig/GenerateCode/";

        /// <summary>
        /// ���õ���������·��
        /// </summary>
        public string CONFIG_BYTES_OUTPUT_PATH = "Assets/XConfig/GenerateBin/";

        /// <summary>
        /// �Զ����ļ���׺��֧�ֶ��ֺ�׺
        /// </summary>
        public string[] FilePatterns = { "*.bytes" };

        /// <summary>
        /// �Զ����ļ���׺��֧�ֶ��ֺ�׺
        /// </summary>
        [Tooltip("�ų��ļ�")]
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
        /// �Ƿ��ų��ļ�
        /// </summary>
        /// <param name="fileNameWithoutExtension"></param>
        /// <returns></returns>
        public bool IsFileExclude(string fileNameWithoutExtension) 
        {
            return ExcludeFile.Contains(fileNameWithoutExtension);
        }
    }
}