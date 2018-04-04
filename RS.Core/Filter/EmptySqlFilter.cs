using System;
using System.Data;
using System.Configuration;
using System.Collections.ObjectModel;
using System.Collections.Generic;
namespace RS.Core.Filter
{
    /// <summary>
    /// 空查询条件项
    /// </summary>
    [Serializable]
    internal class EmptySqlFilter : IFilter
    {
        IDbDriver DB = null;
        private List<SearchItem> AppendItems;
        public EmptySqlFilter(IDbDriver db)
        {
            sqlFiledItems = new List<SearchItem>();
            AppendItems = new List<SearchItem>();
            selfDefineSqlFilter = "";
            DB = db;
        }
        public EmptySqlFilter(string field, FilterDataType type, object value)
            : this(null)
        {
            SearchItem item = new SearchItem(field, type, value);
            AppendDefineItems(item);
            selfDefineSqlFilter = "";            
        }
        public SearchItem Find(string FieldName)
        {
            SearchItem item=AppendItems.Find(f => f.FieldName.IsEquals(FieldName));
            return item;
        }

        public void RemoveDefineItem(SearchItem item) {
            AppendItems.Remove(item);
        }

        public void RemoveDefineItem(string FieldName)
        {
            SearchItem item = Find(FieldName);
            if (item!=null)
            {
                AppendItems.Remove(item);
            }
        }

        #region IFilter 成员

        public void CopySearchItemsTo(IFilter filter)
        {
            if (filter == this) return;
            filter.AppendDefineItems(SqlFieldItems);
            filter.AppendDefineItems(AppendItems);
            if (SelfDefineSqlFilter.IsNotWhiteSpace())
            {
                if (filter.SelfDefineSqlFilter.IsWhiteSpace())
                    filter.SelfDefineSqlFilter = SelfDefineSqlFilter;
                else
                    filter.SelfDefineSqlFilter = string.Format("({0}) and ({1})", SelfDefineSqlFilter, filter.SelfDefineSqlFilter);
            }
        }
        public void AppendDefineItems(System.Collections.Generic.List<SearchItem> items)
        {
            AppendItems.AddRange(items);
        }
        public void AppendDefineItems(SearchItem item)
        {
            AppendItems.Add(item);
        }
        public ListFilter ListFilter
        {
            get
            {
                ListFilter filter = new ListFilter();
                filter.FilterItems.AddRange(sqlFiledItems);
                filter.FilterItems.AddRange(AppendItems);
                filter.SelfDefineSqlFilter = selfDefineSqlFilter;
                return filter;
            }
            set
            {
                this.sqlFiledItems = value.FilterItems;
            }
        }
        
        private List<SearchItem> sqlFiledItems;
        public List<SearchItem> SqlFieldItems
        {
            get
            {
                return sqlFiledItems;
            }
            set
            {
                sqlFiledItems = value;
            }
        }

        #endregion
        #region IFilter 成员

        private string selfDefineSqlFilter;
        public string SelfDefineSqlFilter
        {
            get
            {
                return selfDefineSqlFilter;
            }
            set
            {
                selfDefineSqlFilter = value;
            }
        }

        #endregion
        #region IFilter 成员
        public void AppendDefineItems(string Field, FilterDataType dataType, CompareType compareType, object value)
        {
            SearchItem item = new SearchItem(Field, dataType, value);
            item.CompareType = compareType;
            AppendItems.Add(item);
        }

        public void AppendDefineItems(string Field, FilterDataType dataType, CompareType compareType, object beginvalue, object endvalue)
        {
            SearchItem item = new SearchItem(Field, dataType, beginvalue);
            item.Value2 = endvalue;
            item.CompareType = compareType;
            AppendItems.Add(item);
        }

        public void AppendDefineItems(string Field, SqlPrefixType prefixType, FilterDataType dataType, CompareType compareType, object value)
        {
            SearchItem item = new SearchItem(Field, dataType, value);
            item.SqlPrefixType = prefixType;
            item.CompareType = compareType;
            AppendItems.Add(item);
        }
        public void AppendDefineItems(string Field, SqlPrefixType prefixType, FilterDataType dataType, CompareType compareType, object beginvalue, object endvalue)
        {
            SearchItem item = new SearchItem(Field, dataType, beginvalue);
            item.SqlPrefixType = prefixType;
            item.Value2 = endvalue;
            item.CompareType = compareType;
            AppendItems.Add(item);
        }



        #endregion

        public string ToSqlFilter()
        {
            if (DB == null)
                return ListFilter.ToSqlFilter();
            else
                return ListFilter.ToSqlFilter(DB);
        }

        public string ToSqlFilter(IDbDriver Db)
        {
            return ListFilter.ToSqlFilter(Db);
        }

        public List<T> ToDataSourceLists<T>(IEnumerable<T> SourceDatas)
        {
            return ListFilter.ToDataSourceLists<T>(SourceDatas);
        }
        public string ToSqlFilter(string prefix)
        {
            return RSHelper.GetFilter(this, prefix);
        }

        public string ToSqlFilter(IDbDriver Db, string prefix)
        {
            return RSHelper.GetFilter(this, Db, prefix);
        }
    }
}
