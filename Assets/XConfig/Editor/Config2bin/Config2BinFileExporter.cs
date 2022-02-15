using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

namespace XConfig.Editor
{
    public class Config2BinFileExporter
    {
        protected string outFilePath;
        protected ConfigFileImporter importer;
        protected BytesBuffer buffer;
        int lineNumber;
        public Config2BinFileExporter(string outFilePath, ConfigFileImporter importer, BytesBuffer buffer)
        {
            this.outFilePath = outFilePath;
            this.importer = importer;
            this.buffer = buffer;
        }
        public void Export()
        {
            buffer.Clear();
            //表名
            string csvFileName = importer.fileName;
            buffer.WriteString(csvFileName);
#if UNITY_STANDALONE
            //文件头
            buffer.WriteString(importer.keyLine);
            buffer.WriteString(importer.commentLine);
            buffer.WriteString(importer.typeLine);
            buffer.WriteString(importer.flagLine);
#endif
            //行数
            int rowCount = importer.childFileImporters.Count == 0 ? importer.cellStrs.Count : 0;//有子表说明此表的行都不需要写，子表会关联到这些行数据
            DebugUtil.Assert(rowCount < ushort.MaxValue, $"表{importer.fileName} 行数上限突破了{ushort.MaxValue}:{rowCount}");
            buffer.WriteUInt16((ushort)rowCount);//行数上限到65536
                                                 //建立所有父表的数组，如果有
            List<ConfigFileImporter> parentImporters = new List<ConfigFileImporter>();
            ConfigFileImporter parentImporter = importer.parentFileImporter;
            while (parentImporter != null)
            {
                parentImporters.Insert(0, parentImporter);
                parentImporter = parentImporter.parentFileImporter;
            }
            //循环行
            for (int i = 0; i < rowCount; i++)
            {
                lineNumber = importer.lineNumbers[i];
                string[] values = importer.cellStrs[i];
                DebugUtil.Assert(values.Length == importer.keys.Length,
                    importer.fileName + " 下面这行很可能是少了或多了一个列 {0} != {1} \n {2}",
                    values.Length, importer.keys.Length, string.Join("XTable.SEPARATOR", values));
                //先将所有父表对应行的各列数据写进流里
                for (int j = 0; j < parentImporters.Count; j++)
                {
                    parentImporter = parentImporters[j];
                    DebugUtil.Assert(parentImporter.firstKey2RowCells.ContainsKey(values[0]), $"子表{importer.relativePath} id={values[0]} 在父表{values[0]}中找不到同id的行，请检测是否漏配行！");
                    string[] parentValues = parentImporter.firstKey2RowCells[values[0]];
                    WriteRow(parentImporter.keys, parentImporter.types, parentValues, parentImporter.flags, parentImporter.parentFileImporter == null);
                }
                //再把子表当前行的各列数据写进流里
                WriteRow(importer.keys, importer.types, values, importer.flags, false);
            }
            using (FileStream fs = new FileStream(outFilePath, FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter writer = new BinaryWriter(fs))
                {
                    writer.Write(buffer.buffer, 0, buffer.size);
                }
            }
        }
        void WriteRow(string[] keys, string[] types, string[] values, string[] flags, bool isParentRow)
        {
            string fileName = importer.fileName;
            for (int i = 0; i < types.Length; i++)
            {
                string key = keys[i];
                if (!string.IsNullOrEmpty(key))//有效列
                {
                    string value = values[i];
                    string type = types[i];
                    string flag = flags[i];
                    bool isNeedGen = ConfigFileImporter.IsFilterNotUseColoum(flag) == false;
#if UNITY_STANDALONE
                    isNeedGen = true;
#endif
                    //前后不能含有空白字符
                    if (flag.IndexOf("N") == -1)
                        Assert(!(value.StartsWith(" ") || value.EndsWith(" ")), "前后不能含有空白字符：{0}", value);
                    if (flag.Contains("M"))
                        Assert(!string.IsNullOrEmpty(value), "主键那列不能为空");
                    //子类不用导出主键
                    if (!isParentRow && flag.Contains("M") && importer.parentFileImporter != null || !isNeedGen)
                    {
                        continue;
                    }
                    if (string.IsNullOrEmpty(value))//未配置，填入字节0表示无字段
                        buffer.WriteByte(0);
                    else
                    {
                        buffer.WriteByte(1);//有配置，填入字节1表示有字段
                        if (type.StartsWith("List<"))//数组类型
                            WriteListType(type, value, flag);
                        else if (flag.Contains("L"))//层类型
                            WriteLayerType(type, value, flag);
                        else//基本类型
                            WriteBasicType(type, value, flag);
                    }
                }
            }
#if UNITY_STANDALONE
            buffer.WriteInt32(lineNumber);
#endif
        }
        void WriteListType(string type, string value, string flag)
        {
            int startIdx = type.IndexOf('<');
            int endIdx = type.IndexOf('>');
            string itemType = type.Substring(startIdx + 1, endIdx - startIdx - 1);//数组项的类型
            string[] items = value.Split('|');
            DebugUtil.Assert(items.Length < byte.MaxValue, "表{0} 数组上限突破了{1}:{2}",
                importer.fileName, byte.MaxValue, items.Length);
            buffer.WriteByte((byte)items.Length);//先写入数组长度
            foreach (var listItem in items)
            {
                Assert(!string.IsNullOrEmpty(listItem), "列表元素不能为空：{0}", value);
                WriteBasicType(itemType, listItem, flag);//写入每一项
            }
        }
        void WriteLayerType(string type, string value, string flag)
        {
            string[] str = value.Split('|');
            int layer = 0;
            foreach (var s in str)
                if (!string.IsNullOrEmpty(s))
                    layer |= 1 << int.Parse(s.Trim());
            buffer.WriteInt32(layer);
        }
        void WriteBasicType(string type, string value, string flag)
        {
            if (type.StartsWith("Enum"))//枚举类型
                WriteEnumType(type, value, flag);
            else
            {
                if (flag.Contains("R"))//如果是引用类型，则type为string
                    type = "string";

                IUserDefineType iUserDefineType = GetIUserDefineTypeResult(type);
                if (iUserDefineType != null)
                {
                    string errorInformation = iUserDefineType.CheckConfigFormat(value);
                    if (errorInformation != null)
                    {
                        Assert(false, errorInformation);
                    }
                    iUserDefineType.ReadFromString(value);
                    iUserDefineType.WriteToBytes(buffer);
                    return;
                }

                switch (type)
                {
                    case "bool":
                        buffer.WriteBool(value == "1");
                        break;
                    case "int":
                        int resultInt;
                        Assert(int.TryParse(value, out resultInt), "解析int类型出错：{0}", value);
                        buffer.WriteInt32(resultInt);
                        break;
                    case "uint":
                        uint resultUInt;
                        Assert(uint.TryParse(value, out resultUInt), "解析uint类型出错：{0}", value);
                        buffer.WriteUInt32(resultUInt);
                        break;
                    case "float":
                        float resultFloat;
                        Assert(float.TryParse(value, out resultFloat), "解析float类型出错：{0}", value);
                        AssertFloat(resultFloat);//float类型最多配小数点后4位
                        buffer.WriteFloat(resultFloat);
                        break;
                    case "string":
                        if (flag.Contains("U"))
                            value = value.ToUpper();
                        buffer.WriteString(value.Replace("\\n", "\n"));//解决表中含有一个换行符，但读取到代码中会出现两个的问题
                        break;
                    case "DateTime"://按字串处理
                    case "date":
                        buffer.WriteString(value.Replace("\\n", "\n"));//解决表中含有一个换行符，但读取到代码中会出现两个的问题
                        break;
                    case "Vector2":
                        Vector2 v2 = ParseVector2(value);
                        AssertVector2(v2);
                        buffer.WriteVector2(v2);
                        break;
                    case "Vector3":
                        Vector3 v3 = ParseVector3(value);
                        AssertVector3(v3);
                        buffer.WriteVector3(v3);
                        break;
                    case "Vector4":
                        Vector4 v4 = ParseVector4(value);
                        AssertVector4(v4);
                        buffer.WriteVector4(v4);
                        break;
                    case "Color":
                        Color color = ParseColor(value);
                        buffer.WriteColor(color);
                        break;
                    default:
                        Assert(false, "不支持的数据类型：" + type);
                        break;
                }
            }
        }
        /// <summary>
        /// 获得指向子类对象的父类指针，如果当前传入的类型没有被定义，则返回空
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private IUserDefineType GetIUserDefineTypeResult(string type)
        {
            IUserDefineType iUserDefineType = null;

            Type targetClass = AssemblyUtil.GetType(type);
            if (targetClass != null && typeof(IUserDefineType).IsAssignableFrom(targetClass))
            {
                iUserDefineType = Activator.CreateInstance(targetClass) as IUserDefineType;
            }

            return iUserDefineType;
        }
        void WriteEnumType(string type, string value, string flag)
        {
            try//TODO：这里会有未定义的enum被解析 调用EnumIsDefined防止
            {
                //枚举当做int16处理
                int resultInt;
                Assert(int.TryParse(value, out resultInt), "解析int类型出错：{0}", value);
                buffer.WriteInt16((short)resultInt);
            }
            catch (Exception e)
            {
                DebugUtil.LogError("{0} type={1} value={2}", importer.fileName, type, value);
                DebugUtil.LogError(e.ToString());
            }
        }
        protected void Assert(bool isValid, string msg, params object[] args)
        {
            string logStr = string.Format(msg, args);
            DebugUtil.Assert(isValid, $"表={importer.fileName} 行号={lineNumber}:{logStr}");
        }
        protected void AssertFloat(float f)
        {
            string str = f.ToString();
            int dotIdx = str.IndexOf(".");
            if (dotIdx >= 0)
            {
                int decimalDigits = str.Length - dotIdx - 1;
                if (str[dotIdx] == '-')//要忽略负号
                    decimalDigits -= 1;
                Assert(decimalDigits <= 4, "float类型最多只能配4位小数:{0}", str);
            }
        }
        protected void AssertFixFloat(string str)
        {
            int dotIdx = str.IndexOf(".");
            if (dotIdx >= 0)
            {
                int decimalDigits = str.Length - dotIdx - 1;
                if (str[dotIdx] == '-')//要忽略负号
                    decimalDigits -= 1;
                Assert(decimalDigits <= 4, "FixFloat类型最多只能配4位小数:{0}", str);
            }
        }
        protected void AssertVector2(Vector2 v2)
        {
            AssertFloat(v2.x);
            AssertFloat(v2.y);
        }
        protected void AssertVector3(Vector3 v3)
        {
            AssertFloat(v3.x);
            AssertFloat(v3.y);
            AssertFloat(v3.z);
        }
        protected void AssertVector4(Vector4 v4)
        {
            AssertFloat(v4.x);
            AssertFloat(v4.y);
            AssertFloat(v4.z);
            AssertFloat(v4.w);
        }
        public Vector2 ParseVector2(string value)
        {
            Assert(value.StartsWith("("), "坐标点要以左括号开始 {0}", value);
            Assert(value.EndsWith(")"), "坐标点要以右括号结束 {0}", value);
            value = value.Replace("(", "").Replace(")", "");
            string[] str = value.Split('#');
            Assert(str.Length == 2, "Vector2长度不对：{0}", value);
            float f;
            Assert(float.TryParse(str[0], out f), "字段格式不对：{0}", value);
            Assert(float.TryParse(str[1], out f), "字段格式不对：{0}", value);
            return new Vector2(float.Parse(str[0].Trim()), float.Parse(str[1].Trim()));
        }
        public Vector3 ParseVector3(string value)
        {
            Assert(value.IndexOf(" ") == -1, "Vector3不能包含空格");
            Assert(value.StartsWith("("), "坐标点要以左括号开始 {0}", value);
            Assert(value.EndsWith(")"), "坐标点要以右括号结束 {0}", value);
            value = value.Replace("(", "").Replace(")", "");
            string[] str = value.Split('#');
            Assert(str.Length == 3, "Vector3长度不对：{0}", value);
            float f;
            Assert(float.TryParse(str[0], out f), "字段格式不对：{0}", value);
            Assert(float.TryParse(str[1], out f), "字段格式不对：{0}", value);
            Assert(float.TryParse(str[2], out f), "字段格式不对：{0}", value);
            return new Vector3(float.Parse(str[0]), float.Parse(str[1]), float.Parse(str[2]));
        }
        public Vector4 ParseVector4(string value)
        {
            Assert(value.IndexOf(" ") == -1, "Vector4不能包含空格");
            Assert(value.StartsWith("("), "坐标点要以左括号开始 {0}", value);
            Assert(value.EndsWith(")"), "坐标点要以右括号结束 {0}", value);
            value = value.Replace("(", "").Replace(")", "");
            string[] str = value.Split('#');
            Assert(str.Length == 4, "Vector4长度不对：{0}", value);
            float f;
            Assert(float.TryParse(str[0], out f), "字段格式不对：{0}", value);
            Assert(float.TryParse(str[1], out f), "字段格式不对：{0}", value);
            Assert(float.TryParse(str[2], out f), "字段格式不对：{0}", value);
            Assert(float.TryParse(str[3], out f), "字段格式不对：{0}", value);
            return new Vector4(float.Parse(str[0]), float.Parse(str[1]), float.Parse(str[2]), float.Parse(str[3]));
        }
        public Color ParseColor(string value)
        {
            Assert(value.IndexOf("|") == -1, "颜色值要以|分割");
            value = value.Replace("(", "").Replace(")", "");
            string[] str = value.Split('#');
            foreach (var s in str)
                if (string.IsNullOrEmpty(s))
                    return Color.white;
            if (str.Length == 3)
                return new Color(float.Parse(str[0].Trim()) / 255, float.Parse(str[1].Trim()) / 255, float.Parse(str[2].Trim()) / 255);
            if (str.Length == 4)
            {
                float alpha = float.Parse(str[3].Trim());//人为规定alpha的范围是【0,1】，不同于rgb
                                                         //if (alpha > 1)
                                                         //    DebugUtil.LogError(alpha.ToString());
                return new Color(float.Parse(str[0].Trim()) / 255, float.Parse(str[1].Trim()) / 255, float.Parse(str[2].Trim()) / 255, alpha);
            }
            return Color.white;
        }
    }
}