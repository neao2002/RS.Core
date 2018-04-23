using RS.Core.Data;
using RS.Core.Filter;
using System;
using System.Collections.Generic;
using System.Text;

namespace RS.Core
{
    /// <summary>
    /// 查询统一接口
    /// </summary>
    [System.Xml.Serialization.XmlInclude(typeof(DBNull))]
    [System.Xml.Serialization.SoapInclude(typeof(DBNull))]
    public interface IFilter
    {
        SearchItem Find(string FieldName);
        /// <summary>
        /// 查询条件项及查询结果
        /// </summary>
        ListFilter ListFilter { get;set;}

        /// <summary>
        /// 已设好的查询值
        /// </summary>
        List<SearchItem> SqlFieldItems { get;set;}
        IFilter RemoveDefineItem(SearchItem items);
        IFilter RemoveDefineItem(string FieldName);
        /// <summary>
        /// 追加自定义的查询项(多项)
        /// </summary>
        IFilter AppendDefineItems(List<SearchItem> items);

        /// <summary>
        /// 追加自定义的查询项(单项)
        /// </summary>
        IFilter AppendDefineItems(SearchItem item);
        /// <summary>
        /// 追加自定义的查询项(单项)
        /// </summary>
        /// <param name="Field">字段</param>
        /// <param name="dataType">字段类型</param>
        /// <param name="compareType">过滤类型</param>
        /// <param name="value">值</param>
        IFilter AppendDefineItems(string Field, FilterDataType dataType, CompareType compareType, object value);
        /// <summary>
        /// 追加自定义的查询项(单项)
        /// </summary>
        /// <param name="Field">字段</param>
        /// <param name="dataType">字段类型</param>
        /// <param name="compareType">过滤类型</param>
        /// <param name="value">值</param>
        IFilter AppendDefineItems(string Field, FilterDataType dataType, CompareType compareType, object beginvalue,object endvalue);


        IFilter AppendDefineItems(string Field,SqlPrefixType prefixType, FilterDataType dataType, CompareType compareType, object value);
        IFilter AppendDefineItems(string Field, SqlPrefixType prefixType, FilterDataType dataType, CompareType compareType, object beginvalue, object endvalue);

        /// <summary>
        /// 添加一个SQL过虑条件项
        /// </summary>
        /// <param name="SqlFilterItem"></param>
        /// <returns></returns>
        IFilter AppendSqlFilter(string SqlFilterItem);



        /// <summary>
        /// 当前检索项条件添加到指定过滤项中
        /// </summary>
        /// <param name="filter"></param>
        IFilter CopySearchItemsTo(IFilter filter);
        
        /// <summary>
        /// 自定义过滤条件
        /// </summary>
        string SelfDefineSqlFilter { get;set; }


        #region 以下为直接输入SQL语句
        /// <summary>
        /// 获取当前有效的查询条件
        /// </summary>
        /// <returns></returns>
        string ToSqlFilter();
        /// <summary>
        /// 获取当前有效的查询条件
        /// </summary>
        /// <param name="Db">当前数据库该问驱动</param>
        /// <param name="prefix">构成SQL语句的前缀，如：where ,or,and</param>
        /// <returns></returns>
        string ToSqlFilter(IDbDriver Db);
        /// <summary>
        /// 获取当前有效的查询条件
        /// </summary>
        /// <param name="prefix">构成SQL语句的前缀，如：where ,or,and</param>
        /// <returns></returns>
        string ToSqlFilter(string prefix);
        /// <summary>
        /// 获取当前有效的查询条件
        /// </summary>
        /// <param name="Db">当前数据库该问驱动</param>
        /// <param name="prefix">构成SQL语句的前缀，如：where ,or,and</param>
        /// <returns></returns>
        string ToSqlFilter(IDbDriver Db, string prefix);
        /// <summary>
        /// 对指定集合数据进行Linq过滤
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SourceDatas"></param>
        /// <returns></returns>
        List<T> ToDataSourceLists<T>(IEnumerable<T> SourceDatas);
        #endregion
    }
}
