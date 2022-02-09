using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using XConfig;

/*
 * 自动生成代码，请勿编辑
 * 增加变量，函数请在其他目录新建对应的
 * partial class 里增加，
 * 重新生成代码会删除旧代码
 */
public partial class Config
{
	[BindConfigPath("items")]
	public ItemsTable itemsTable = new ItemsTable();
}
[Serializable]
public partial class ItemsTable : XTable
{
	public List<ItemsRow> rows { get { return _tableRows; }}
	List<ItemsRow> _tableRows;
	override public void FromBytes(BytesBuffer buffer)
	{
		#if UNITY_STANDALONE || SERVER_EDITOR
		keys = buffer.ReadString();
		comments = buffer.ReadString();
		types = buffer.ReadString();
		flags = buffer.ReadString();
		#endif
		if (_tableRows == null)
		{
			_tableRows = new List<ItemsRow>();
			ushort rowCount = buffer.ReadUInt16();
			for (int i = 0; i < rowCount; i++)
			{
				ItemsRow row = new ItemsRow();
				row.FromBytes(buffer);
				_tableRows.Add(row);
			}
		}
		else
		{
			ushort rowCount = buffer.ReadUInt16();
			for (int i = 0; i < rowCount; i++)
				_tableRows[i].FromBytes(buffer);
		}
	}
	Dictionary<int, ItemsRow> _intMajorKey2Row;
	override public void InitRows()
	{
		ItemsRow row = null;
		_intMajorKey2Row = new Dictionary<int, ItemsRow>();
		for (int i = 0; i < _tableRows.Count; i++)
		{
			row = _tableRows[i];
			int majorKey = row.Id;
			DebugUtil.Assert(!_intMajorKey2Row.ContainsKey(majorKey), "{0} 主键重复：{1}，请先按键盘【alt+r】导出配置试试！", name, majorKey);
			_intMajorKey2Row.Add(majorKey, row);
		}
	}
	virtual public ItemsRow GetRow(int majorKey, bool isAssert=true)
	{
		ItemsRow row;
		if (_intMajorKey2Row.TryGetValue(majorKey, out row))
			return row;
		if (isAssert)
			DebugUtil.Assert(row != null, "{0} 找不到指定主键为 {1} 的行，请先按键盘【alt+r】导出配置试试！", name, majorKey);
		return null;
	}
	virtual public bool TryGetRow(int majorKey, out ItemsRow row)
	{
		return _intMajorKey2Row.TryGetValue(majorKey, out row);
	}
	public bool ContainsMajorKey(int majorKey)
	{
		return _intMajorKey2Row.ContainsKey(majorKey);
	}
	public void AddRow(ItemsRow row)
	{
		if (!_intMajorKey2Row.ContainsKey(row.Id))
		{
			_tableRows.Add(row);
			_intMajorKey2Row.Add(row.Id, row);
		}
	}
	override public void AllTableInitComplete()
	{
		if (_tableRows.Count > 0 && _tableRows[0] is IRowInitComplete)
		{
			for (int i = 0; i < _tableRows.Count; i++)
			{
				_tableRows[i].ClearReadOnlyCache();
				(_tableRows[i] as IRowInitComplete).AfterRowInitComplete();
			}
		}
		if (this is ITableInitComplete)
			(this as ITableInitComplete).AfterTableInitComplete();
	}
}
[Serializable]
public partial class ItemsRow : XRow
{
	[SerializeField]
	private int _Id;
	public int Id{ get { return _Id; }}
	[SerializeField]
	private string _Name;
	public string Name{ get { return _Name; }}
	[SerializeField]
	[CsvReference("Type")]
	private string _TypeId;
	public string TypeId{ get { return _TypeId; }}
	private ItemTypeRow _typeCache;
	public ItemTypeRow Type
	{
		get
		{
			if (string.IsNullOrEmpty(_TypeId)) return null;
			if (_typeCache == null)
			{
				_typeCache = Config.Inst.itemTypeTable.GetRow(int.Parse(TypeId));
			}
			return _typeCache;
		}
	}
	[SerializeField]
	private string _Icon;
	public string Icon{ get { return _Icon; }}
	[SerializeField]
	private string _SmallIcon;
	public string SmallIcon{ get { return _SmallIcon; }}
	[SerializeField]
	private int _MaxHave;
	public int MaxHave{ get { return _MaxHave; }}
	[SerializeField]
	private int _MaxStacking;
	public int MaxStacking{ get { return _MaxStacking; }}
	[SerializeField]
	private List<int> _Source;
	private ReadOnlyCollection<int> _sourceReadOnlyCache;
	public ReadOnlyCollection<int> Source
	{
		get
		{
			if (_sourceReadOnlyCache == null)
				_sourceReadOnlyCache = _Source.AsReadOnly();
			return _sourceReadOnlyCache;
		}
	}
	[SerializeField]
	private bool _IsArchive;
	public bool IsArchive{ get { return _IsArchive; }}
	[SerializeField]
	private bool _IsSell;
	public bool IsSell{ get { return _IsSell; }}
	[SerializeField]
	private int _SellDropCount;
	public int SellDropCount{ get { return _SellDropCount; }}
	[SerializeField]
	private int _UseDropCount;
	public int UseDropCount{ get { return _UseDropCount; }}
	[SerializeField]
	private string _Desc;
	public string Desc{ get { return _Desc; }}
	[SerializeField]
	private int _ArrayPriority;
	public int ArrayPriority{ get { return _ArrayPriority; }}
	
	#region editor fields 编辑模式使用的成员变量
	#if UNITY_STANDALONE || SERVER_EDITOR
	private string _Comment;
	private string Comment{ get { return _Comment; }}
	#endif
	#endregion
	override public void FromBytes(BytesBuffer buffer)
	{
		if (buffer.ReadByte() == 1) _Id = buffer.ReadInt32();
		else _Id = 0;
		if (buffer.ReadByte() == 1) _Name = buffer.ReadString();
		else _Name = "未命名";
		#if UNITY_STANDALONE || SERVER_EDITOR
		if (buffer.ReadByte() == 1) _Comment = buffer.ReadString();
		else _Comment = null;
		#endif
		_typeCache = null;
		if (buffer.ReadByte() == 1) _TypeId = buffer.ReadString();
		else _TypeId = null;
		if (buffer.ReadByte() == 1) _Icon = buffer.ReadString();
		else _Icon = null;
		if (buffer.ReadByte() == 1) _SmallIcon = buffer.ReadString();
		else _SmallIcon = null;
		if (buffer.ReadByte() == 1) _MaxHave = buffer.ReadInt32();
		else _MaxHave = 999;
		if (buffer.ReadByte() == 1) _MaxStacking = buffer.ReadInt32();
		else _MaxStacking = 999;
		if (buffer.ReadByte() == 1)
		{
			byte itemCount = buffer.ReadByte();
			if (_Source != null) _Source.Clear();
			else _Source = new List<int>();
			for (int i = 0; i < itemCount; i++)
				_Source.Add(buffer.ReadInt32());
		}
		else _Source = new List<int>();
		if (buffer.ReadByte() == 1) _IsArchive = buffer.ReadBool();
		else _IsArchive = true;
		if (buffer.ReadByte() == 1) _IsSell = buffer.ReadBool();
		else _IsSell = false;
		if (buffer.ReadByte() == 1) _SellDropCount = buffer.ReadInt32();
		else _SellDropCount = 1;
		if (buffer.ReadByte() == 1) _UseDropCount = buffer.ReadInt32();
		else _UseDropCount = 1;
		if (buffer.ReadByte() == 1) _Desc = buffer.ReadString();
		else _Desc = null;
		if (buffer.ReadByte() == 1) _ArrayPriority = buffer.ReadInt32();
		else _ArrayPriority = 0;
		#if UNITY_STANDALONE || SERVER_EDITOR
		rowIndex = buffer.ReadInt32();
		#endif
	}
}