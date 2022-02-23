using System;
using System.Collections.Generic;
using UnityEngine;
using XConfig;

namespace XConfig.Editor 
{
    public class ConfigInherit
    {
        static Dictionary<string, ConfigScheme> configSchemeDic
        {
            get 
            {
                if (_configSchemeDic == null)
                { 
                    _configSchemeDic = new Dictionary<string, ConfigScheme>();
                    Init();
                }

                return _configSchemeDic;
            }
        }
        static Dictionary<string, ConfigScheme> _configSchemeDic;


        static void Init()
        {
            configSchemeDic.Clear();
            foreach (var info in InheritSettings.Inst.inheritInfos) 
            {
                if (info.parent == null) continue;
                string parentName = info.parent.name;
                if (!configSchemeDic.TryGetValue(parentName, out var parentScheme))
                {
                    parentScheme = new ConfigScheme() { configName = parentName };
                    configSchemeDic.Add(parentName, parentScheme);
                }

                parentScheme.childSchemes = new List<ConfigScheme>();
                foreach (var child in info.children) 
                {
                    if (child == null) continue;
                    string childName = child.name;
                    if (!configSchemeDic.TryGetValue(childName, out var childScheme))
                    {
                        childScheme = new ConfigScheme() { configName = childName };
                        configSchemeDic.Add(childName, childScheme);
                    }

                    parentScheme.childSchemes.Add(childScheme);
                    childScheme.parentScheme = parentScheme;
                }
            }
        }
        /// <summary>
        /// 如果传入table属于某个继承链上的表，则返回整个继承链上表的数组，数组每一项是表的名字，譬如master_equipment.bytes
        /// 如果传入table没有继承关系，返回null
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        static public List<string> GetInheritTree(ConfigRecordInfo table)
        {
            ConfigScheme config;
            string name = table.sourceFileNameWithoutExtension;
            if (configSchemeDic.TryGetValue(name, out config))
            {
                ConfigScheme rootConfig = config.rootScheme;
                if (rootConfig != null)
                {
                    List<string> tree = new List<string>();
                    tree.Add(rootConfig.configName);
                    FindChildRecurison(rootConfig, tree);
                    return tree;
                }
            }
            return null;
        }
        /// <summary>
        /// 传入一个表名，返回父表名，譬如传入equip_weapon，返回master_equipment
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        static public string GetParentFileName(string fileName)
        {
            ConfigScheme config;
            if (configSchemeDic.TryGetValue(fileName, out config))
                return config.parentScheme != null ? config.parentScheme.configName : null;
            return null;
        }
        static void FindChildRecurison(ConfigScheme parent, List<string> tree)
        {
            if (parent.childSchemes == null) return;
            for (int i = 0; i < parent.childSchemes.Count; i++)
            {
                ConfigScheme child = parent.childSchemes[i];
                tree.Add(child.configName);
                FindChildRecurison(child, tree);
            }
        }
    }

    public class ConfigScheme
    {
        public string configName;
        public ConfigScheme parentScheme;
        public List<ConfigScheme> childSchemes;
        public ConfigScheme rootScheme
        {
            get
            {
                ConfigScheme rootConfig = parentScheme;
                while (rootConfig != null && rootConfig.parentScheme != null)
                    rootConfig = rootConfig.parentScheme;
                
                if (rootConfig == null && childSchemes != null)
                    return this;

                return rootConfig;
            }
        }
    }
}
