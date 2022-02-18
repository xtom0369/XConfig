using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace XConfig
{
    public class XTable
    {
        public string name;
        public string keys;
        public string comments;
        public string types;
        public string flags;

        virtual public void FromBytes(BytesBuffer buffer)
        {
        }
        virtual public void Init()
        {
        }
        virtual public void OnInit()
        {
        }
        virtual public void OnAfterInit()
        {
        }
        virtual public void OnCheckWhenExport()
        {
        }
    }
}