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
	[BindConfigPath("master_equipment")]
	public MasterEquipmentTable masterEquipmentTable = new MasterEquipmentTable();
}
[Serializable]
public partial class MasterEquipmentTable : XTable
{
	public List<MasterEquipmentRow> rows { get { return _tableRows; }}
	List<MasterEquipmentRow> _tableRows;
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
			_tableRows = new List<MasterEquipmentRow>();
			ushort rowCount = buffer.ReadUInt16();
			for (int i = 0; i < rowCount; i++)
			{
				MasterEquipmentRow row = new MasterEquipmentRow();
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
	Dictionary<int, MasterEquipmentRow> _intMajorKey2Row;
	override public void InitRows()
	{
		MasterEquipmentRow row = null;
		_intMajorKey2Row = new Dictionary<int, MasterEquipmentRow>();
		for (int i = 0; i < _tableRows.Count; i++)
		{
			row = _tableRows[i];
			int majorKey = row.Id;
			DebugUtil.Assert(!_intMajorKey2Row.ContainsKey(majorKey), "{0} 主键重复：{1}，请先按键盘【alt+r】导出配置试试！", name, majorKey);
			_intMajorKey2Row.Add(majorKey, row);
		}
	}
	virtual public MasterEquipmentRow GetRow(int majorKey, bool isAssert=true)
	{
		MasterEquipmentRow row;
		if (_intMajorKey2Row.TryGetValue(majorKey, out row))
			return row;
		if (isAssert)
			DebugUtil.Assert(row != null, "{0} 找不到指定主键为 {1} 的行，请先按键盘【alt+r】导出配置试试！", name, majorKey);
		return null;
	}
	virtual public bool TryGetRow(int majorKey, out MasterEquipmentRow row)
	{
		return _intMajorKey2Row.TryGetValue(majorKey, out row);
	}
	public bool ContainsMajorKey(int majorKey)
	{
		return _intMajorKey2Row.ContainsKey(majorKey);
	}
	public void AddRow(MasterEquipmentRow row)
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
public partial class MasterEquipmentRow : XRow
{
	[SerializeField]
	private int _Id;
	public int Id{ get { return _Id; }}
	[SerializeField]
	private int _ValueLevel;
	public int ValueLevel{ get { return _ValueLevel; }}
	[SerializeField]
	private int _UseLevel;
	public int UseLevel{ get { return _UseLevel; }}
	[SerializeField]
	private int _StrengthenId;
	public int StrengthenId{ get { return _StrengthenId; }}
	[SerializeField]
	private int _InitStrengthenLevel;
	public int InitStrengthenLevel{ get { return _InitStrengthenLevel; }}
	[SerializeField]
	private int _StrengthenLevelMax;
	public int StrengthenLevelMax{ get { return _StrengthenLevelMax; }}
	[SerializeField]
	private int _JewelCount;
	public int JewelCount{ get { return _JewelCount; }}
	[SerializeField]
	private List<int> _JewelQuality;
	private ReadOnlyCollection<int> _jewelQualityReadOnlyCache;
	public ReadOnlyCollection<int> JewelQuality
	{
		get
		{
			if (_jewelQualityReadOnlyCache == null)
				_jewelQualityReadOnlyCache = _JewelQuality.AsReadOnly();
			return _jewelQualityReadOnlyCache;
		}
	}
	[SerializeField]
	private int _SellDropCount;
	public int SellDropCount{ get { return _SellDropCount; }}
	[SerializeField]
	[CsvReference("UnlockItem")]
	private string _UnlockItemId;
	public string UnlockItemId{ get { return _UnlockItemId; }}
	private ItemsRow _unlockItemCache;
	public ItemsRow UnlockItem
	{
		get
		{
			if (string.IsNullOrEmpty(_UnlockItemId)) return null;
			if (_unlockItemCache == null)
			{
				_unlockItemCache = Config.Inst.itemsTable.GetRow(int.Parse(UnlockItemId));
			}
			return _unlockItemCache;
		}
	}
	[SerializeField]
	private float _DurabilityCostRate;
	public float DurabilityCostRate{ get { return _DurabilityCostRate; }}
	
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
		#if UNITY_STANDALONE || SERVER_EDITOR
		if (buffer.ReadByte() == 1) _Comment = buffer.ReadString();
		else _Comment = null;
		#endif
		if (buffer.ReadByte() == 1) _ValueLevel = buffer.ReadInt32();
		else _ValueLevel = 0;
		if (buffer.ReadByte() == 1) _UseLevel = buffer.ReadInt32();
		else _UseLevel = 0;
		if (buffer.ReadByte() == 1) _StrengthenId = buffer.ReadInt32();
		else _StrengthenId = 0;
		if (buffer.ReadByte() == 1) _InitStrengthenLevel = buffer.ReadInt32();
		else _InitStrengthenLevel = 0;
		if (buffer.ReadByte() == 1) _StrengthenLevelMax = buffer.ReadInt32();
		else _StrengthenLevelMax = 0;
		if (buffer.ReadByte() == 1) _JewelCount = buffer.ReadInt32();
		else _JewelCount = 0;
		if (buffer.ReadByte() == 1)
		{
			byte itemCount = buffer.ReadByte();
			if (_JewelQuality != null) _JewelQuality.Clear();
			else _JewelQuality = new List<int>();
			for (int i = 0; i < itemCount; i++)
				_JewelQuality.Add(buffer.ReadInt32());
		}
		else _JewelQuality = new List<int>();
		if (buffer.ReadByte() == 1) _SellDropCount = buffer.ReadInt32();
		else _SellDropCount = 1;
		_unlockItemCache = null;
		if (buffer.ReadByte() == 1) _UnlockItemId = buffer.ReadString();
		else _UnlockItemId = null;
		if (buffer.ReadByte() == 1) _DurabilityCostRate = buffer.ReadFloat();
		else _DurabilityCostRate = 0;
		#if UNITY_STANDALONE || SERVER_EDITOR
		rowIndex = buffer.ReadInt32();
		#endif
	}
}
