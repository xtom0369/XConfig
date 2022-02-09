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
	[BindCsvPath("items")]
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
		BindCsvPathAttribute attr = typeof(Config).GetField("itemsTable").GetCustomAttribute<BindCsvPathAttribute>(false);
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
		row = null;
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
	#if UNITY_STANDALONE || SERVER_EDITOR
	public void RemoveRow(ItemsRow row)
	{
		if (row != null)
			RemoveRow(row.Id);
	}
	public void RemoveRow(int majorKey)
	{
		if (_intMajorKey2Row.ContainsKey(majorKey))
		{
			ItemsRow row = GetRow(majorKey);
			_tableRows.Remove(row);
			_intMajorKey2Row.Remove(majorKey);
		}
	}
	#endif
	private int SortRows(ItemsRow left, ItemsRow right)
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
	public int Id_editor{ get { return _Id; } set { _Id = value; }}
	public string Name_editor{ get { return _Name; } set { _Name = value; }}
	private string _Comment;
	private string Comment{ get { return _Comment; }}
	public string Comment_editor{ get { return _Comment; } set { _Comment = value; }}
	public string TypeId_editor{ get { return _TypeId; } set { _TypeId = value; _typeCache = null; }}
	public ItemTypeRow Type_editor
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
		set
		{
			_typeCache = value;
		}
	}
	public string Icon_editor{ get { return _Icon; } set { _Icon = value; }}
	public string SmallIcon_editor{ get { return _SmallIcon; } set { _SmallIcon = value; }}
	public int MaxHave_editor{ get { return _MaxHave; } set { _MaxHave = value; }}
	public int MaxStacking_editor{ get { return _MaxStacking; } set { _MaxStacking = value; }}
	public List<int> Source_editor{ get { return _Source; } set { _Source = value; _sourceReadOnlyCache = null; }}
	public bool IsArchive_editor{ get { return _IsArchive; } set { _IsArchive = value; }}
	public bool IsSell_editor{ get { return _IsSell; } set { _IsSell = value; }}
	public int SellDropCount_editor{ get { return _SellDropCount; } set { _SellDropCount = value; }}
	public int UseDropCount_editor{ get { return _UseDropCount; } set { _UseDropCount = value; }}
	public string Desc_editor{ get { return _Desc; } set { _Desc = value; }}
	public int ArrayPriority_editor{ get { return _ArrayPriority; } set { _ArrayPriority = value; }}
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
	#if UNITY_STANDALONE || SERVER_EDITOR
	public void ExportCsvRow(StreamWriter writer)
	{
		writer.Write(_Id.ToString());
		writer.Write(XTable.SEPARATOR);
		writer.Write(_Name == "未命名" ? "" : string.IsNullOrEmpty(_Name) ? null : _Name.Replace("\n","\\n"));
		writer.Write(XTable.SEPARATOR);
		writer.Write(_Comment == null ? "" : string.IsNullOrEmpty(_Comment) ? null : _Comment.Replace("\n","\\n"));
		writer.Write(XTable.SEPARATOR);
		writer.Write(string.IsNullOrEmpty(_TypeId) ? null : _TypeId.Replace("\n","\\n"));
		writer.Write(XTable.SEPARATOR);
		writer.Write(_Icon == null ? "" : string.IsNullOrEmpty(_Icon) ? null : _Icon.Replace("\n","\\n"));
		writer.Write(XTable.SEPARATOR);
		writer.Write(_SmallIcon == null ? "" : string.IsNullOrEmpty(_SmallIcon) ? null : _SmallIcon.Replace("\n","\\n"));
		writer.Write(XTable.SEPARATOR);
		writer.Write(_MaxHave == 999 ? "" : _MaxHave.ToString());
		writer.Write(XTable.SEPARATOR);
		writer.Write(_MaxStacking == 999 ? "" : _MaxStacking.ToString());
		writer.Write(XTable.SEPARATOR);
		if (_Source != null && _Source.Count > 0)
		{
			for (int i = 0; i < _Source.Count; i++)
			{
				writer.Write(_Source[i].ToString());
				if (i < _Source.Count - 1)
					writer.Write("|");
			}
		}
		writer.Write(XTable.SEPARATOR);
		writer.Write(_IsArchive == true ? "" : _IsArchive ? "1" : "0");
		writer.Write(XTable.SEPARATOR);
		writer.Write(_IsSell == false ? "" : _IsSell ? "1" : "0");
		writer.Write(XTable.SEPARATOR);
		writer.Write(_SellDropCount == 1 ? "" : _SellDropCount.ToString());
		writer.Write(XTable.SEPARATOR);
		writer.Write(_UseDropCount == 1 ? "" : _UseDropCount.ToString());
		writer.Write(XTable.SEPARATOR);
		writer.Write(_Desc == null ? "" : string.IsNullOrEmpty(_Desc) ? null : _Desc.Replace("\n","\\n"));
		writer.Write(XTable.SEPARATOR);
		writer.Write(_ArrayPriority == 0 ? "" : _ArrayPriority.ToString());
	}
	#endif
	#if UNITY_STANDALONE || SERVER_EDITOR
	public ItemsRow Clone()
	{
		ItemsRow row = new ItemsRow();
		row.Id_editor = _Id;
		row.Name_editor = _Name;
		row.Comment_editor = _Comment;
		row.TypeId_editor = _TypeId;
		row.Icon_editor = _Icon;
		row.SmallIcon_editor = _SmallIcon;
		row.MaxHave_editor = _MaxHave;
		row.MaxStacking_editor = _MaxStacking;
		row.Source_editor = new List<int>();
		if(Source_editor != null)
			row.Source_editor.AddRange(Source_editor);
		row.IsArchive_editor = _IsArchive;
		row.IsSell_editor = _IsSell;
		row.SellDropCount_editor = _SellDropCount;
		row.UseDropCount_editor = _UseDropCount;
		row.Desc_editor = _Desc;
		row.ArrayPriority_editor = _ArrayPriority;
		row.rowIndex = rowIndex;
		return row;
	}
	/// <summary>
	/// 深拷贝数据，但不修改实例的内存地址
	/// </summary>
	public void Clone(ItemsRow row)
	{
		row.Id_editor = _Id;
		row.Name_editor = _Name;
		row.Comment_editor = _Comment;
		row.Type_editor = null;
		row.TypeId_editor = _TypeId;
		row.Icon_editor = _Icon;
		row.SmallIcon_editor = _SmallIcon;
		row.MaxHave_editor = _MaxHave;
		row.MaxStacking_editor = _MaxStacking;
		row.Source_editor = new List<int>();
		if(Source_editor != null)
			row.Source_editor.AddRange(Source_editor);
		row.IsArchive_editor = _IsArchive;
		row.IsSell_editor = _IsSell;
		row.SellDropCount_editor = _SellDropCount;
		row.UseDropCount_editor = _UseDropCount;
		row.Desc_editor = _Desc;
		row.ArrayPriority_editor = _ArrayPriority;
		row.rowIndex = rowIndex;
	}
	public static ItemsRow CloneDefault()
	{
		ItemsRow row = new ItemsRow();
		row.Id_editor = 0;
		row.Name_editor = "未命名";
		row.Comment_editor = string.Empty;
		row.Icon_editor = string.Empty;
		row.SmallIcon_editor = string.Empty;
		row.MaxHave_editor = 999;
		row.MaxStacking_editor = 999;
		row.Source_editor = new List<int>();
		row.IsArchive_editor = true;
		row.IsSell_editor = false;
		row.SellDropCount_editor = 1;
		row.UseDropCount_editor = 1;
		row.Desc_editor = string.Empty;
		row.ArrayPriority_editor = 0;
		return row;
	}
	#endif
	override public void ClearReadOnlyCache()
	{
		_sourceReadOnlyCache = null;
	}
}
