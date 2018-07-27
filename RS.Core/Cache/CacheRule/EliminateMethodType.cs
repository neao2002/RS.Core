using System;
using System.Collections.Generic;
using System.Text;

namespace RS.Cache
{
    /// <summary>
    /// 缓存存储规则类型
    /// </summary>
    public enum EliminateMethodType
    {
        /// <summary>
        /// 先进先出的淘汰算法
        /// </summary>
        FirstIn_FirstOut,
        /// <summary>
        /// 最近访问淘汰算法
        /// </summary>
        RecentlyVisited
    }
}
