using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class CsvRow
{
    [NonSerialized]
    public string fileName;
    [NonSerialized]
    public int rowIndex;
    [NonSerialized]
    const int PRECI_SHIFT = 14;//10000;//配置表的小数精度 2^14 = 16384

    virtual public void FromBytes(BytesBuffer buffer)
    {
    }

    virtual public void ClearReadOnlyCache()
    {
    }

    protected void Assert(bool isValid)
    {
        Debug.AssertFormat(isValid, "\n{0} 第{1}行有问题", fileName, rowIndex);
    }
    protected void Assert(bool isValid, string msg, params object[] args)
    {
        string logStr = string.Format(msg, args);
        Debug.AssertFormat(isValid, string.Format("\n{0} 第{1}行有问题:{2}", fileName, rowIndex, logStr));
    }
    protected void AssertWeight(float weight)
    {
        Assert(weight >= 0 && weight <= 1, "不在合理范围的权重值：{0}", weight);
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
    //检查字串中是否包含大写字母
    protected void CheckIsContainUpperChar(string str)
    {
        if (string.IsNullOrEmpty(str))
            return;
        char[] chars = str.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            char ch = chars[i];
            if (ch >= 'A' && ch <= 'Z')
                Assert(false, "资源路径包括资源名不能为大写：{0}", str);
        }
    }

    public static Color ParseColor(string value)
    {
        Debug.Assert(value.IndexOf("|") == -1);
        value = value.Replace("(", "").Replace(")", "");
        string[] str = value.Split('#');

        foreach (var s in str)
            if (string.IsNullOrEmpty(s))
                return Color.white;
        if (str.Length == 3)
            return new Color(float.Parse(str[0].Trim()) / 255, float.Parse(str[1].Trim()) / 255, float.Parse(str[2].Trim()) / 255);
        if (str.Length == 4)
            return new Color(float.Parse(str[0].Trim()) / 255, float.Parse(str[1].Trim()) / 255, float.Parse(str[2].Trim()) / 255, float.Parse(str[3].Trim()) / 255);
        return Color.white;
    }

    private bool ParseLayerToField(FieldInfo field, string value)
    {
        if (field == null) return false;
        var attrs = field.GetCustomAttributes(typeof(CsvLayerIntegerAttribute), true);
        if (attrs != null && attrs.Length > 0)
        {
            string[] str = value.Split('|');
            int layer = 0;
            foreach (var s in str)
                if (!string.IsNullOrEmpty(s))
                    layer |= 1 << int.Parse(s.Trim());
            field.SetValue(this, layer);
            return true;
        }
        return false;
    }

    public static Vector2 ParseVector2(string value)
    {
        Debug.AssertFormat(value.StartsWith("("), "坐标点要以左括号开始 {0}", value);
        Debug.AssertFormat(value.EndsWith(")"), "坐标点要以右括号结束 {0}", value);
        value = value.Replace("(", "").Replace(")", "");
        string[] str = value.Split('#');
        Debug.AssertFormat(str.Length == 2, "Vector2长度不对：{0}", value);
        return new Vector2(float.Parse(str[0].Trim()), float.Parse(str[1].Trim()));
    }

    public static Vector3 ParseVector3(string value)
    {
        Debug.AssertFormat(value.IndexOf(" ") == -1, "Vector3不能包含空格");
        Debug.AssertFormat(value.StartsWith("("), "坐标点要以左括号开始 {0}", value);
        Debug.AssertFormat(value.EndsWith(")"), "坐标点要以右括号结束 {0}", value);
        value = value.Replace("(", "").Replace(")", "");
        string[] str = value.Split('#');
        Debug.AssertFormat(str.Length == 3, "Vector3长度不对：{0}", value);
        return new Vector3(float.Parse(str[0]), float.Parse(str[1]), float.Parse(str[2]));
    }
    public static Vector4 ParseVector4(string value)
    {
        Debug.AssertFormat(value.IndexOf(" ") == -1, "Vector4不能包含空格");
        Debug.AssertFormat(value.StartsWith("("), "坐标点要以左括号开始 {0}", value);
        Debug.AssertFormat(value.EndsWith(")"), "坐标点要以右括号结束 {0}", value);
        value = value.Replace("(", "").Replace(")", "");
        string[] str = value.Split('#');
        Debug.AssertFormat(str.Length == 4, "Vector4长度不对：{0}", value);
        return new Vector4(float.Parse(str[0]), float.Parse(str[1]), float.Parse(str[2]), float.Parse(str[3]));
    }

    public override string ToString()
    {
        FieldInfo[] fileds = this.GetType().GetFields();
        string result = this.GetType().Name + " ";
        foreach (FieldInfo field in fileds)
            result += field.Name + "=" + field.GetValue(this) + " ";
        return result;
    }
}

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class BindCsvPathAttribute : Attribute
{
    public string csvPath;
    public BindCsvPathAttribute(string csvPath)
    {
        this.csvPath = csvPath;
    }
}

[AttributeUsage(AttributeTargets.Field)]
public class CsvLayerIntegerAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Field)]
public class TestUITexturePathAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Field)]
public class CsvReferenceAttribute : Attribute
{
    public string property;
    public CsvReferenceAttribute(string property)
    {
        this.property = property;
    }
}
