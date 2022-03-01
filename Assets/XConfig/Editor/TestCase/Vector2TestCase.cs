﻿using UnityEngine;
using System.Text;
using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace XConfig.Editor
{
    public class Vector2TestCase : ConfigTypeTestCase<Vector2Type, Vector2>
    {
        public override void Run() 
        {
            // 格式合法性
            CheckConfigFormat("(2#4)", true);
            CheckConfigFormat("(2.5#4.6)", true);
            CheckConfigFormat("(-2.5#-4.6)", true);
            CheckConfigFormat("(-2.5#-4.6)", true);
            //CheckConfigFormat("2", true);
            //CheckConfigFormat("55", true);
            //CheckConfigFormat("255", true);
            //CheckConfigFormat(byte.MinValue.ToString(), true);
            //CheckConfigFormat(byte.MaxValue.ToString(), true);
            //CheckConfigFormat("  ", false);
            //CheckConfigFormat("-1", false);
            //CheckConfigFormat("a", false);
            //CheckConfigFormat("-", false);
            //CheckConfigFormat("=", false);
            //CheckConfigFormat("测试", false);
            //CheckConfigFormat((byte.MinValue - 1).ToString(), false);
            //CheckConfigFormat((byte.MaxValue + 1).ToString(), false);

            // 检测默认值
            //CheckDefaultValue("", "0");
            //CheckDefaultValue("0", "0");
            //CheckDefaultValue("1", "1");
            //CheckDefaultValue("50", "50");
            //CheckDefaultValue("255", "255");

            // 检测配置值
            //CheckConfigValue("", 0);
            //CheckConfigValue("0", 0);
            //CheckConfigValue("1", 1);
            //CheckConfigValue("50", 50);
            //CheckConfigValue("255", 255);
        }

        public override Vector2 ReadFromBytes(BytesBuffer buffer)
        {
            Vector2Type.ReadFromBytes(buffer, out var value);
            return value;
        }
    }
}
