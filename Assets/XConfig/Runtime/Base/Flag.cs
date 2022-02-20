using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using XConfig;

namespace XConfig 
{
    public enum FlagType 
    {
        None = 1,
        /// <summary>
        /// MajorKey，主键
        /// </summary>
        M = 1 << 1,
        /// <summary>
        /// Reference，引用
        /// </summary>
        R = 1 << 2,
        /// <summary>
        /// NotExport
        /// </summary>
        N = 1 << 3,
        /// <summary>
        /// Texture
        /// </summary>
        T = 1 << 4,
    }

    public struct Flag 
    {
        static string[] flagTypeNames
        {
            get { return _flagTypeNames ?? (_flagTypeNames = Enum.GetNames(typeof(FlagType))); }
        }

        static Array flagTypeValues
        {
            get { return _flagTypeValues ?? (_flagTypeValues = Enum.GetValues(typeof(FlagType))); }
        }

        static string[] _flagTypeNames;
        static Array _flagTypeValues;

        public static Flag Parse(string flag) 
        {
            FlagType type = FlagType.None;

            var names = flagTypeNames;
            var values = flagTypeValues;

            // 挨个字符解析
            foreach (var ch in flag) 
            {
                for(int i = 0; i < names.Length; i++)
                {
                    if (names[i] == ch.ToString())
                    {
                        type ^= FlagType.None; 
                        type |= (FlagType)values.GetValue(i);
                        break;
                    }
                }
            }

            return new Flag(type);
        }

        public FlagType flagType;

        public Flag(FlagType type) 
        {
            this.flagType = type;
        }

        public bool IsMainKey { get { return Contains(FlagType.M); } }
        public bool IsReference { get { return Contains(FlagType.R); } }
        public bool IsNotExport { get { return Contains(FlagType.N); } }
        public bool IsTexture { get { return Contains(FlagType.T); } }

        public bool Contains(FlagType type) { return (this.flagType & type) > 0; }
    }

}
