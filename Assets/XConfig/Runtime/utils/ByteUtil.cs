using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class ByteUtil
{
    static public int Bool2Bytes(bool value, byte[] bytes, int offset)
    {
        bytes[offset] = value ? (byte)1 : (byte)0;
        return sizeof(byte);
    }
	static public int ULong2Bytes(ulong value, byte[] bytes, int offset)
	{
		bytes[offset] = (byte)(value & 0xff);
		bytes[offset + 1] = (byte)((value >> 8) & 0xff);
		bytes[offset + 2] = (byte)((value >> 16) & 0xff);
		bytes[offset + 3] = (byte)((value >> 24) & 0xff);
		bytes[offset + 4] = (byte)((value >> 32) & 0xff);
		bytes[offset + 5] = (byte)((value >> 40) & 0xff);
		bytes[offset + 6] = (byte)((value >> 48) & 0xff);
		bytes[offset + 7] = (byte)((value >> 56) & 0xff);
		return sizeof(ulong);
	}
	static public int Long2Bytes(long value, byte[] bytes, int offset)
    {
        bytes[offset] = (byte)(value & 0xff);
        bytes[offset + 1] = (byte)((value >> 8) & 0xff);
        bytes[offset + 2] = (byte)((value >> 16) & 0xff);
        bytes[offset + 3] = (byte)((value >> 24) & 0xff);
        bytes[offset + 4] = (byte)((value >> 32) & 0xff);
        bytes[offset + 5] = (byte)((value >> 40) & 0xff);
        bytes[offset + 6] = (byte)((value >> 48) & 0xff);
        bytes[offset + 7] = (byte)((value >> 56) & 0xff);
        return sizeof(long);
    }
    static public int UShort2Bytes(ushort value, byte[] bytes, int offset)
    {
        bytes[offset] = (byte)(value & 0xff);
        bytes[offset + 1] = (byte)((value >> 8) & 0xff);
        return sizeof(ushort);
    }
    static public int Short2Bytes(short value, byte[] bytes, int offset)
    {
        bytes[offset] = (byte)(value & 0xff);
        bytes[offset + 1] = (byte)((value >> 8) & 0xff);
        return sizeof(short);
    }
    static public int UInt2Bytes(uint value, byte[] bytes, int offset)
    {
        bytes[offset] = (byte)(value & 0xff);
        bytes[offset + 1] = (byte)((value >> 8) & 0xff);
        bytes[offset + 2] = (byte)((value >> 16) & 0xff);
        bytes[offset + 3] = (byte)((value >> 24) & 0xff);
        return sizeof(uint);
    }
    static public int Int2Bytes(int value, byte[] bytes, int offset)
    {
        bytes[offset] = (byte)(value & 0xff);
        bytes[offset + 1] = (byte)((value >> 8) & 0xff);
        bytes[offset + 2] = (byte)((value >> 16) & 0xff);
        bytes[offset + 3] = (byte)((value >> 24) & 0xff);
        return sizeof(int);
    }
}
