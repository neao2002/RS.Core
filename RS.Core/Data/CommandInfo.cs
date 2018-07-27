using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace RS.Data
{
    internal sealed class CommandInfo
    {
        /// <summary>
        /// 命令文本(指SQL模板)
        /// </summary>
        public string CommandText { get; set; }
        /// <summary>
        /// 命令参数
        /// </summary>
        public List<DbParameter> Parameters { get; set; }
    }
}
