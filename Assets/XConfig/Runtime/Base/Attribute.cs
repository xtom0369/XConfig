using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using XConfig;

namespace XConfig 
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class, AllowMultiple = false)]
    public class BindConfigFileNameAttribute : Attribute
    {
        public string configName;
        public BindConfigFileNameAttribute(string configName)
        {
            this.configName = configName;
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
