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
        /// ���ñ��ļ���
        /// </summary>
        public Object ConfigFolder;

        /// <summary>
        /// ���������ļ���
        /// </summary>
        public Object GenerateCodeFolder;

        /// <summary>
        /// �������ɶ������ļ���
        /// </summary>
        public Object GenerateBinFolder;

        public string ConfigPath
        {
            get 
            {
                DebugUtil.Assert(ConfigFolder != null, $"{AssetDatabase.GetAssetPath(this)} ��ȱ�� {nameof(ConfigFolder)} ���ã��������ļ���·��");
                return AssetDatabase.GetAssetPath(ConfigFolder);
            }
        } 

        /// <summary>
        /// ���ô�������·��
        /// </summary>
        public string GenerateCodePath
        {
            get
            {
                DebugUtil.Assert(GenerateCodeFolder != null, $"{AssetDatabase.GetAssetPath(this)} ��ȱ�� {nameof(GenerateCodeFolder)}  ���ã�����������ļ���·��");
                return AssetDatabase.GetAssetPath(GenerateCodeFolder);
            }
        }

        /// <summary>
        /// ���õ���������·��
        /// </summary>
        public string GenerateBinPath
        {
            get
            {
                DebugUtil.Assert(GenerateBinFolder != null, $"{AssetDatabase.GetAssetPath(this)} ��ȱ�� {nameof(GenerateBinFolder)}  ���ã��赼���������ļ���·��");
                return AssetDatabase.GetAssetPath(GenerateBinFolder);
            }
        }

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