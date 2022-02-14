using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace XConfig
{
    /// <summary>
    /// TODO
    /// </summary>
    public interface ICheckTableRowExportTime
    {
        /// <summary>
        /// 只限XRow的子类继承此接口
        /// 生成配置时会被调用到，时机是在所有表都导出完之后才进行的合法性检测
        /// 只会在editor模式下执行，时机在CheckTableInExportTime之前
        /// </summary>
        void CheckRowInExportTime();
    }

    public class XTable
    {
        static public System.Text.Encoding ENCODING;
        static public char[] SEPARATOR = { '\t' };

        public string name;
        public string keys;
        public string comments;
        public string types;
        public string flags;

        virtual public void FromBytes(BytesBuffer buffer)
        {
        }
        virtual public void InitRows()
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