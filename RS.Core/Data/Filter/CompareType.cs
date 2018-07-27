using System;
using System.Collections.Generic;
using System.Text;

namespace RS.Data
{
    /// <summary>
    /// 比较类型
    /// </summary>
    public enum CompareType
    {
        /// <summary>
        /// 等于
        /// </summary>
        Equal=0,
        /// <summary>
        /// 不等于
        /// </summary>
        NoEqual=1,
        /// <summary>
        /// 包含,指所有地方
        /// </summary>
        Comprise=2,
        /// <summary>
        /// 左包含，相当于like 'a%';
        /// </summary>
        LeftComprise=3,
        /// <summary>
        /// 右包含,相当于like '%a';
        /// </summary>
        RightComprise=4,
        /// <summary>
        /// 不包含
        /// </summary>
        NoComprise=5,
        /// <summary>
        /// 左不包含
        /// </summary>
        NoLeftComprise=6,
        /// <summary>
        /// 右不包含
        /// </summary>
        NoRightComprise=7,
        /// <summary>
        /// 大于等于
        /// </summary>
        BigOrEqual=8,
        /// <summary>
        /// 小于等于
        /// </summary>
        LessOrEqual=9,
        /// <summary>
        /// 大于
        /// </summary>
        BigThen=10,
        /// <summary>
        /// 小于
        /// </summary>
        LessThen=11,
        /// <summary>
        /// 在之间
        /// </summary>
        Between=12,
        /// <summary>
        /// 含下级
        /// </summary>
        Chields=13,
        /// <summary>
        /// 为空值
        /// </summary>
        DBNull=14,
        /// <summary>
        /// 不为空值
        /// </summary>
        NotDBNull=15
    }
}
