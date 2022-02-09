using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using XConfig;
#if UNITY_STANDALONE || SERVER_EDITOR
using System.IO;
using System.Text;
using System.Reflection;
#endif

/*
 * 自动生成代码，请勿编辑
 * 增加变量，函数请在其他目录新建对应的
 * partial class 里增加，
 * 重新生成代码会删除旧代码
 */
public partial class Config
{
	[BindCsvPath("item_type")]
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
	#if UNITY_STANDALONE || SERVER_EDITOR
	override public void ExportCsv()
	{
		string csvPath = GetCsvPath();
		if (!string.IsNullOrEmpty(csvPath) && !IsOpenCsv())
		{
			using (FileStream fs = new FileStream(csvPath, FileMode.Create, FileAccess.Write))
			{
				using (StreamWriter writer = new StreamWriter(fs, Encoding.GetEncoding("GB2312")))
				{
					writer.NewLine = "\r\n";
					writer.WriteLine(keys);
					writer.WriteLine(comments);
					writer.WriteLine(types);
					writer.WriteLine(flags);
					if(_tableRows != null)
					{
						if (IsOverrideSort())
							_tableRows.Sort(Sort);
						else
							_tableRows.Sort(SortRows);
						for (int i = 0; i < _tableRows.Count; i++)
						{
							_tableRows[i].ExportCsvRow(writer);
							writer.Write("\r\n");
						}
					}
					writer.Dispose();
					writer.Close();
				}
			}
		}
	}
	#endif
	#if UNITY_STANDALONE || SERVER_EDITOR
	public static string GetCsvPath()
	{
		string path = null;
		BindCsvPathAttribute attr = typeof(Config).GetField("itemTypeTable").GetCustomAttribute<BindCsvPathAttribute>(false);
		if (attr != null)
		{
			path = string.Format("{0}{1}.bytes", "../config/", attr.csvPath);
		}
		return path;
	}
	#endif
	#if UNITY_STANDALONE || SERVER_EDITOR
	public static bool IsOpenCsv()
	{
		bool ret = false;
		string path = GetCsvPath();
		{
			ret = FileUtil.IsFileInUse(path);
		}
		return ret;
	}
	#endif
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
		row = null;
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
	#if UNITY_STANDALONE || SERVER_EDITOR
	public void RemoveRow(ItemTypeRow row)
	{
		if (row != null)
			RemoveRow(row.Id);
	}
	public void RemoveRow(int majorKey)
	{
		if (_intMajorKey2Row.ContainsKey(majorKey))
		{
			ItemTypeRow row = GetRow(majorKey);
			_tableRows.Remove(row);
			_intMajorKey2Row.Remove(majorKey);
		}
	}
	#endif
	private int SortRows(ItemTypeRow left, ItemTypeRow right)
	{
		int result = left.Id.CompareTo(right.Id);
		return result;
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
	public int Id_editor{ get { return _Id; } set { _Id = value; }}
	public string IdName_editor{ get { return _IdName; } set { _IdName = value; }}
	public string Name_editor{ get { return _Name; } set { _Name = value; }}
	public int CreateType_editor{ get { return _CreateType; } set { _CreateType = value; }}
	public string ClientExtArgs_editor{ get { return _ClientExtArgs; } set { _ClientExtArgs = value; }}
	public string ServerExtArgs_editor{ get { return _ServerExtArgs; } set { _ServerExtArgs = value; }}
	public int ProxyRemoveOrder_editor{ get { return _ProxyRemoveOrder; } set { _ProxyRemoveOrder = value; }}
	public bool CanAdd_editor{ get { return _CanAdd; } set { _CanAdd = value; }}
	public bool CanRemove_editor{ get { return _CanRemove; } set { _CanRemove = value; }}
	public bool CanCheckCount_editor{ get { return _CanCheckCount; } set { _CanCheckCount = value; }}
	public string SmallIcon_editor{ get { return _SmallIcon; } set { _SmallIcon = value; }}
	public int WarehouseType_editor{ get { return _WarehouseType; } set { _WarehouseType = value; }}
	public int Order_editor{ get { return _Order; } set { _Order = value; }}
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
	#if UNITY_STANDALONE || SERVER_EDITOR
	public void ExportCsvRow(StreamWriter writer)
	{
		writer.Write(_Id.ToString());
		writer.Write(XTable.SEPARATOR);
		writer.Write(_IdName == null ? "" : string.IsNullOrEmpty(_IdName) ? null : _IdName.Replace("\n","\\n"));
		writer.Write(XTable.SEPARATOR);
		writer.Write(_Name == null ? "" : string.IsNullOrEmpty(_Name) ? null : _Name.Replace("\n","\\n"));
		writer.Write(XTable.SEPARATOR);
		writer.Write(_CreateType == 0 ? "" : _CreateType.ToString());
		writer.Write(XTable.SEPARATOR);
		writer.Write(_ClientExtArgs == null ? "" : string.IsNullOrEmpty(_ClientExtArgs) ? null : _ClientExtArgs.Replace("\n","\\n"));
		writer.Write(XTable.SEPARATOR);
		writer.Write(_ServerExtArgs == null ? "" : string.IsNullOrEmpty(_ServerExtArgs) ? null : _ServerExtArgs.Replace("\n","\\n"));
		writer.Write(XTable.SEPARATOR);
		writer.Write(_ProxyRemoveOrder == 999 ? "" : _ProxyRemoveOrder.ToString());
		writer.Write(XTable.SEPARATOR);
		writer.Write(_CanAdd == true ? "" : _CanAdd ? "1" : "0");
		writer.Write(XTable.SEPARATOR);
		writer.Write(_CanRemove == true ? "" : _CanRemove ? "1" : "0");
		writer.Write(XTable.SEPARATOR);
		writer.Write(_CanCheckCount == true ? "" : _CanCheckCount ? "1" : "0");
		writer.Write(XTable.SEPARATOR);
		writer.Write(_SmallIcon == null ? "" : string.IsNullOrEmpty(_SmallIcon) ? null : _SmallIcon.Replace("\n","\\n"));
		writer.Write(XTable.SEPARATOR);
		writer.Write(_WarehouseType == 0 ? "" : _WarehouseType.ToString());
		writer.Write(XTable.SEPARATOR);
		writer.Write(_Order == 0 ? "" : _Order.ToString());
	}
	#endif
	#if UNITY_STANDALONE || SERVER_EDITOR
	public ItemTypeRow Clone()
	{
		ItemTypeRow row = new ItemTypeRow();
		row.Id_editor = _Id;
		row.IdName_editor = _IdName;
		row.Name_editor = _Name;
		row.CreateType_editor = _CreateType;
		row.ClientExtArgs_editor = _ClientExtArgs;
		row.ServerExtArgs_editor = _ServerExtArgs;
		row.ProxyRemoveOrder_editor = _ProxyRemoveOrder;
		row.CanAdd_editor = _CanAdd;
		row.CanRemove_editor = _CanRemove;
		row.CanCheckCount_editor = _CanCheckCount;
		row.SmallIcon_editor = _SmallIcon;
		row.WarehouseType_editor = _WarehouseType;
		row.Order_editor = _Order;
		row.rowIndex = rowIndex;
		return row;
	}
	/// <summary>
	/// 深拷贝数据，但不修改实例的内存地址
	/// </summary>
	public void Clone(ItemTypeRow row)
	{
		row.Id_editor = _Id;
		row.IdName_editor = _IdName;
		row.Name_editor = _Name;
		row.CreateType_editor = _CreateType;
		row.ClientExtArgs_editor = _ClientExtArgs;
		row.ServerExtArgs_editor = _ServerExtArgs;
		row.ProxyRemoveOrder_editor = _ProxyRemoveOrder;
		row.CanAdd_editor = _CanAdd;
		row.CanRemove_editor = _CanRemove;
		row.CanCheckCount_editor = _CanCheckCount;
		row.SmallIcon_editor = _SmallIcon;
		row.WarehouseType_editor = _WarehouseType;
		row.Order_editor = _Order;
		row.rowIndex = rowIndex;
	}
	public static ItemTypeRow CloneDefault()
	{
		ItemTypeRow row = new ItemTypeRow();
		row.Id_editor = 0;
		row.IdName_editor = string.Empty;
		row.Name_editor = string.Empty;
		row.CreateType_editor = 0;
		row.ClientExtArgs_editor = string.Empty;
		row.ServerExtArgs_editor = string.Empty;
		row.ProxyRemoveOrder_editor = 999;
		row.CanAdd_editor = true;
		row.CanRemove_editor = true;
		row.CanCheckCount_editor = true;
		row.SmallIcon_editor = string.Empty;
		row.WarehouseType_editor = 0;
		row.Order_editor = 0;
		return row;
	}
	#endif
}
