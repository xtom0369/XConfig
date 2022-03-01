using System;
using System.Collections.Generic;
using UnityEngine;

namespace XConfig
{
    public class Vector2Type : VectorType<Vector2>
    {
        public sealed override int Count => 2;

        public static Vector2 ReadFromBytes(BytesBuffer buffer)
        {
            return buffer.ReadVector2();
        }

        public static void ReadFromBytes(BytesBuffer buffer, out Vector2 value)
        {
            value = ReadFromBytes(buffer);
        }

        public override void WriteToBytes(BytesBuffer buffer, string content)
        {
            string[] str = ParseMultiParam(content);
            buffer.WriteVector2(new Vector2(float.Parse(str[0]), float.Parse(str[1])));
        }
    }
}
