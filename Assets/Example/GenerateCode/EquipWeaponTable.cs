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
	[BindConfigPath("equip_weapon")]
	public EquipWeaponTable equipWeaponTable = new EquipWeaponTable();
}
public partial class EquipWeaponTable : XTable
{
	public List<EquipWeaponRow> rows { get { return _tableRows; }}
	List<EquipWeaponRow> _tableRows;
	override public void ReadFromBytes(BytesBuffer buffer)
	{
		if (_tableRows == null)
		{
			_tableRows = new List<EquipWeaponRow>();
			ushort rowCount = buffer.ReadUInt16();
			for (int i = 0; i < rowCount; i++)
			{
				EquipWeaponRow row = new EquipWeaponRow();
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
	Dictionary<int, EquipWeaponRow> _intMajorKey2Row;
	override public void Init()
	{
		EquipWeaponRow row = null;
		_intMajorKey2Row = new Dictionary<int, EquipWeaponRow>();
		for (int i = 0; i < _tableRows.Count; i++)
		{
			row = _tableRows[i];
			int majorKey = row.Id;
			DebugUtil.Assert(!_intMajorKey2Row.ContainsKey(majorKey), "{0} 主键重复：{1}，请先按键盘【alt+r】导出配置试试！", name, majorKey);
			_intMajorKey2Row.Add(majorKey, row);
		}
	}
	virtual public EquipWeaponRow GetValue(int majorKey, bool isAssert=true)
	{
		EquipWeaponRow row;
		if (_intMajorKey2Row.TryGetValue(majorKey, out row))
			return row;
		if (isAssert)
			DebugUtil.Assert(row != null, "{0} 找不到指定主键为 {1} 的行，请先按键盘【alt+r】导出配置试试！", name, majorKey);
		return null;
	}
	virtual public bool TryGetValue(int majorKey, out EquipWeaponRow row)
	{
		return _intMajorKey2Row.TryGetValue(majorKey, out row);
	}
	public bool ContainsKey(int majorKey)
	{
		return _intMajorKey2Row.ContainsKey(majorKey);
	}
	public void AddRow(EquipWeaponRow row)
	{
		if (!_intMajorKey2Row.ContainsKey(row.Id))
		{
			_tableRows.Add(row);
			_intMajorKey2Row.Add(row.Id, row);
			Config.Inst.masterEquipmentTable.AddRow(row);//子表才需要往总表添加
		}
	}
	override public void OnInit()
	{
		for (int i = 0; i < _tableRows.Count; i++)
			_tableRows[i].OnAfterInit();

		for (int i = 0; i < _tableRows.Count; i++)//子表才需要往总表添加
			Config.Inst.masterEquipmentTable.AddRow(_tableRows[i]);

		OnAfterInit();
	}
}
public partial class EquipWeaponRow : MasterEquipmentRow
{
	private string _AnimatorResPath;
	public string AnimatorResPath { get { return _AnimatorResPath; }}
	public override void ReadFromBytes(BytesBuffer buffer)
	{
		base.ReadFromBytes(buffer);
		if (buffer.ReadByte() == 1) StringType.ReadFromBytes(buffer, out _AnimatorResPath);
		else _AnimatorResPath = string.Empty;
		rowIndex = buffer.ReadInt32();
	}
}
