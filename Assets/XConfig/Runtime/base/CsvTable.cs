using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace XConfig
{
    public interface IRowInitComplete
    {
        /// <summary>
        /// 只限CsvRow的子类继承此接口
        /// 所有表初始化完毕后，用于二次处理的函数，譬如建立更多表之间的关联或者增加一些新的字段给表行
        /// 在editor下和游戏运行时都会被调用到，并且会在AfterTableInitComplete之前调用
        /// 【千万注意！】不要在里面去写Assert的检测代码，
        /// 要写就写在CheckRowInExportTime或CheckTableInExportTime，特殊情况除外
        /// </summary>
        void AfterRowInitComplete();
    }
    public interface ITableInitComplete
    {
        /// <summary>
        /// 只限CsvTable的子类继承此接口
        /// 所有表初始化完毕后，用于二次处理的函数，譬如建立不同于默认字典的集合来关联表行
        /// 在editor下和游戏运行时都会被调用到
        /// 【千万注意！】不要在里面去写Assert的检测代码，
        /// 要写就写在CheckRowInExportTime或CheckTableInExportTime，特殊情况除外
        /// 配置表热加载时会调用此方法，所以要先清掉现有数据
        /// </summary>
        void AfterTableInitComplete();
    }
    public interface ICheckTableRowExportTime
    {
        /// <summary>
        /// 只限CsvRow的子类继承此接口
        /// 生成配置时会被调用到，时机是在所有表都导出完之后才进行的合法性检测
        /// 只会在editor模式下执行，时机在CheckTableInExportTime之前
        /// </summary>
        void CheckRowInExportTime();
    }
    public interface ICheckTableExportTime
    {
        /// <summary>
        /// 只限CsvTable的子类继承此接口
        /// 生成配置时会被调用到，时机是在所有表都导出完之后才进行的合法性检测
        /// 只会在editor模式下执行
        /// </summary>
        void CheckTableInExportTime();
    }

    public class CsvTable
    {
        static public System.Text.Encoding ENCODING;
        static public char[] SEPARATOR = { '\t' };

        public string name;
        public string keys;
        public string comments;
        public string types;
        public string flags;

        virtual public void FromBytes(BytesBuffer buffer)
        {
        }
        virtual public void InitRows()
        {
        }
        virtual public void AllTableInitComplete()
        {
        }
        virtual public void ExportCsv()
        {

        }

        virtual protected bool IsOverrideSort()
        {
            return false;
        }

        virtual protected int Sort(CsvRow left, CsvRow right)
        {
            return 0;
        }
    }
}