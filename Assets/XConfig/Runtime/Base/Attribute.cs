using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using XConfig;

namespace XConfig 
{
    //配置表主键的类型
    public enum EnumTableMainKeyType
    {
        SINGLE, // 单组件
        DOUBLE, // 双主键
        OTHER, // 其它情况
    }

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

    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigMainKeyAttribute : Attribute
    {
        public ConfigMainKeyAttribute()
        {
        }
    }
}
