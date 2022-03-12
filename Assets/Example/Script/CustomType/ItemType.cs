using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XConfig
{
    public class ItemType : ConfigType<ItemType>
    {
        /// <summary>
        /// 道具id
        /// </summary>
        public int id { get; private set; }
        /// <summary>
        /// 道具数量
        /// </summary>
        public int count { get; private set; }

        public override string defaultValue => "null";

        public ItemType() 
        { 
            
        }

        public static void ReadFromBytes(BytesBuffer buffer, out ItemType value)
        {
            value = new ItemType();
            value.id = buffer.ReadInt32();
            value.count = buffer.ReadInt32();
        }

        public override void WriteToBytes(BytesBuffer buffer, string content)
        {
            buffer.WriteInt32(id);
            buffer.WriteInt32(count);
        }

        public override string ParseDefaultValue(string content)
        {
            string[] strs = ParseMultiParam(content);
            // new ItemType(){ id = XX, count = XX };
            return $"new {configTypeName}() {{ id = {strs[0]}, count = {strs[1]} }};";
        }

        public override void CheckConfigFormat(string content)
        {
            AssertMultiParamFormat(content);

            AssertParamCount(content, 2);

            AssertParamsType(content, typeof(int));
        }

        public override bool CheckConfigValid(out string error)
        {
            return base.CheckConfigValid(out error);
        }
    }
}
