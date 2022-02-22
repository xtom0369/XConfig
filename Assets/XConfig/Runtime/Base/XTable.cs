using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace XConfig
{
    public abstract class XTable<TKey, TRow> : XTable where TRow : XRow, new()
    {
        public List<TRow> rows { get { return _rows; } }
        protected List<TRow> _rows;
        protected Dictionary<TKey, TRow> _mainKey2Row;

        public override void ReadFromBytes(BytesBuffer buffer)
        {
            if (_rows == null) _rows = new List<TRow>();
            var newCount = buffer.ReadUInt16();
            var oldCount = _rows.Count;

            for (int i = 0; i < newCount; i++)
            {
                TRow row;
                if (i >= oldCount) { row = new TRow(); _rows.Add(row); }
                else row = _rows[i];
                row.ReadFromBytes(buffer);
            }
        }

        public virtual TRow GetRow(TKey mainKey)
        {
            if (TryGetRow(mainKey, out var row)) return row;
            DebugUtil.Assert(row != null, "{0} 找不到指定主键为 {1} 的行，请尝试重新导出配置！", name, mainKey);
            return null;
        }

        public virtual bool TryGetRow(TKey mainKey, out TRow row) 
        { 
            return _mainKey2Row.TryGetValue(mainKey, out row); 
        }

        public bool ContainsKey(TKey mainKey) 
        { 
            return _mainKey2Row.ContainsKey(mainKey); 
        }
    }

    public abstract class XTable<TKey1, TKey2, TRow> : XTable where TRow : XRow, new()
    {
        public List<TRow> rows { get { return _rows; } }
        protected List<TRow> _rows;
        protected Dictionary<int, List<MasterLevelRow>> _firstKey2Rows;
        protected Dictionary<TKey1, Dictionary<TKey2, TRow>> _mainKey2Row;

        public override void ReadFromBytes(BytesBuffer buffer)
        {
            if (_rows == null) _rows = new List<TRow>();
            var newCount = buffer.ReadUInt16();
            var oldCount = _rows.Count;

            for (int i = 0; i < newCount; i++)
            {
                TRow row;
                if (i >= oldCount) { row = new TRow(); _rows.Add(row); }
                else row = _rows[i];
                row.ReadFromBytes(buffer);
            }
        }

        public virtual List<MasterLevelRow> GetRows(TKey1 key1)
        {
            if (!_mainKey2Row.TryGetValue(key1, out var secondKey2Row))
                return null;

            if (_firstKey2Rows == null) _firstKey2Rows = new Dictionary<int, List<MasterLevelRow>>();


            if (TryGetRow(mainKey, out var row)) return row;
            DebugUtil.Assert(row != null, $"{name} 找不到指定主键为 {key1} {key2} 的行，请尝试重新导出配置！");
            return null;
        }

        public virtual TRow GetRow(TKey1 key1, TKey2 key2)
        {
            if (TryGetRow(key1, key2, out var row)) return row;
            DebugUtil.Assert(row != null, $"{name} 找不到指定主键为 {key1} {key2} 的行，请尝试重新导出配置！");
            return null;
        }

        public virtual bool TryGetRow(TKey1 key1, TKey2 key2, out TRow row) 
        {
            row = null;
            if (!_mainKey2Row.TryGetValue(key1, out var secondKey2Row))
                return false;

            return secondKey2Row.TryGetValue(key2, out row);
        }

        public virtual bool TryGetRow(TKey1 key1, out List<TRow> rows)
        {
            rows = null;
            if (!_mainKey2Row.TryGetValue(key1, out var secondKey2Row))
                return false;

            return secondKey2Row.TryGetValue(key2, out rows);
        }

        public bool ContainsKey(TKey1 key1, TKey2 key2) 
        {
            if (!_mainKey2Row.TryGetValue(key1, out var secondKey2Row))
                return false;

            return secondKey2Row.ContainsKey(key2); 
        }
    }

    public abstract class XTable
    {
        public string name;

        virtual public void ReadFromBytes(BytesBuffer buffer)
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