using UnityEngine;
using System.Text;
using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace XConfig.Editor
{
    public class BoolTypeTestCase : ConfigTypeTestCase<BoolType, bool>
    {
        public override void Run() 
        {
            // 格式合法性
            CheckConfigFormat("0", true);
            CheckConfigFormat("1", true);
            CheckConfigFormat("", true);
            CheckConfigFormat("  ", false);
            CheckConfigFormat("-1", false);
            CheckConfigFormat("2", false);
            CheckConfigFormat("a", false);
            CheckConfigFormat("-", false);
            CheckConfigFormat("=", false);

            // 检测默认值
            CheckDefaultValue("", "false");
            CheckDefaultValue("0", "false");
            CheckDefaultValue("1", "true");

            // 检测配置值
            CheckConfigValue("", false);
            CheckConfigValue("0", false);
            CheckConfigValue("1", true);
        }

        public override bool ReadFromBytes(BytesBuffer buffer)
        {
            BoolType.ReadFromBytes(buffer, out var value);
            return value;
        }
    }
}
