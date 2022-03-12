using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XConfig
{
    public class ItemType : ConfigType<ItemType>
    {
        #region custom property

        /// <summary>
        /// 道具id
        /// </summary>
        public int id { get; private set; }
        /// <summary>
        /// 道具数量
        /// </summary>
        public int count { get; private set; }
        /// <summary>
        /// 道具配置
        /// </summary>
        public ItemsRow config { get { return _config ?? (_config = Config.Inst.itemsTable.GetRow(id)); } }
        ItemsRow _config;

        #endregion

        #region override 

        public override string defaultValue => "null";

        public static void ReadFromBytes(BytesBuffer buffer, out ItemType value)
        {
            value = new ItemType();
            value.id = buffer.ReadInt32();
            value.count = buffer.ReadInt32();
        }

        public override void WriteToBytes(BytesBuffer buffer, string content)
        {
            string[] strs = ParseMultiParam(content);
            buffer.WriteInt32(int.Parse(strs[0]));
            buffer.WriteInt32(int.Parse(strs[1]));
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

        public override void CheckConfigValid(IConfigType configType)
        {
            var value = configType as ItemType;
            // 检测是否包含道具配置
            Config.Inst.itemsTable.GetRow(value.id);
        }

        #endregion
    }
}
