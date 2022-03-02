using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace XConfig
{
    /// <summary>
    /// 单主键配置表基类
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TRow"></typeparam>
    public abstract class XTable<TKey, TRow> : XTable where TRow : XRow<TKey>, new()
    {
        public List<TRow> rows { get { return _rows; } }
        protected List<TRow> _rows;
        protected Dictionary<TKey, TRow> mainKey2Row;

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

        public override void Init()
        {
            mainKey2Row = new Dictionary<TKey, TRow>();
            for (int i = 0; i < _rows.Count; i++)
            {
                TRow row = _rows[i];
                TKey mainKey = row.mainKey1;
                DebugUtil.Assert(!mainKey2Row.ContainsKey(mainKey), "{0} 主键重复：{1}，请尝试重新导出配置！", name, mainKey);
                mainKey2Row.Add(mainKey, row);
            }
        }

        public override void OnInit()
        {
            for (int i = 0; i < _rows.Count; i++)
                _rows[i].OnAfterInit();

            OnAfterInit();
        }

        public virtual TRow GetRow(TKey mainKey)
        {
            if (TryGetRow(mainKey, out var row)) return row;
            DebugUtil.Assert(row != null, "{0} 找不到指定主键为 {1} 的行，请尝试重新导出配置！", name, mainKey);
            return null;
        }

        public virtual bool TryGetRow(TKey mainKey, out TRow row) 
        { 
            return mainKey2Row.TryGetValue(mainKey, out row); 
        }

        public bool ContainsKey(TKey mainKey) 
        { 
            return mainKey2Row.ContainsKey(mainKey); 
        }

        protected void AddRow(TRow row)
        {
            TKey mainKey = row.mainKey1;
            DebugUtil.Assert(!mainKey2Row.ContainsKey(mainKey), "{0} 主键重复：{1}，请尝试重新导出配置！", name, mainKey);
            mainKey2Row.Add(mainKey, row);
        }
    }

    /// <summary>
    /// 双主键配置表基类
    /// </summary>
    /// <typeparam name="TKey1"></typeparam>
    /// <typeparam name="TKey2"></typeparam>
    /// <typeparam name="TRow"></typeparam>
    public abstract class XTable<TKey1, TKey2, TRow> : XTable where TRow : XRow<TKey1, TKey2>, new()
    {
        public List<TRow> rows { get { return _rows; } }
        protected List<TRow> _rows;
        protected Dictionary<TKey1, List<TRow>> _firstKey2Rows;
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

        public override void Init()
        {
            _mainKey2Row = new Dictionary<TKey1, Dictionary<TKey2, TRow>>();
            for (int i = 0; i < _rows.Count; i++)
            {
                var row = _rows[i];
                if (!_mainKey2Row.TryGetValue(row.mainKey1, out var secondKey2Row))
                {
                    secondKey2Row = new Dictionary<TKey2, TRow>();
                    _mainKey2Row.Add(row.mainKey1, secondKey2Row);
                }

                DebugUtil.Assert(!secondKey2Row.ContainsKey(row.mainKey2), $"{name} 主键重复：{row.mainKey1} {row.mainKey2}");
                secondKey2Row.Add(row.mainKey2, row);
            }
        }

        public override void OnInit()
        {
            for (int i = 0; i < _rows.Count; i++)
                _rows[i].OnAfterInit();

            OnAfterInit();
        }

        public virtual List<TRow> GetRows(TKey1 key1)
        {
            if (TryGetRows(key1, out var rows)) return rows;
            DebugUtil.Assert(rows != null, $"{name} 找不到指定主键为 {key1} 的行，请尝试重新导出配置！");
            return null;
        }

        public virtual TRow GetRow(TKey1 key1, TKey2 key2)
        {
            if (TryGetRow(key1, key2, out var row)) return row;
            DebugUtil.Assert(row != null, $"{name} 找不到指定主键为 {key1}, {key2} 的行，请尝试重新导出配置！");
            return null;
        }

        public virtual bool TryGetRow(TKey1 key1, TKey2 key2, out TRow row) 
        {
            row = null;
            if (!_mainKey2Row.TryGetValue(key1, out var secondKey2Row))
                return false;

            return secondKey2Row.TryGetValue(key2, out row);
        }

        public virtual bool TryGetRows(TKey1 key1, out List<TRow> rows)
        {
            rows = null;
            if (!_mainKey2Row.TryGetValue(key1, out var secondKey2Row))
                return false;

            if (_firstKey2Rows == null) _firstKey2Rows = new Dictionary<TKey1, List<TRow>>();
            if (!_firstKey2Rows.TryGetValue(key1, out rows))
            {
                rows = secondKey2Row.Values.ToList();
                _firstKey2Rows.Add(key1, rows);
            }

            return true;
        }

        public bool ContainsKey(TKey1 key1)
        {
            return _mainKey2Row.TryGetValue(key1, out var secondKey2Row);
        }

        public bool ContainsKey(TKey1 key1, TKey2 key2) 
        {
            if (!_mainKey2Row.TryGetValue(key1, out var secondKey2Row))
                return false;

            return secondKey2Row.ContainsKey(key2); 
        }


        protected void AddRow(TRow row)
        {
            TKey1 mainKey1= row.mainKey1;
            TKey2 mainKey2 = row.mainKey2;
            if (!_mainKey2Row.TryGetValue(mainKey1, out var secondKey2Row))
            {
                secondKey2Row = new Dictionary<TKey2, TRow>();
                _mainKey2Row.Add(mainKey1, secondKey2Row);
            }

            DebugUtil.Assert(!secondKey2Row.ContainsKey(mainKey2), $"{name} 主键重复：{mainKey1},{mainKey2}，请尝试重新导出配置！");
            _rows.Add(row);
            secondKey2Row.Add(mainKey2, row);
        }
    }

    public abstract class XTable
    {
        public string name;
        public abstract void ReadFromBytes(BytesBuffer buffer);
        public abstract void Init();
        public virtual void OnInit() {}
        public virtual void OnAfterInit() {}
        public virtual void OnCheckWhenExport() {}
    }
}