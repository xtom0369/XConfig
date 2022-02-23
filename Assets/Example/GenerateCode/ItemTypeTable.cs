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
	[BindConfigFileName("item_type")]
	public ItemTypeTable itemTypeTable = new ItemTypeTable();
}
[BindConfigFileName("item_type")]
public partial class ItemTypeTable : XTable<int, ItemTypeRow>
{
	public override void OnInit()
	{
		for (int i = 0; i < _rows.Count; i++)
			_rows[i].OnAfterInit();

		OnAfterInit();
	}
}
[BindConfigFileName("item_type")]
public partial class ItemTypeRow : XRow<int>
{
	public override int mainKey1 => Id;
	public int Id => _id; int _id;
	public string IdName => _idName; string _idName;
	public string Name => _name; string _name;
	public int CreateType => _createType; int _createType;
	public string ClientExtArgs => _clientExtArgs; string _clientExtArgs;
	public string ServerExtArgs => _serverExtArgs; string _serverExtArgs;
	public int ProxyRemoveOrder => _proxyRemoveOrder; int _proxyRemoveOrder;
	public bool CanAdd => _canAdd; bool _canAdd;
	public bool CanRemove => _canRemove; bool _canRemove;
	public bool CanCheckCount => _canCheckCount; bool _canCheckCount;
	public string SmallIcon => _smallIcon; string _smallIcon;
	public uint WarehouseType => _warehouseType; uint _warehouseType;
	public int Order => _order; int _order;
	public Vector2 xy => _xy; Vector2 _xy;
	public Vector3 xy3 => _xy3; Vector3 _xy3;
	public float f1 => _f1; float _f1;
	public Color c1 => _c1; Color _c1;
	public DateTime t1 => _t1; DateTime _t1;
	public FlagType flag => _flag; FlagType _flag;
	public override void ReadFromBytes(BytesBuffer buffer)
	{
		if (buffer.ReadByte() == 1) { IntType.ReadFromBytes(buffer, out Int32 value); _id = (int)value;}
		else _id = 0;
		if (buffer.ReadByte() == 1) { StringType.ReadFromBytes(buffer, out String value); _idName = (string)value;}
		else _idName = string.Empty;
		if (buffer.ReadByte() == 1) { StringType.ReadFromBytes(buffer, out String value); _name = (string)value;}
		else _name = string.Empty;
		if (buffer.ReadByte() == 1) { IntType.ReadFromBytes(buffer, out Int32 value); _createType = (int)value;}
		else _createType = 0;
		if (buffer.ReadByte() == 1) { StringType.ReadFromBytes(buffer, out String value); _clientExtArgs = (string)value;}
		else _clientExtArgs = string.Empty;
		if (buffer.ReadByte() == 1) { StringType.ReadFromBytes(buffer, out String value); _serverExtArgs = (string)value;}
		else _serverExtArgs = string.Empty;
		if (buffer.ReadByte() == 1) { IntType.ReadFromBytes(buffer, out Int32 value); _proxyRemoveOrder = (int)value;}
		else _proxyRemoveOrder = 999;
		if (buffer.ReadByte() == 1) { BoolType.ReadFromBytes(buffer, out Boolean value); _canAdd = (bool)value;}
		else _canAdd = true;
		if (buffer.ReadByte() == 1) { BoolType.ReadFromBytes(buffer, out Boolean value); _canRemove = (bool)value;}
		else _canRemove = true;
		if (buffer.ReadByte() == 1) { BoolType.ReadFromBytes(buffer, out Boolean value); _canCheckCount = (bool)value;}
		else _canCheckCount = true;
		if (buffer.ReadByte() == 1) { StringType.ReadFromBytes(buffer, out String value); _smallIcon = (string)value;}
		else _smallIcon = string.Empty;
		if (buffer.ReadByte() == 1) { UIntType.ReadFromBytes(buffer, out UInt32 value); _warehouseType = (uint)value;}
		else _warehouseType = 0;
		if (buffer.ReadByte() == 1) { IntType.ReadFromBytes(buffer, out Int32 value); _order = (int)value;}
		else _order = 0;
		if (buffer.ReadByte() == 1) { Vector2Type.ReadFromBytes(buffer, out Vector2 value); _xy = (Vector2)value;}
		else _xy = Vector2.zero;
		if (buffer.ReadByte() == 1) { Vector3Type.ReadFromBytes(buffer, out Vector3 value); _xy3 = (Vector3)value;}
		else _xy3 = Vector3.zero;
		if (buffer.ReadByte() == 1) { FloatType.ReadFromBytes(buffer, out Single value); _f1 = (float)value;}
		else _f1 = 0f;
		if (buffer.ReadByte() == 1) { ColorType.ReadFromBytes(buffer, out Color value); _c1 = (Color)value;}
		else _c1 = Color.clear;
		if (buffer.ReadByte() == 1) { DateTimeType.ReadFromBytes(buffer, out DateTime value); _t1 = (DateTime)value;}
		else _t1 = DateTime.MinValue;
		if (buffer.ReadByte() == 1) { EnumType.ReadFromBytes(buffer, out short value); _flag = (FlagType)value;}
		else _flag = FlagType.None;
		rowIndex = buffer.ReadInt32();
	}
}
