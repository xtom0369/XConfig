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
	[BindConfigPath("item_type")]
	public ItemTypeTable itemTypeTable = new ItemTypeTable();
}
[Serializable]
public partial class ItemTypeTable : XTable
{
	public List<ItemTypeRow> rows { get { return _tableRows; }}
	List<ItemTypeRow> _tableRows;
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
			_tableRows = new List<ItemTypeRow>();
			ushort rowCount = buffer.ReadUInt16();
			for (int i = 0; i < rowCount; i++)
			{
				ItemTypeRow row = new ItemTypeRow();
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
	Dictionary<int, ItemTypeRow> _intMajorKey2Row;
	override public void InitRows()
	{
		ItemTypeRow row = null;
		_intMajorKey2Row = new Dictionary<int, ItemTypeRow>();
		for (int i = 0; i < _tableRows.Count; i++)
		{
			row = _tableRows[i];
			int majorKey = row.Id;
			DebugUtil.Assert(!_intMajorKey2Row.ContainsKey(majorKey), "{0} 主键重复：{1}，请先按键盘【alt+r】导出配置试试！", name, majorKey);
			_intMajorKey2Row.Add(majorKey, row);
		}
	}
	virtual public ItemTypeRow GetRow(int majorKey, bool isAssert=true)
	{
		ItemTypeRow row;
		if (_intMajorKey2Row.TryGetValue(majorKey, out row))
			return row;
		if (isAssert)
			DebugUtil.Assert(row != null, "{0} 找不到指定主键为 {1} 的行，请先按键盘【alt+r】导出配置试试！", name, majorKey);
		return null;
	}
	virtual public bool TryGetRow(int majorKey, out ItemTypeRow row)
	{
		return _intMajorKey2Row.TryGetValue(majorKey, out row);
	}
	public bool ContainsMajorKey(int majorKey)
	{
		return _intMajorKey2Row.ContainsKey(majorKey);
	}
	public void AddRow(ItemTypeRow row)
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
public partial class ItemTypeRow : XRow
{
	[SerializeField]
	private int _Id;
	public int Id{ get { return _Id; }}
	[SerializeField]
	private string _IdName;
	public string IdName{ get { return _IdName; }}
	[SerializeField]
	private string _Name;
	public string Name{ get { return _Name; }}
	[SerializeField]
	private int _CreateType;
	public int CreateType{ get { return _CreateType; }}
	[SerializeField]
	private string _ClientExtArgs;
	public string ClientExtArgs{ get { return _ClientExtArgs; }}
	[SerializeField]
	private string _ServerExtArgs;
	public string ServerExtArgs{ get { return _ServerExtArgs; }}
	[SerializeField]
	private int _ProxyRemoveOrder;
	public int ProxyRemoveOrder{ get { return _ProxyRemoveOrder; }}
	[SerializeField]
	private bool _CanAdd;
	public bool CanAdd{ get { return _CanAdd; }}
	[SerializeField]
	private bool _CanRemove;
	public bool CanRemove{ get { return _CanRemove; }}
	[SerializeField]
	private bool _CanCheckCount;
	public bool CanCheckCount{ get { return _CanCheckCount; }}
	[SerializeField]
	private string _SmallIcon;
	public string SmallIcon{ get { return _SmallIcon; }}
	[SerializeField]
	private int _WarehouseType;
	public int WarehouseType{ get { return _WarehouseType; }}
	[SerializeField]
	private int _Order;
	public int Order{ get { return _Order; }}
	
	#region editor fields 编辑模式使用的成员变量
	#if UNITY_STANDALONE || SERVER_EDITOR
	#endif
	#endregion
	override public void FromBytes(BytesBuffer buffer)
	{
		if (buffer.ReadByte() == 1) _Id = buffer.ReadInt32();
		else _Id = 0;
		if (buffer.ReadByte() == 1) _IdName = buffer.ReadString();
		else _IdName = null;
		if (buffer.ReadByte() == 1) _Name = buffer.ReadString();
		else _Name = null;
		if (buffer.ReadByte() == 1) _CreateType = buffer.ReadInt32();
		else _CreateType = 0;
		if (buffer.ReadByte() == 1) _ClientExtArgs = buffer.ReadString();
		else _ClientExtArgs = null;
		if (buffer.ReadByte() == 1) _ServerExtArgs = buffer.ReadString();
		else _ServerExtArgs = null;
		if (buffer.ReadByte() == 1) _ProxyRemoveOrder = buffer.ReadInt32();
		else _ProxyRemoveOrder = 999;
		if (buffer.ReadByte() == 1) _CanAdd = buffer.ReadBool();
		else _CanAdd = true;
		if (buffer.ReadByte() == 1) _CanRemove = buffer.ReadBool();
		else _CanRemove = true;
		if (buffer.ReadByte() == 1) _CanCheckCount = buffer.ReadBool();
		else _CanCheckCount = true;
		if (buffer.ReadByte() == 1) _SmallIcon = buffer.ReadString();
		else _SmallIcon = null;
		if (buffer.ReadByte() == 1) _WarehouseType = buffer.ReadInt32();
		else _WarehouseType = 0;
		if (buffer.ReadByte() == 1) _Order = buffer.ReadInt32();
		else _Order = 0;
		#if UNITY_STANDALONE || SERVER_EDITOR
		rowIndex = buffer.ReadInt32();
		#endif
	}
}
