
namespace RS.Core
{
    /// <summary>
    /// 用于数据分页的类
    /// </summary>
    public class PageInfo
    {
        /// <summary>
        /// 当前页的索引，从0开始
        /// </summary>
        public int PageIndex { get; set; } = 0;
        /// <summary>
        /// 分页的大小
        /// </summary>
        public int PageSize { get; set; } = 0;
        /// <summary>
        /// 未分页时的总记录数
        /// </summary>
        public long Totals { get; set; } = 0;

        //以下为汇总相关，用于创建合计相关数据
        public DList<string,TotalType> StatInfos
        {
            get;
            set;
        }
        /// <summary>
        /// 统计行对象
        /// </summary>
        public DynamicObj TotalSubObj { get; set; }




        /// <summary>
        /// 根据页索引和页大小创建分页实例
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public static PageInfo CreateNew(int PageIndex, int PageSize)
        {
            return new PageInfo {PageSize=PageSize, Totals=0, PageIndex=PageIndex };
        }
        public static PageInfo Empty()
        {
            return new PageInfo();
        }
    }
}
