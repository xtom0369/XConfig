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

[BindConfigFileName("master_level", false)]
public partial class MasterLevelTable : XTable<int, int, MasterLevelRow>
{
}
[BindConfigFileName("master_level", false)]
public partial class MasterLevelRow : XRow<int, int>
{
	public override int mainKey1 => Id;
	public override int mainKey2 => Level;
	public int Id => _id; int _id;
	public int Level => _level; int _level;
	public int Exp => _exp; int _exp;
	public override void ReadFromBytes(BytesBuffer buffer)
	{
		if (buffer.ReadByte() == 1) { IntType.ReadFromBytes(buffer, out Int32 value); _id = (int)value;}
		else _id = 0;
		if (buffer.ReadByte() == 1) { IntType.ReadFromBytes(buffer, out Int32 value); _level = (int)value;}
		else _level = 0;
		if (buffer.ReadByte() == 1) { IntType.ReadFromBytes(buffer, out Int32 value); _exp = (int)value;}
		else _exp = 0;
	}
}
