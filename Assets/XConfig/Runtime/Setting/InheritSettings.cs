using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace XConfig
{
    [CreateAssetMenu(fileName = nameof(InheritSettings), menuName = "XConfig/InheritSettings", order = 100)]
    public class InheritSettings : ScriptableObject
    {
        [Serializable]
        public class InheritInfo 
        {
            public Object parent;
            public Object[] children;
        }

        public InheritInfo[] inheritInfos = new InheritInfo[] { };

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