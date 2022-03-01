using System;
using System.Reflection;
using UnityEngine;

namespace XConfig
{
    public abstract class XRow<TKey> : XRow 
    {
        public abstract TKey mainKey1 { get; }
    }

    public abstract class XRow<TKey1, TKey2> : XRow
    {
        public abstract TKey1 mainKey1 { get; }
        public abstract TKey2 mainKey2 { get; }
    }


    public abstract class XRow
    {
        public abstract void ReadFromBytes(BytesBuffer buffer);

        public virtual void OnAfterInit() { }

        public virtual void OnCheckWhenExport() { }

        public override string ToString()
        {
            var fileds = this.GetType().GetProperties();
            string result = this.GetType().Name + "\n";
            foreach (var field in fileds)
                result += $"{field.Name} = {field.GetValue(this)}\n";
            return result;
        }
    }
}