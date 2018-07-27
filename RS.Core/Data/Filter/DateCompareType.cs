using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RS.Data
{
    /// <summary>
    /// 针对值为日期格式的检索项比较方式，只对日期或日期类字符有效
    /// </summary>
    public enum DateCompareType
    {
        /// <summary>
        /// 普通，即不受此项条件影响
        /// </summary>
        Normal=0,
        /// <summary>
        /// 日比较:精确到日
        /// </summary>
        Date=1,
        /// <summary>
        /// 日期时间比较，精确到具体时间点
        /// </summary>
        DateTime=2,
        /// <summary>
        /// 月份比较，精确到月份(对于季度比较，可直接通过月份比较方式实现，这里不单独列出)
        /// </summary>
        Month=3,
        /// <summary>
        /// 年份比较，精确到年份
        /// </summary>
        Year=4
    }
}
