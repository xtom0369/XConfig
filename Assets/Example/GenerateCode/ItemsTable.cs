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
	[BindConfigFileName("items")]
	public ItemsTable itemsTable = new ItemsTable();
}
[BindConfigFileName("items")]
public partial class ItemsTable : XTable<int, ItemsRow>
{
	public override void OnInit()
	{
		for (int i = 0; i < _rows.Count; i++)
			_rows[i].OnAfterInit();

		OnAfterInit();
	}
}
[BindConfigFileName("items")]
public partial class ItemsRow : XRow<int>
{
	public override int mainKey1 => Id;
	public int Id => _id; int _id;
	public string Name => _name; string _name;
	public int TypeId { get { return _typeId; }}
	[ConfigReference("Type")]
	int _typeId;
	public ItemTypeRow Type
	{
		get
		{
			if (_typeId == 0) return null;
			return _type ?? (_type = Config.Inst.itemTypeTable.GetRow(TypeId));
		}
	}
	ItemTypeRow _type;
	[ConfigReference("Types")]
	List<int> _typesIds;
	ReadOnlyCollection<int> _typesIdsReadOnlyCache;
	public ReadOnlyCollection<int> TypesIds { get { return _typesIdsReadOnlyCache ?? (_typesIdsReadOnlyCache = _typesIds.AsReadOnly()); } }
	List<ItemTypeRow> _types;
	ReadOnlyCollection<ItemTypeRow> _typesReadOnlyCache;
	public ReadOnlyCollection<ItemTypeRow> Types
	{
		get
		{
			if (_types == null)
			{
				_types = new List<ItemTypeRow>();
				for (int i = 0; i < TypesIds.Count; i++) _types.Add(Config.Inst.itemTypeTable.GetRow(TypesIds[i]));
			}
			return _typesReadOnlyCache ?? (_typesReadOnlyCache = _types.AsReadOnly());
		}
	}
	public string Icon => _icon; string _icon;
	public string SmallIcon => _smallIcon; string _smallIcon;
	public int MaxHave => _maxHave; int _maxHave;
	public int MaxStacking => _maxStacking; int _maxStacking;
	public ReadOnlyCollection<int> Source { get { return _sourceReadOnlyCache ?? (_sourceReadOnlyCache = _source.AsReadOnly()); } }
	List<int> _source;
	ReadOnlyCollection<int> _sourceReadOnlyCache;
	public bool IsArchive => _isArchive; bool _isArchive;
	public bool IsSell => _isSell; bool _isSell;
	public int SellDropCount => _sellDropCount; int _sellDropCount;
	public int UseDropCount => _useDropCount; int _useDropCount;
	public string Desc => _desc; string _desc;
	public int ArrayPriority => _arrayPriority; int _arrayPriority;
	public ReadOnlyCollection<int> Counts { get { return _countsReadOnlyCache ?? (_countsReadOnlyCache = _counts.AsReadOnly()); } }
	List<int> _counts;
	ReadOnlyCollection<int> _countsReadOnlyCache;
	public override void ReadFromBytes(BytesBuffer buffer)
	{
		if (buffer.ReadByte() == 1) { IntType.ReadFromBytes(buffer, out Int32 value); _id = (int)value;}
		else _id = 0;
		if (buffer.ReadByte() == 1) { StringType.ReadFromBytes(buffer, out String value); _name = (string)value;}
		else _name = "未命名";
		_type = null;
		if (buffer.ReadByte() == 1) { ReferenceType.ReadFromBytes(buffer, out Int32 value); _typeId = (int)value;}
		else _typeId = 0;
		_types = null;
		_typesReadOnlyCache = null;
		_typesIds = new List<int>();
		if (buffer.ReadByte() == 1)
		{
			byte itemCount = buffer.ReadByte();
			for (int i = 0; i < itemCount; i++) { ReferenceType.ReadFromBytes(buffer, out Int32 value); _typesIds.Add((int)value); }
		}
		if (buffer.ReadByte() == 1) { StringType.ReadFromBytes(buffer, out String value); _icon = (string)value;}
		else _icon = string.Empty;
		if (buffer.ReadByte() == 1) { StringType.ReadFromBytes(buffer, out String value); _smallIcon = (string)value;}
		else _smallIcon = string.Empty;
		if (buffer.ReadByte() == 1) { IntType.ReadFromBytes(buffer, out Int32 value); _maxHave = (int)value;}
		else _maxHave = 999;
		if (buffer.ReadByte() == 1) { IntType.ReadFromBytes(buffer, out Int32 value); _maxStacking = (int)value;}
		else _maxStacking = 999;
		_sourceReadOnlyCache = null;
		_source = new List<int>();
		if (buffer.ReadByte() == 1)
		{
			byte itemCount = buffer.ReadByte();
			for (int i = 0; i < itemCount; i++) { IntType.ReadFromBytes(buffer, out Int32 value); _source.Add((int)value); }
		}
		if (buffer.ReadByte() == 1) { BoolType.ReadFromBytes(buffer, out Boolean value); _isArchive = (bool)value;}
		else _isArchive = true;
		if (buffer.ReadByte() == 1) { BoolType.ReadFromBytes(buffer, out Boolean value); _isSell = (bool)value;}
		else _isSell = false;
		if (buffer.ReadByte() == 1) { IntType.ReadFromBytes(buffer, out Int32 value); _sellDropCount = (int)value;}
		else _sellDropCount = 1;
		if (buffer.ReadByte() == 1) { IntType.ReadFromBytes(buffer, out Int32 value); _useDropCount = (int)value;}
		else _useDropCount = 1;
		if (buffer.ReadByte() == 1) { StringType.ReadFromBytes(buffer, out String value); _desc = (string)value;}
		else _desc = string.Empty;
		if (buffer.ReadByte() == 1) { IntType.ReadFromBytes(buffer, out Int32 value); _arrayPriority = (int)value;}
		else _arrayPriority = 0;
		_countsReadOnlyCache = null;
		_counts = new List<int>();
		if (buffer.ReadByte() == 1)
		{
			byte itemCount = buffer.ReadByte();
			for (int i = 0; i < itemCount; i++) { IntType.ReadFromBytes(buffer, out Int32 value); _counts.Add((int)value); }
		}
		rowIndex = buffer.ReadInt32();
	}
}
