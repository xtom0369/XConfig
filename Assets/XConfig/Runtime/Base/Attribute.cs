using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using XConfig;

namespace XConfig 
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class)]
    public class BindConfigFileNameAttribute : Attribute
    {
        public string configName;
        public bool isParent;
        public BindConfigFileNameAttribute(string configName, bool isParent = false)
        {
            this.configName = configName;
            this.isParent = isParent;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class ConfigReferenceAttribute : Attribute
    {
        public string property;
        public ConfigReferenceAttribute(string property)
        {
            this.property = property;
        }
    }
}
