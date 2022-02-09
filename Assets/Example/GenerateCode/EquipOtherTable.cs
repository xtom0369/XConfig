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
	[BindConfigPath("equip_other")]
	public EquipOtherTable equipOtherTable = new EquipOtherTable();
}
[Serializable]
public partial class EquipOtherTable : XTable
{
	public List<EquipOtherRow> rows { get { return _tableRows; }}
	List<EquipOtherRow> _tableRows;
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
			_tableRows = new List<EquipOtherRow>();
			ushort rowCount = buffer.ReadUInt16();
			for (int i = 0; i < rowCount; i++)
			{
				EquipOtherRow row = new EquipOtherRow();
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
	Dictionary<int, EquipOtherRow> _intMajorKey2Row;
	override public void InitRows()
	{
		EquipOtherRow row = null;
		_intMajorKey2Row = new Dictionary<int, EquipOtherRow>();
		for (int i = 0; i < _tableRows.Count; i++)
		{
			row = _tableRows[i];
			int majorKey = row.Id;
			DebugUtil.Assert(!_intMajorKey2Row.ContainsKey(majorKey), "{0} 主键重复：{1}，请先按键盘【alt+r】导出配置试试！", name, majorKey);
			_intMajorKey2Row.Add(majorKey, row);
		}
	}
	virtual public EquipOtherRow GetRow(int majorKey, bool isAssert=true)
	{
		EquipOtherRow row;
		if (_intMajorKey2Row.TryGetValue(majorKey, out row))
			return row;
		if (isAssert)
			DebugUtil.Assert(row != null, "{0} 找不到指定主键为 {1} 的行，请先按键盘【alt+r】导出配置试试！", name, majorKey);
		return null;
	}
	virtual public bool TryGetRow(int majorKey, out EquipOtherRow row)
	{
		return _intMajorKey2Row.TryGetValue(majorKey, out row);
	}
	public bool ContainsMajorKey(int majorKey)
	{
		return _intMajorKey2Row.ContainsKey(majorKey);
	}
	public void AddRow(EquipOtherRow row)
	{
		if (!_intMajorKey2Row.ContainsKey(row.Id))
		{
			_tableRows.Add(row);
			_intMajorKey2Row.Add(row.Id, row);
			Config.Inst.masterEquipmentTable.AddRow(row);//子表才需要往总表添加
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
		for (int i = 0; i < _tableRows.Count; i++)//子表才需要往总表添加
			Config.Inst.masterEquipmentTable.AddRow(_tableRows[i]);
	}
}
[Serializable]
public partial class EquipOtherRow : MasterEquipmentRow
{
	
	#region editor fields 编辑模式使用的成员变量
	#if UNITY_STANDALONE || SERVER_EDITOR
	private string _Comment_1;
	private string Comment_1{ get { return _Comment_1; }}
	#endif
	#endregion
	override public void FromBytes(BytesBuffer buffer)
	{
		base.FromBytes(buffer);
		#if UNITY_STANDALONE || SERVER_EDITOR
		if (buffer.ReadByte() == 1) _Comment_1 = buffer.ReadString();
		else _Comment_1 = null;
		#endif
		#if UNITY_STANDALONE || SERVER_EDITOR
		rowIndex = buffer.ReadInt32();
		#endif
	}
}