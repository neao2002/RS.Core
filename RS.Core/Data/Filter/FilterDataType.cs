using System;
using System.Collections.Generic;
using System.Text;

namespace RS.Data
{
    /// <summary>
    /// 过滤条件项数据类型
    /// </summary>
    public enum FilterDataType
    {
        /// <summary>
        /// 字符类数据类型，如：string,char等类型的字段
        /// </summary>
        String=0,
        /// <summary>
        /// 日期型字段,如:datetime
        /// </summary>
        DateTime=1,
        /// <summary>
        /// 数值类型，可含小数点
        /// </summary>        
        Numeric=2,
        /// <summary>
        /// 整型类数据
        /// </summary>
        Int=3,
        /// <summary>
        /// Guid类数据
        /// </summary>
        Guid=4,
        /// <summary>
        /// 布尔型数据
        /// </summary>
        Bool=5,
        /// <summary>
        /// 分层数据类型
        /// </summary>
        XPath=6,
        /// <summary>
        /// 打开窗口选择指定内容并可编辑的数据类型(已弃用)
        /// </summary>
        OpenWinString=7,
        /// <summary>
        /// 打开窗口选择指定ID的不可编辑的数据类型(已弃用)
        /// </summary>
        OpenWinGuid = 8,
        /// <summary>
        /// 自定义函数参数类
        /// </summary>
        Parament=9
    }
}
