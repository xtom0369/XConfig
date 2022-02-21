using System;
using System.Collections.Generic;
using UnityEngine;

namespace XConfig
{
    public class Vector3Type : VectorType
    {
        public override string TypeName => nameof(Vector3);

        public sealed override int Count => 3;

        public static Vector3 ReadFromBytes(BytesBuffer buffer)
        {
            return buffer.ReadVector3();
        }

        public static void ReadFromBytes(BytesBuffer buffer, out Vector3 value)
        {
            value = ReadFromBytes(buffer);
        }

        public override void WriteToBytes(BytesBuffer buffer, string content)
        {
            string[] str = ParseMultiContent(content);
            buffer.WriteVector3(new Vector3(float.Parse(str[0]), float.Parse(str[1]), float.Parse(str[2])));
        }
    }
}
