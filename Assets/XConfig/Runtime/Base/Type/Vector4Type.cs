using System;
using System.Collections.Generic;
using UnityEngine;

namespace XConfig
{
    public class Vector4Type : VectorType<Vector4>
    {
        public sealed override int Count => 4;

        public static void ReadFromBytes(BytesBuffer buffer, out Vector4 value)
        {
            value = buffer.ReadVector4();
        }

        public override void WriteToBytes(BytesBuffer buffer, string content)
        {
            string[] str = ParseMultiParam(content);
            buffer.WriteVector4(new Vector4(float.Parse(str[0]), float.Parse(str[1]), float.Parse(str[2]), float.Parse(str[3])));
        }
    }
}
