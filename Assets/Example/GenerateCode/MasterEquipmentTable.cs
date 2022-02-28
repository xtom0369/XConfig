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

[BindConfigFileName("master_equipment", true)]
public partial class MasterEquipmentTable : XTable<int, MasterEquipmentRow>
{
	public override void Init()
	{
		base.Init();

		foreach (var row in Config.Inst.equipOtherTable.rows)
			AddRow(row);

		foreach (var row in Config.Inst.equipWeaponTable.rows)
			AddRow(row);

	}
}
[BindConfigFileName("master_equipment", true)]
public partial class MasterEquipmentRow : XRow<int>
{
	public override int mainKey1 => Id;
	public int Id => _id; int _id;
	public int ValueLevel => _valueLevel; int _valueLevel;
	public int UseLevel => _useLevel; int _useLevel;
	public int StrengthenId => _strengthenId; int _strengthenId;
	public int InitStrengthenLevel => _initStrengthenLevel; int _initStrengthenLevel;
	public int StrengthenLevelMax => _strengthenLevelMax; int _strengthenLevelMax;
	public int JewelCount => _jewelCount; int _jewelCount;
	public ReadOnlyCollection<int> JewelQuality { get { return _jewelQualityReadOnlyCache ?? (_jewelQualityReadOnlyCache = _jewelQuality.AsReadOnly()); } }
	List<int> _jewelQuality;
	ReadOnlyCollection<int> _jewelQualityReadOnlyCache;
	public int SellDropCount => _sellDropCount; int _sellDropCount;
	public int UnlockItemId { get { return _unlockItemId; }}
	int _unlockItemId;
	[ConfigReference]
	public ItemsRow UnlockItem
	{
		get
		{
			if (_unlockItemId == 0) return null;
			return _unlockItem ?? (_unlockItem = Config.Inst.itemsTable.GetRow(UnlockItemId));
		}
	}
	ItemsRow _unlockItem;
	public float DurabilityCostRate => _durabilityCostRate; float _durabilityCostRate;
	public override void ReadFromBytes(BytesBuffer buffer)
	{
		if (buffer.ReadByte() == 1) { IntType.ReadFromBytes(buffer, out Int32 value); _id = (int)value;}
		else _id = 0;
		if (buffer.ReadByte() == 1) { IntType.ReadFromBytes(buffer, out Int32 value); _valueLevel = (int)value;}
		else _valueLevel = 0;
		if (buffer.ReadByte() == 1) { IntType.ReadFromBytes(buffer, out Int32 value); _useLevel = (int)value;}
		else _useLevel = 0;
		if (buffer.ReadByte() == 1) { IntType.ReadFromBytes(buffer, out Int32 value); _strengthenId = (int)value;}
		else _strengthenId = 0;
		if (buffer.ReadByte() == 1) { IntType.ReadFromBytes(buffer, out Int32 value); _initStrengthenLevel = (int)value;}
		else _initStrengthenLevel = 0;
		if (buffer.ReadByte() == 1) { IntType.ReadFromBytes(buffer, out Int32 value); _strengthenLevelMax = (int)value;}
		else _strengthenLevelMax = 0;
		if (buffer.ReadByte() == 1) { IntType.ReadFromBytes(buffer, out Int32 value); _jewelCount = (int)value;}
		else _jewelCount = 0;
		_jewelQualityReadOnlyCache = null;
		_jewelQuality = new List<int>();
		if (buffer.ReadByte() == 1)
		{
			byte itemCount = buffer.ReadByte();
			for (int i = 0; i < itemCount; i++) { IntType.ReadFromBytes(buffer, out Int32 value); _jewelQuality.Add((int)value); }
		}
		if (buffer.ReadByte() == 1) { IntType.ReadFromBytes(buffer, out Int32 value); _sellDropCount = (int)value;}
		else _sellDropCount = 1;
		_unlockItem = null;
		if (buffer.ReadByte() == 1) { ReferenceType.ReadFromBytes(buffer, out Int32 value); _unlockItemId = (int)value;}
		else _unlockItemId = 0;
		if (buffer.ReadByte() == 1) { FloatType.ReadFromBytes(buffer, out Single value); _durabilityCostRate = (float)value;}
		else _durabilityCostRate = 0f;
	}
}
