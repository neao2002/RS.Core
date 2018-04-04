using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using System.Linq;

namespace RS.Core.Filter
{
    /// <summary>
    /// 列表记录过滤条件集
    /// </summary>
    [Serializable]
    public class ListFilter
    {
        public ListFilter()
        {
            ///高级查询项，用于高级查询显示
            pendingItems = new List<SearchItem>();
            ///查询过滤项
            filterItems = new List<SearchItem>();
            ///普通查询项
            NormalItems = new List<SearchItem>();

            shortcutItems = new List<SearchItem>();
            //自定义查询条件
            selfDefineSqlFilter = "";
        }
        private List<SearchItem> normalItems;//普通查询条件项
        private List<SearchItem> pendingItems;//待查询的过滤条件
        private List<SearchItem> filterItems;//已设好的查询条件
        private List<SearchItem> shortcutItems;//已设好的快捷查询

        private string selfDefineSqlFilter;//自定义过滤条件
        /// <summary>
        /// 待查询的过滤条件
        /// </summary>
        public List<SearchItem> PendingItems
        {
            get { return pendingItems; }
            set { pendingItems = value; }
        }
        /// <summary>
        /// 普通条件查询项
        /// </summary>
        public List<SearchItem> NormalItems
        {
            get { return normalItems; }
            set { normalItems = value; }
        }

        /// <summary>
        /// 已设好的查询条件
        /// </summary>
        public List<SearchItem> FilterItems
        {
            get { return filterItems; }
            set { filterItems = value; }
        }
        /// <summary>
        /// 快捷查询条件项
        /// </summary>
        public List<SearchItem> ShortcutItems
        {
            get { return shortcutItems; }
            set { shortcutItems = value; }
        }
        /// <summary>
        /// 自定义查询条件
        /// </summary>
        public string SelfDefineSqlFilter
        {
            get { return selfDefineSqlFilter; }
            set { selfDefineSqlFilter = value; }
        }

        #region 得出SQL语句过滤条件
        public string ToSqlFilter()
        {
            return ToSqlFilter(DbUtils.NewDB());
        }

        /// <summary>
        /// 将当前过滤条件项转换为指定SQL过滤条件字符
        /// </summary>
        /// <returns></returns>
        public string ToSqlFilter(DbUtils Db)
        {
            StringBuilder SqlFilter = new StringBuilder();
            bool isHeader = true;
            for (int i = 0; i < filterItems.Count; i++)
            {
                SearchItem item = filterItems[i];
                if (item.SqlPrefixType == SqlPrefixType.EndByGroup)
                {
                    if (!isHeader)
                    {
                        SqlFilter.Append(")");
                    }
                    continue;
                }
                string s = item.ToSqlFilter(Db); //得到当前项查询条件
                if (s.IsNotWhiteSpace()) //有条件
                {
                    string parfix = "";
                    if (item.SqlPrefixType == SqlPrefixType.AfterGroupByAnd || item.SqlPrefixType == SqlPrefixType.AfterGroupByOr)
                        parfix = " (";
                    SqlFilter.AppendFormat(" {0} ({1}) ", isHeader ? parfix : GetPrefix(item.SqlPrefixType), s);
                    if (isHeader) isHeader = false;
                }
            }
            string sql = SqlFilter.ToString();
            if (sql.Length > 0)
            {
                return string.Format("{0} {1}", sql, string.IsNullOrEmpty(selfDefineSqlFilter.Trim()) ? "" : string.Format(" {0} ({1})",Db.Function.AndSqlExp(), selfDefineSqlFilter));
            }
            else
                return selfDefineSqlFilter;
        }

        #endregion

        #region Linq过滤
        public virtual List<T> ToDataSourceLists<T>(IEnumerable<T> SourceDatas)
        {
            var s = from item in SourceDatas
                    where GetLinqWhereExp(item)
                    select item;
            return s.ToList<T>();     
        }

        /// <summary>
        /// 获取当前数据的表达式
        /// </summary>
        /// <returns></returns>
        protected virtual bool GetLinqWhereExp(object o)
        {
            bool currtn = true; //当前检索项的值
            FilterCompCollection Comps = new FilterCompCollection();
            FilterCompCollection curComps = Comps; //当前检索项
            FilterCompCollection child;
            for (int i = 0; i < filterItems.Count; i++)
            {
                SearchItem item = filterItems[i];
                if (item.SqlPrefixType == SqlPrefixType.EndByGroup)  continue;
                currtn = item.ToLinkFilter(o);

                switch (item.SqlPrefixType)
                {
                    case SqlPrefixType.AndSingle:
                        curComps.Add(new FilterComp() { IsFirst = (i == 0), IsAnd = true, CurValue = currtn });
                        break;
                    case SqlPrefixType.OrSingle:
                        curComps.Add(new FilterComp() { IsFirst = (i == 0), IsAnd = false, CurValue = currtn });
                        break;
                    case SqlPrefixType.AfterGroupByAnd:
                        child = new FilterCompCollection() { Owners = curComps };
                        child.Add(new FilterComp() { IsFirst = true, IsAnd = true, CurValue = currtn });

                        curComps.Add(new FilterComp() { IsFirst = (i == 0), IsAnd = true, CurValue = currtn, Child = child });
                        curComps = child;
                        break;
                    case SqlPrefixType.AfterGroupByOr:
                        child = new FilterCompCollection() { Owners = curComps };
                        child.Add(new FilterComp() { IsFirst = true, IsAnd = true, CurValue = currtn });

                        curComps.Add(new FilterComp() { IsFirst = (i == 0), IsAnd = false, CurValue = currtn, Child = child });
                        curComps = child;
                        break;
                    case SqlPrefixType.BeforeGroupByAnd:
                        if (curComps.Owners != null)
                            curComps = curComps.Owners;

                        curComps.Add(new FilterComp() { IsFirst = (i == 0), IsAnd = true, CurValue = currtn });
                        break;
                    case SqlPrefixType.BeforeGroupByOr:
                        if (curComps.Owners != null)
                            curComps = curComps.Owners;

                        curComps.Add(new FilterComp() { IsFirst = (i == 0), IsAnd = false, CurValue = currtn });
                        break;
                    default:
                        curComps.Add(new FilterComp() { IsFirst = (i == 0), IsAnd = true, CurValue = currtn });
                        break;
                }
            }
            return FilterComp.ToValue(Comps);
        }
        #endregion
       
       
        private string GetPrefix(SqlPrefixType type)
        {
            switch (type)
            {
                case SqlPrefixType.AndSingle:
                    return " and ";
                case SqlPrefixType.OrSingle:
                    return " or ";
                case SqlPrefixType.AfterGroupByAnd:
                    return " and ( ";
                case SqlPrefixType.AfterGroupByOr:
                    return " or ( ";
                case SqlPrefixType.BeforeGroupByAnd:
                    return " ) and ";
                case SqlPrefixType.BeforeGroupByOr:
                    return " ) or ";
                case SqlPrefixType.EndByGroup:
                    return ") ";
                default:
                    return " and ";
            }
        }

        /// <summary>
        /// 获取当前查询实例的复本
        /// </summary>
        /// <returns></returns>
        public ListFilter Copy()
        {
            ListFilter filter = new ListFilter();
            filter.FilterItems = this.FilterItems;
            filter.NormalItems = this.NormalItems;
            filter.PendingItems = this.PendingItems;
            filter.ShortcutItems = this.ShortcutItems;
            filter.SelfDefineSqlFilter = this.SelfDefineSqlFilter;
            return filter;
        }

    }
    internal class FilterCompCollection : List<FilterComp>
    {

        /// <summary>
        /// 父级所属集合
        /// </summary>
        public FilterCompCollection Owners { get; set; }
    }
    /// <summary>
    /// 条件比较类
    /// </summary>
    internal class FilterComp
    {
        /// <summary>
        /// 是否为同层的第一项
        /// </summary>
        public bool IsFirst { get; set; }
        /// <summary>
        /// 是否和之前的检索项为AND联系
        /// </summary>
        public bool IsAnd { get; set; }
        /// <summary>
        /// 当前值
        /// </summary>
        public bool CurValue { get; set; }
        /// <summary>
        /// 子查询,如果有子条件，则不计CurValue;
        /// </summary>
        public List<FilterComp> Child { get; set; }



        public static bool ToValue(List<FilterComp> items)
        {
            bool rtn = true;
            foreach (FilterComp item in items)
            {
                if (item.IsFirst)
                {
                    if (item.Child != null && item.Child.Count > 0)
                        rtn = FilterComp.ToValue(item.Child);
                    else
                        rtn = item.CurValue;
                }
                else
                {
                    bool r;
                    if (item.Child != null && item.Child.Count > 0)
                        r = FilterComp.ToValue(item.Child);
                    else
                        r = item.CurValue;

                    if (item.IsAnd)
                        rtn = rtn && r;
                    else
                        rtn = rtn || r;
                }
            }
            return rtn;
        }
    }
}
