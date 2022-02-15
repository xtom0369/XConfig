using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using XConfig;

namespace XConfig 
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class BindConfigPathAttribute : Attribute
    {
        public string csvPath;
        public BindConfigPathAttribute(string csvPath)
        {
            this.csvPath = csvPath;
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
