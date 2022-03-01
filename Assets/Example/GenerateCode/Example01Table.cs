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

[BindConfigFileName("example01", false)]
public partial class Example01Table : XTable<int, Example01Row>
{
}
[BindConfigFileName("example01", false)]
public partial class Example01Row : XRow<int>
{
	public override int mainKey1 => Id;
	public int Id => _id; int _id;
	public string StringField => _stringField; string _stringField;
	public bool BoolField => _boolField; bool _boolField;
	public bool BoolFieldDefault => _boolFieldDefault; bool _boolFieldDefault;
	public byte ByteField => _byteField; byte _byteField;
	public Color ColorField => _colorField; Color _colorField;
	public DateTime DateTimeField => _dateTimeField; DateTime _dateTimeField;
	public EnumExample01 EnumField => _enumField; EnumExample01 _enumField;
	public float FloatField => _floatField; float _floatField;
	public short ShortField => _shortField; short _shortField;
	public Vector2 Vector2Field => _vector2Field; Vector2 _vector2Field;
	public Vector3 Vector3Field => _vector3Field; Vector3 _vector3Field;
	public Vector4 Vector4Field => _vector4Field; Vector4 _vector4Field;
	public int ReferenceFieldId { get { return _referenceFieldId; }}
	int _referenceFieldId;
	[ConfigReference]
	public Example01RefRow ReferenceField
	{
		get
		{
			if (_referenceFieldId == 0) return null;
			return _referenceField ?? (_referenceField = Config.Inst.example01RefTable.GetRow(ReferenceFieldId));
		}
	}
	Example01RefRow _referenceField;
	public ReadOnlyCollection<int> IntFields { get { return _intFieldsReadOnlyCache ?? (_intFieldsReadOnlyCache = _intFields.AsReadOnly()); } }
	List<int> _intFields;
	ReadOnlyCollection<int> _intFieldsReadOnlyCache;
	public ReadOnlyCollection<float> FloatFields { get { return _floatFieldsReadOnlyCache ?? (_floatFieldsReadOnlyCache = _floatFields.AsReadOnly()); } }
	List<float> _floatFields;
	ReadOnlyCollection<float> _floatFieldsReadOnlyCache;
	List<int> _referenceFieldsIds;
	ReadOnlyCollection<int> _referenceFieldsIdsReadOnlyCache;
	public ReadOnlyCollection<int> ReferenceFieldsIds { get { return _referenceFieldsIdsReadOnlyCache ?? (_referenceFieldsIdsReadOnlyCache = _referenceFieldsIds.AsReadOnly()); } }
	List<Example01RefRow> _referenceFields;
	ReadOnlyCollection<Example01RefRow> _referenceFieldsReadOnlyCache;
	[ConfigReference]
	public ReadOnlyCollection<Example01RefRow> ReferenceFields
	{
		get
		{
			if (_referenceFields == null)
			{
				_referenceFields = new List<Example01RefRow>();
				for (int i = 0; i < ReferenceFieldsIds.Count; i++) _referenceFields.Add(Config.Inst.example01RefTable.GetRow(ReferenceFieldsIds[i]));
			}
			return _referenceFieldsReadOnlyCache ?? (_referenceFieldsReadOnlyCache = _referenceFields.AsReadOnly());
		}
	}
	public override void ReadFromBytes(BytesBuffer buffer)
	{
		if (buffer.ReadByte() == 1) { IntType.ReadFromBytes(buffer, out Int32 value); _id = (int)value;}
		else _id = 0;
		if (buffer.ReadByte() == 1) { StringType.ReadFromBytes(buffer, out String value); _stringField = (string)value;}
		else _stringField = string.Empty;
		if (buffer.ReadByte() == 1) { BoolType.ReadFromBytes(buffer, out Boolean value); _boolField = (bool)value;}
		else _boolField = false;
		if (buffer.ReadByte() == 1) { BoolType.ReadFromBytes(buffer, out Boolean value); _boolFieldDefault = (bool)value;}
		else _boolFieldDefault = true;
		if (buffer.ReadByte() == 1) { ByteType.ReadFromBytes(buffer, out Byte value); _byteField = (byte)value;}
		else _byteField = 0;
		if (buffer.ReadByte() == 1) { ColorType.ReadFromBytes(buffer, out Color value); _colorField = (Color)value;}
		else _colorField = Color.clear;
		if (buffer.ReadByte() == 1) { DateTimeType.ReadFromBytes(buffer, out DateTime value); _dateTimeField = (DateTime)value;}
		else _dateTimeField = DateTime.MinValue;
		if (buffer.ReadByte() == 1) { EnumType.ReadFromBytes(buffer, out short value); _enumField = (EnumExample01)value;}
		else _enumField = EnumExample01.Value0;
		if (buffer.ReadByte() == 1) { FloatType.ReadFromBytes(buffer, out Single value); _floatField = (float)value;}
		else _floatField = 0f;
		if (buffer.ReadByte() == 1) { ShortType.ReadFromBytes(buffer, out Int16 value); _shortField = (short)value;}
		else _shortField = 0;
		if (buffer.ReadByte() == 1) { Vector2Type.ReadFromBytes(buffer, out Vector2 value); _vector2Field = (Vector2)value;}
		else _vector2Field = Vector2.zero;
		if (buffer.ReadByte() == 1) { Vector3Type.ReadFromBytes(buffer, out Vector3 value); _vector3Field = (Vector3)value;}
		else _vector3Field = Vector3.zero;
		if (buffer.ReadByte() == 1) { Vector4Type.ReadFromBytes(buffer, out Vector4 value); _vector4Field = (Vector4)value;}
		else _vector4Field = Vector4.zero;
		_referenceField = null;
		if (buffer.ReadByte() == 1) { ReferenceType.ReadFromBytes(buffer, out Int32 value); _referenceFieldId = (int)value;}
		else _referenceFieldId = 0;
		_intFieldsReadOnlyCache = null;
		_intFields = new List<int>();
		if (buffer.ReadByte() == 1)
		{
			byte itemCount = buffer.ReadByte();
			for (int i = 0; i < itemCount; i++) { IntType.ReadFromBytes(buffer, out Int32 value); _intFields.Add((int)value); }
		}
		_floatFieldsReadOnlyCache = null;
		_floatFields = new List<float>();
		if (buffer.ReadByte() == 1)
		{
			byte itemCount = buffer.ReadByte();
			for (int i = 0; i < itemCount; i++) { FloatType.ReadFromBytes(buffer, out Single value); _floatFields.Add((float)value); }
		}
		_referenceFields = null;
		_referenceFieldsReadOnlyCache = null;
		_referenceFieldsIds = new List<int>();
		if (buffer.ReadByte() == 1)
		{
			byte itemCount = buffer.ReadByte();
			for (int i = 0; i < itemCount; i++) { ReferenceType.ReadFromBytes(buffer, out Int32 value); _referenceFieldsIds.Add((int)value); }
		}
	}
}
