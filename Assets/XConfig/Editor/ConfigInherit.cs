using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using XConfig;

namespace XConfig.Editor 
{
    public class ConfigInherit
    {
        public static ConfigInherit Inst => _inst ?? (_inst = new ConfigInherit());
        static ConfigInherit _inst;

        static Dictionary<string, string> _child2ParentDic = new Dictionary<string, string>();
        static Dictionary<string, byte> _parentDic = new Dictionary<string, byte>(); // 用于记录父表，检测多级继承

        public ConfigInherit() 
        {
            Init();
        }

        void Init()
        {
            foreach (var info in InheritSettings.Inst.inheritInfos) 
            {
                if (info.parent == null) continue;
                string parentName = info.parent.name;
                DebugUtil.Assert(!_parentDic.ContainsKey(parentName), $"存在多级继承，{parentName}既为父表，也为子表");
                _parentDic.Add(parentName, 1);

                foreach (var child in info.children) 
                {
                    if (child == null) continue;
                    string childName = child.name;
                    if (_child2ParentDic.TryGetValue(childName, out var parent))
                        DebugUtil.Assert(!_child2ParentDic.ContainsKey(childName), $"{childName} 只能存在一个父表，当前存在多个，{parentName} 和 {parent}");

                    _child2ParentDic.Add(childName, parentName);
                }
            }
        }

        /// <summary>
        /// 尝试获取父表名
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public bool TryGetParent(string fileName, out string parent)
        {
            return _child2ParentDic.TryGetValue(fileName, out parent);
        }

        public bool TryGetChildren(string fileName, out List<string> children)
        {
            children = _child2ParentDic.Where(kvp => kvp.Value == fileName).Select(kvp => kvp.Key).ToList();
            return children.Count > 0;
        }

        /// <summary>
        /// 是否为父表
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool IsParent(string fileName) 
        {
            return _parentDic.ContainsKey(fileName);
        }
    }
}
