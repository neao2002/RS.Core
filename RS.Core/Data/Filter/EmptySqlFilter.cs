using System;
using System.Data;
using System.Configuration;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using RS.Data;

namespace RS.Data
{
    /// <summary>
    /// 空查询条件项
    /// </summary>
    [Serializable]
    internal class EmptySqlFilter : IFilter
    {
        IDbContext DB = null;
        private List<SearchItem> AppendItems;
        public EmptySqlFilter(IDbContext db)
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

        public IFilter RemoveDefineItem(SearchItem item) {
            AppendItems.Remove(item);
            return this;
        }

        public IFilter RemoveDefineItem(string FieldName)
        {
            SearchItem item = Find(FieldName);
            if (item!=null)
            {
                AppendItems.Remove(item);
            }
            return this;
        }

        #region IFilter 成员

        public IFilter CopySearchItemsTo(IFilter filter)
        {
            if (filter == this) return this;
            filter.AppendDefineItems(SqlFieldItems);
            filter.AppendDefineItems(AppendItems);
            if (SelfDefineSqlFilter.IsNotWhiteSpace())
            {
                if (filter.SelfDefineSqlFilter.IsWhiteSpace())
                    filter.SelfDefineSqlFilter = SelfDefineSqlFilter;
                else
                    filter.SelfDefineSqlFilter = string.Format("({0}) and ({1})", SelfDefineSqlFilter, filter.SelfDefineSqlFilter);
            }
            return this;
        }
        public IFilter AppendDefineItems(System.Collections.Generic.List<SearchItem> items)
        {
            AppendItems.AddRange(items);
            return this;
        }
        public IFilter AppendDefineItems(SearchItem item)
        {
            AppendItems.Add(item);
            return this;
        }

        /// <summary>
        /// 添加一个SQL过虑条件项
        /// </summary>
        /// <param name="SqlFilterItem"></param>
        /// <returns></returns>
        public IFilter AppendSqlFilter(string SqlFilterItem)
        {
            if (SqlFilterItem.IsNotWhiteSpace()) CustomSqlFilters.Add(SqlFilterItem);
            return this;
        }

        public ListFilter ListFilter
        {
            get
            {
                ListFilter filter = new ListFilter();
                filter.FilterItems.AddRange(sqlFiledItems);
                filter.FilterItems.AddRange(AppendItems);
                filter.SelfDefineSqlFilter = selfDefineSqlFilter;
                filter.CustomSqlFilters = CustomSqlFilters;
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
        public IFilter AppendDefineItems(string Field, FilterDataType dataType, CompareType compareType, object value)
        {
            SearchItem item = new SearchItem(Field, dataType, value);
            item.CompareType = compareType;
            AppendItems.Add(item);
            return this;
        }

        public IFilter AppendDefineItems(string Field, FilterDataType dataType, CompareType compareType, object beginvalue, object endvalue)
        {
            SearchItem item = new SearchItem(Field, dataType, beginvalue);
            item.Value2 = endvalue;
            item.CompareType = compareType;
            AppendItems.Add(item);
            return this;
        }

        public IFilter AppendDefineItems(string Field, SqlPrefixType prefixType, FilterDataType dataType, CompareType compareType, object value)
        {
            SearchItem item = new SearchItem(Field, dataType, value);
            item.SqlPrefixType = prefixType;
            item.CompareType = compareType;
            AppendItems.Add(item);
            return this;
        }
        public IFilter AppendDefineItems(string Field, SqlPrefixType prefixType, FilterDataType dataType, CompareType compareType, object beginvalue, object endvalue)
        {
            SearchItem item = new SearchItem(Field, dataType, beginvalue);
            item.SqlPrefixType = prefixType;
            item.Value2 = endvalue;
            item.CompareType = compareType;
            AppendItems.Add(item);
            return this;
        }



        #endregion

        public string ToSqlFilter()
        {
            if (DB == null)
                return ListFilter.ToSqlFilter();
            else
                return ListFilter.ToSqlFilter(DB);
        }

        public string ToSqlFilter(IDbContext Db)
        {
            return ListFilter.ToSqlFilter(Db);
        }

        public List<T> ToDataSourceLists<T>(IEnumerable<T> SourceDatas)
        {
            return ListFilter.ToDataSourceLists<T>(SourceDatas);
        }
        public string ToSqlFilter(string prefix)
        {
            return FilterHelper.GetFilter(this, prefix);
        }

        public string ToSqlFilter(IDbContext Db, string prefix)
        {
            return FilterHelper.GetFilter(this, Db, prefix);
        }


        internal List<string> CustomSqlFilters { get; set; } = new List<string>();//自定义查询条件，即通过各项直接构成SQL中where查询项，并以and链接而成，如果其中有项为空，则会出异常，类似于“and and”

    }
}
