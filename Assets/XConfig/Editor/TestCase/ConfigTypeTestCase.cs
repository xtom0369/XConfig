using UnityEngine;
using System.Text;
using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace XConfig.Editor
{
    public abstract class ConfigTypeTestCase<TConfigType, TValueType> : TestCase where TConfigType : IConfigType, new()
    {
        public static BytesBuffer buffer = new BytesBuffer(1024);
        public TConfigType configType;

        public ConfigTypeTestCase() 
        {
            configType = new TConfigType();
        }

        /// <summary>
        /// 检查配置格式正确性
        /// </summary>
        /// <param name="content"></param>
        /// <param name="exceptedValue"></param>
        /// <returns></returns>
        public bool CheckConfigFormat(string content, bool exceptedValue)
        {
            bool result = true;
            string error = string.Empty;
            try 
            {
                configType.CheckConfigFormat(content);
            }
            catch (Exception e)
            {
                error = e.ToString();
                result = false;
            }
            bool checkResult = result || string.IsNullOrEmpty(content);
            Assert.AreEqual(exceptedValue, checkResult, error);
            return checkResult;
        }

        /// <summary>
        /// 检查配置值
        /// </summary>
        /// <param name="content"></param>
        /// <param name="exceptedValue"></param>
        /// <returns></returns>
        public void CheckConfigValue(string content, TValueType exceptedValue) 
        {
            if (string.IsNullOrEmpty(content))
                return;

            if (!CheckConfigFormat(content, true)) // 值检测失败测不执行写入读取检测
                return;

            buffer.Clear();
            configType.WriteToBytes(buffer, content);
            var value = ReadFromBytes(buffer);
            Assert.AreEqual(exceptedValue, value);
            return;
        }

        /// <summary>
        /// 读取二进制数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public abstract TValueType ReadFromBytes(BytesBuffer buffer); 

        /// <summary>
        /// 检查默认值
        /// </summary>
        /// <param name="content"></param>
        /// <param name="exceptedValue"></param>
        public void CheckDefaultValue(string content, string exceptedValue)
        {
            string value;

            if (string.IsNullOrEmpty(content))
            {
                value = configType.defaultValue;
                Assert.AreEqual(exceptedValue, value);
                return;
            }

            if (!CheckConfigFormat(content, true)) // 值检测失败测不执行写入读取检测
                return;

            value = configType.ParseDefaultValue(content);
            Assert.AreEqual(exceptedValue, value);
        }
    }
}
