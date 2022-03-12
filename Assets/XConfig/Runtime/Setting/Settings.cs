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
        /// ���ñ��ļ���
        /// </summary>
        public string ConfigPath;

        /// <summary>
        /// ���������ļ���
        /// </summary>
        public string GenerateCodePath;

        /// <summary>
        /// ���õ���������·��
        /// </summary>
        public string GenerateBinPath;

        /// <summary>
        /// �Զ����ļ���׺��֧�ֶ��ֺ�׺
        /// </summary>
        public string[] SourceFilePatterns = { "*.bytes" };

        /// <summary>
        /// �Զ����ļ�������׺
        /// </summary>
        public string OutputFileExtend = "bytes";

        /// <summary>
        /// �Զ����ļ���׺��֧�ֶ��ֺ�׺
        /// </summary>
        [Tooltip("�ų��ļ�")]
        public string[] ExcludeFile = { };
        
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