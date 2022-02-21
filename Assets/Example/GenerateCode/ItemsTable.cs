//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using XConfig;

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
	override public void ReadFromBytes(BytesBuffer buffer)
	{
		if (_tableRows == null)
		{
			_tableRows = new List<ItemsRow>();
			ushort rowCount = buffer.ReadUInt16();
			for (int i = 0; i < rowCount; i++)
			{
				ItemsRow row = new ItemsRow();
				row.ReadFromBytes(buffer);
				_tableRows.Add(row);
			}
		}
		else
		{
			ushort rowCount = buffer.ReadUInt16();
			for (int i = 0; i < rowCount; i++)
				_tableRows[i].ReadFromBytes(buffer);
		}
	}
	Dictionary<int, ItemsRow> _intMajorKey2Row;
	override public void Init()
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
	virtual public ItemsRow GetValue(int majorKey, bool isAssert=true)
	{
		ItemsRow row;
		if (_intMajorKey2Row.TryGetValue(majorKey, out row))
			return row;
		if (isAssert)
			DebugUtil.Assert(row != null, "{0} 找不到指定主键为 {1} 的行，请先按键盘【alt+r】导出配置试试！", name, majorKey);
		return null;
	}
	virtual public bool TryGetValue(int majorKey, out ItemsRow row)
	{
		return _intMajorKey2Row.TryGetValue(majorKey, out row);
	}
	public bool ContainsKey(int majorKey)
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
	override public void OnInit()
	{
		for (int i = 0; i < _tableRows.Count; i++)
			_tableRows[i].OnAfterInit();


		OnAfterInit();
	}
}
[Serializable]
public partial class ItemsRow : XRow
{
	[SerializeField]
	private int _Id;
	[ConfigMainKey]
	public int Id { get { return _Id; }}
	[SerializeField]
	private string _Name;
	public string Name { get { return _Name; }}
	[SerializeField]
	[ConfigReference("Type")]
	private int _TypeId;
	public int TypeId { get { return _TypeId; }}
	private ItemTypeRow _type;
	public ItemTypeRow Type
	{
		get
		{
			if (_TypeId == 0) return null;
			return _type ?? (_type = Config.Inst.itemTypeTable.GetValue(TypeId));
		}
	}
	[SerializeField]
	[ConfigReference("Types")]
	private List<int> _TypesIds;
	private ReadOnlyCollection<int> _TypesIdsReadOnlyCache;
	public ReadOnlyCollection<int> TypesIds { get { return _TypesIdsReadOnlyCache ?? (_TypesIdsReadOnlyCache = _TypesIds.AsReadOnly()); } }
	private List<ItemTypeRow> _types;
	private ReadOnlyCollection<ItemTypeRow> _typesReadOnlyCache;
	public ReadOnlyCollection<ItemTypeRow> Types
	{
		get
		{
			if (_types == null)
			{
				_types = new List<ItemTypeRow>();
				for (int i = 0; i < TypesIds.Count; i++) _types.Add(Config.Inst.itemTypeTable.GetValue(TypesIds[i]));
			}
			return _typesReadOnlyCache ?? (_typesReadOnlyCache = _types.AsReadOnly());
		}
	}
	[SerializeField]
	private string _Icon;
	public string Icon { get { return _Icon; }}
	[SerializeField]
	private string _SmallIcon;
	public string SmallIcon { get { return _SmallIcon; }}
	[SerializeField]
	private int _MaxHave;
	public int MaxHave { get { return _MaxHave; }}
	[SerializeField]
	private int _MaxStacking;
	public int MaxStacking { get { return _MaxStacking; }}
	[SerializeField]
	private List<int> _Source;
	private ReadOnlyCollection<int> _sourceReadOnlyCache;
	public ReadOnlyCollection<int> Source { get { return _sourceReadOnlyCache ?? (_sourceReadOnlyCache = _Source.AsReadOnly()); } }
	[SerializeField]
	private bool _IsArchive;
	public bool IsArchive { get { return _IsArchive; }}
	[SerializeField]
	private bool _IsSell;
	public bool IsSell { get { return _IsSell; }}
	[SerializeField]
	private int _SellDropCount;
	public int SellDropCount { get { return _SellDropCount; }}
	[SerializeField]
	private int _UseDropCount;
	public int UseDropCount { get { return _UseDropCount; }}
	[SerializeField]
	private string _Desc;
	public string Desc { get { return _Desc; }}
	[SerializeField]
	private int _ArrayPriority;
	public int ArrayPriority { get { return _ArrayPriority; }}
	[SerializeField]
	private List<int> _Counts;
	private ReadOnlyCollection<int> _countsReadOnlyCache;
	public ReadOnlyCollection<int> Counts { get { return _countsReadOnlyCache ?? (_countsReadOnlyCache = _Counts.AsReadOnly()); } }
	public override void ReadFromBytes(BytesBuffer buffer)
	{
		if (buffer.ReadByte() == 1) IntType.ReadFromBytes(buffer, out _Id);
		else _Id = 0;
		if (buffer.ReadByte() == 1) StringType.ReadFromBytes(buffer, out _Name);
		else _Name = "未命名";
		_type = null;
		if (buffer.ReadByte() == 1) ReferenceType.ReadFromBytes(buffer, out _TypeId);
		else _TypeId = 0;
		_types = null;
		_typesReadOnlyCache = null;
		_TypesIds = new List<int>();
		if (buffer.ReadByte() == 1)
		{
			byte itemCount = buffer.ReadByte();
			for (int i = 0; i < itemCount; i++) { ReferenceType.ReadFromBytes(buffer, out int value); _TypesIds.Add(value); }
		}
		if (buffer.ReadByte() == 1) StringType.ReadFromBytes(buffer, out _Icon);
		else _Icon = string.Empty;
		if (buffer.ReadByte() == 1) StringType.ReadFromBytes(buffer, out _SmallIcon);
		else _SmallIcon = string.Empty;
		if (buffer.ReadByte() == 1) IntType.ReadFromBytes(buffer, out _MaxHave);
		else _MaxHave = 999;
		if (buffer.ReadByte() == 1) IntType.ReadFromBytes(buffer, out _MaxStacking);
		else _MaxStacking = 999;
		_sourceReadOnlyCache = null;
		_Source = new List<int>();
		if (buffer.ReadByte() == 1)
		{
			byte itemCount = buffer.ReadByte();
			for (int i = 0; i < itemCount; i++) { IntType.ReadFromBytes(buffer, out int value); _Source.Add(value); }
		}
		if (buffer.ReadByte() == 1) BoolType.ReadFromBytes(buffer, out _IsArchive);
		else _IsArchive = true;
		if (buffer.ReadByte() == 1) BoolType.ReadFromBytes(buffer, out _IsSell);
		else _IsSell = false;
		if (buffer.ReadByte() == 1) IntType.ReadFromBytes(buffer, out _SellDropCount);
		else _SellDropCount = 1;
		if (buffer.ReadByte() == 1) IntType.ReadFromBytes(buffer, out _UseDropCount);
		else _UseDropCount = 1;
		if (buffer.ReadByte() == 1) StringType.ReadFromBytes(buffer, out _Desc);
		else _Desc = string.Empty;
		if (buffer.ReadByte() == 1) IntType.ReadFromBytes(buffer, out _ArrayPriority);
		else _ArrayPriority = 0;
		_countsReadOnlyCache = null;
		_Counts = new List<int>();
		if (buffer.ReadByte() == 1)
		{
			byte itemCount = buffer.ReadByte();
			for (int i = 0; i < itemCount; i++) { IntType.ReadFromBytes(buffer, out int value); _Counts.Add(value); }
		}
		rowIndex = buffer.ReadInt32();
	}
}
