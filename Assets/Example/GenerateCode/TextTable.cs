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
	[BindConfigFileName("text")]
	public TextTable textTable = new TextTable();
}
[BindConfigFileName("text")]
public partial class TextTable : XTable<string, TextRow>
{
	public void AddRow(TextRow row)
	{
		if (!_mainKey2Row.ContainsKey(row.Id))
		{
			_rows.Add(row);
			_mainKey2Row.Add(row.Id, row);
		}
	}
	public override void OnInit()
	{
		for (int i = 0; i < _rows.Count; i++)
			_rows[i].OnAfterInit();

		OnAfterInit();
	}
}
[BindConfigFileName("text")]
public partial class TextRow : XRow<string>
{
	public override string mainKey1 => Id;
	public string Id => _id;
	string _id;
	public string Text => _text;
	string _text;
	public override void ReadFromBytes(BytesBuffer buffer)
	{
		if (buffer.ReadByte() == 1) { StringType.ReadFromBytes(buffer, out String value); _id = (string)value;}
		else _id = string.Empty;
		if (buffer.ReadByte() == 1) { StringType.ReadFromBytes(buffer, out String value); _text = (string)value;}
		else _text = string.Empty;
		rowIndex = buffer.ReadInt32();
	}
}
