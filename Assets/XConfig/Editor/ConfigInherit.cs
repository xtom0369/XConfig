using System;
using System.Collections.Generic;
using UnityEngine;
using XConfig;

namespace XConfig.Editor 
{
    public class ConfigInherit
    {
        //此字典记录了所有[子表名]=》[其直接父表]的映射
        static Dictionary<string, CsvScheme> csvDic = new Dictionary<string, CsvScheme>();
        static public void Init()
        {
            csvDic.Clear();
            foreach (var info in InheritSettings.Inst.infos) 
            {
                if (!csvDic.TryGetValue(info.parent, out var parentScheme))
                {
                    parentScheme = new CsvScheme() { csv_name = info.parent };
                    csvDic.Add(info.parent, parentScheme);
                }

                parentScheme.childCsvs = new List<CsvScheme>();
                foreach (var child in info.children) 
                {
                    if (!csvDic.TryGetValue(child, out var childScheme))
                    {
                        childScheme = new CsvScheme() { csv_name = child };
                        csvDic.Add(child, childScheme);
                    }

                    parentScheme.childCsvs.Add(childScheme);
                    childScheme.parentCsv = parentScheme;
                }
            }
        }
        /// <summary>
        /// 如果传入table属于某个继承链上的表，则返回整个继承链上表的数组，数组每一项是表的名字，譬如master_equipment.bytes
        /// 如果传入table没有继承关系，返回null
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        static public List<string> GetInheritTree(NeedRecordTable table)
        {
            CsvScheme csv;
            string name = table.csvFileNameWithoutExtension;
            if (csvDic.TryGetValue(name, out csv))
            {
                CsvScheme rootCsv = csv.rootCsv;
                if (rootCsv != null)
                {
                    List<string> tree = new List<string>();
                    tree.Add(rootCsv.csv_name);
                    FindChildRecurison(rootCsv, tree);
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
            CsvScheme csv;
            if (csvDic.TryGetValue(fileName, out csv))
                return csv.parentCsv != null ? csv.parentCsv.csv_name : null;
            return null;
        }
        static void FindChildRecurison(CsvScheme parent, List<string> tree)
        {
            if (parent.childCsvs == null) return;
            for (int i = 0; i < parent.childCsvs.Count; i++)
            {
                CsvScheme child = parent.childCsvs[i];
                tree.Add(child.csv_name);
                FindChildRecurison(child, tree);
            }
        }
    }
    [Serializable]
    public class CsvScheme
    {
        public string csv_name;//形如master_equipment
        public CsvScheme parentCsv;
        public List<CsvScheme> childCsvs;
        public CsvScheme rootCsv
        {
            get
            {
                CsvScheme rootCsv = parentCsv;
                while (rootCsv != null && rootCsv.parentCsv != null)
                {
                    rootCsv = rootCsv.parentCsv;
                }
                if (rootCsv == null && childCsvs != null)//如果自己是总表的话，rootCsv会返回自己
                    return this;
                return rootCsv;
            }
        }
    }
}
