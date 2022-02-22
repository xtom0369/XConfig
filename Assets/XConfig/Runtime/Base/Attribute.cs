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
        INT,//单一整型
        STRING,//单一字串
        INT_INT,//两个整型
        OTHER,//其它情况
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class, AllowMultiple = false)]
    public class BindConfigPathAttribute : Attribute
    {
        public string configName;
        public BindConfigPathAttribute(string configName)
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
