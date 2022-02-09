using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace XConfig
{
    [CreateAssetMenu(fileName = nameof(InheritSettings), menuName = "XConfig/InheritSettings", order = 100)]
    public class InheritSettings : ScriptableObject
    {
        [Serializable]
        public class InheritInfo 
        {
            public string parent;
            public string[] children;
        }

        public InheritInfo[] infos = new InheritInfo[] { };

        static InheritSettings _inst;
        public static InheritSettings Inst
        {
            get
            {
                if (_inst != null) return _inst;

                _inst = Resources.Load<InheritSettings>(nameof(InheritSettings));
                if (_inst == null)
                    _inst = ScriptableObject.CreateInstance<InheritSettings>();

                return _inst;
            }
        }

        
    }
}