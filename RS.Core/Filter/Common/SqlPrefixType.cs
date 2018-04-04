using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RS.Core.Filter
{
    public enum SqlPrefixType
    {
        /// <summary>
        /// 与前一项独立And，相当于 XXX and 本项
        /// </summary>
        AndSingle = 0,
        /// <summary>
        /// 与前一项独产or，相当于XXX or 本项
        /// </summary>
        OrSingle = 1,
        /// <summary>
        /// 从本项开始子项分组And，相当于XXX and (本项 ...
        /// </summary>
        AfterGroupByAnd = 2,
        /// <summary>
        /// 从本项开始子项分组or，相当于XXX or (本项...)
        /// </summary>
        AfterGroupByOr = 3,
        /// <summary>
        /// 结束之前分组并继续And,相当于...) And 本项
        /// </summary>
        BeforeGroupByAnd = 4,
        /// <summary>
        /// 结束之前分组并继续Or,相当于...) Or 本项
        /// </summary>
        BeforeGroupByOr = 5,
        /// <summary>
        /// 结束分组条件，相当于...)  注意，如是这项，则该项不设任何条件项。
        /// </summary>
        EndByGroup = 6
    }
}
