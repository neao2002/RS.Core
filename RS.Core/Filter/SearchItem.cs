using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using RS.Lib.Data;

namespace RS.Core.Filter
{
    public class SearchItem
    {
        #region 字段定义
        private string name;//检索项名称
        private string fieldname;//检索的字段名
        private FilterDataType dataType;//检索项数据类型
        private CompareType compareType;//数据比较类型
        private object value1;//比较的第一个值
        private object value2;//比较的第二个值:只有当为之间比较时该值才有较
        protected DelegateOfGetDataSource getDataSource;
        private string itemID;
        private Guid defineQuery;
        private SqlPrefixType sqlPrefixType;       
        
        private SearchFieldCollection SearchFields=null;
        private string dateFormat = "";//日期格式表达式,仅针对日期型字符有效


        #endregion

        #region 构造函数:单字段
        /// <summary>
        /// 默认构造函数：创建检索项实例
        /// </summary>
        public SearchItem()
        {
            name = string.Empty;
            defineQuery = Guid.Empty;
            fieldname = string.Empty;
            dataType = FilterDataType.String;
            compareType=CompareType.Comprise;
            itemID = "";
            value1 = DBNull.Value;
            value2 = DBNull.Value;
            valueField = "";
            value2Field="";
            sqlPrefixType = SqlPrefixType.AndSingle;
            dateCompareType = DateCompareType.Normal;
        }

        #region 创建检索控件所需的构造函数

        /// <summary>
        /// 构造函数:主要用于定义检索控件条件项(同一条件单字段)。
        /// </summary>
        /// <param name="field">要检索的字段</param>
        /// <param name="title">该检索项标题</param>
        /// <param name="datatype">检索的数据类型</param>
        /// <param name="_getDataSource">获取检索项数据来源的委托</param>
        /// <param name="_setControl">进行控件定义的委托</param>
        public SearchItem(string field,string title, FilterDataType datatype,DelegateOfGetDataSource _getDataSource)
        {
            defineQuery = Guid.Empty;
            name = title;
            fieldname = field;
            dataType = datatype;
            switch(datatype)
            {
                case FilterDataType.String:
                    compareType = CompareType.Comprise;
                    break;
                case FilterDataType.Bool:
                    compareType = CompareType.Equal;
                    break;
                case FilterDataType.DateTime:
                    compareType = CompareType.Between;
                    break;
                case FilterDataType.XPath:
                    compareType = CompareType.Chields;
                    break;
                default:
                    compareType = CompareType.Equal;
                    break;
            }
            value1 = DBNull.Value;
            value2 = DBNull.Value;
            getDataSource = _getDataSource;
            itemID = "";
            sqlPrefixType = SqlPrefixType.AndSingle;
            dateCompareType = DateCompareType.Normal;
        }

        public SearchItem(string field, string title, FilterDataType datatype, DelegateOfGetDataSource _getDataSource, DelegateOfSetControlForQuery _setControl,Guid QueryID)
          :this(field,title,datatype,_getDataSource)
        {
            defineQuery = QueryID;
        }
        #endregion
        #region 为创建检索过滤条件而自定义创建的检索值的构造函数
        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="datatype"></param>
        /// <param name="value"></param>
        public SearchItem(string field, FilterDataType datatype, object value)
        {
            defineQuery = Guid.Empty;
            name = string.Empty;
            fieldname = field;
            dataType = datatype;
            compareType = CompareType.Equal;
            itemID = "";
            if (value == null)
                value1 = DBNull.Value;
            else
                value1 = value;
            value2 = DBNull.Value;
            sqlPrefixType = SqlPrefixType.AndSingle;
            dateCompareType = DateCompareType.Normal;
        }
        #endregion

        #endregion

        #region 构造函数:多字段，即检索项检索指定多字段，这些字段的类型是一样的
        #region 创建检索控件所需的构造函数
        public SearchItem(FilterDataType datatype, params SearchFieldInfo[] Fields)
        {
            SearchFields = new SearchFieldCollection();
            SearchFields.AddRange(Fields);
            SearchFieldInfo field = SearchFields.ActiveField;
            fieldname = field.FieldName;
            dataType =datatype;
            dateCompareType = DateCompareType.Date;
            name = field.Caption;
            switch (dataType)
            {
                case FilterDataType.String:
                    compareType = CompareType.Comprise;
                    break;
                case FilterDataType.Bool:
                    compareType = CompareType.Equal;
                    break;
                case FilterDataType.DateTime:
                    compareType = CompareType.Between;
                    break;
                case FilterDataType.XPath:
                    compareType = CompareType.Chields;
                    break;
                default:
                    compareType = CompareType.Equal;
                    break;
            }

            value1 = DBNull.Value;
            value2 = DBNull.Value;
            itemID = "";
            sqlPrefixType = SqlPrefixType.AndSingle;
            defineQuery = Guid.Empty;
            dateCompareType = DateCompareType.Normal;
        }
        public SearchItem(string title,FilterDataType datatype, params SearchFieldInfo[] Fields):
            this(datatype,Fields)
        {
            name = title;
        }
        /// <summary>
        /// 构造函数:主要用于定义检索控件条件项(同一条件多字段)。
        /// </summary>
        /// <param name="field">要检索的字段</param>
        /// <param name="title">该检索项标题</param>
        /// <param name="datatype">检索的数据类型</param>
        /// <param name="_getDataSource">获取检索项数据来源的委托</param>
        /// <param name="_setControl">进行控件定义的委托</param>
        public SearchItem(string title, FilterDataType datatype, DelegateOfGetDataSource _getDataSource,params SearchFieldInfo[] Fields):this(title,datatype,Fields)
        {   
            getDataSource = _getDataSource;            
        }

        public SearchItem(string title, FilterDataType datatype, DelegateOfGetDataSource _getDataSource, Guid QueryID,params SearchFieldInfo[] Fields)
            : this( title, datatype, _getDataSource,Fields)
        {
            defineQuery = QueryID;
        }
        #endregion
        #region 为创建检索过滤条件而自定义创建的检索值的构造函数
        /// <summary>
        /// 创建指定字段的检索实例(单字段)
        /// </summary>
        /// <param name="field"></param>
        /// <param name="datatype"></param>
        /// <param name="value"></param>
        public SearchItem(string field, FilterDataType datatype,CompareType ctype,DateCompareType dtype, object v1,object v2)
        {
            defineQuery = Guid.Empty;
            name = string.Empty;
            fieldname = field;
            dataType = datatype;
            compareType = ctype;
            dateCompareType = dtype;
            itemID = "";

            value1 = v1.IsNull() ? DBNull.Value : v1;
            value2 = v2.IsNull() ? DBNull.Value : v2;

            sqlPrefixType = SqlPrefixType.AndSingle;
            dateCompareType = DateCompareType.Normal;
        }
        /// <summary>
        /// 创建多字段的检索实例,这里是同时检索多字段，每个字段过滤条件是以 or 连接
        /// </summary>
        /// <param name="datatype"></param>
        /// <param name="value"></param>
        /// <param name="Fields"></param>
        public SearchItem(FilterDataType datatype, object value, SearchFieldInfo[] Fields)
        {
            SearchFields = new SearchFieldCollection();
            SearchFields.AddRange(Fields);

            defineQuery = Guid.Empty;
            name = string.Empty;            
            dataType = datatype;
            compareType = CompareType.Equal;
            itemID = "";

            value1 = value.IsNull() ? DBNull.Value : value;
            value2 = DBNull.Value;
            sqlPrefixType = SqlPrefixType.AndSingle;
            isMultiColumn = true;
            dateCompareType = DateCompareType.Normal;
        }
        /// <summary>
        /// 创建多字段的检索实例,这里是同时检索多字段，每个字段过滤条件是以 or 连接
        /// </summary>
        /// <param name="datatype"></param>
        /// <param name="ctype"></param>
        /// <param name="dtype"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="Fields"></param>
        public SearchItem(FilterDataType datatype, CompareType ctype, DateCompareType dtype, object v1, object v2, SearchFieldInfo[] Fields)
            :this(datatype,v1,Fields)
        {
            value2 = v2.IsNull() ? DBNull.Value : v2;
            compareType = ctype;
            dateCompareType = dtype;            
        }
        #endregion
        #endregion

        /// <summary>
        /// 追加检索字段
        /// </summary>
        public void AppendSearchField(string fieldName,string FieldCaption)
        {   
            SearchFields.Add(new SearchFieldInfo
            {
                FieldName = fieldname,
                Caption = FieldCaption
            });
        }
        public void AppendSearchField(string fieldName)
        {
            SearchFields.Add(new SearchFieldInfo
            {
                FieldName = fieldname,
                Caption = fieldName
            });
        }
        public void AppendSearchField(SearchFieldInfo field)
        {
            SearchFields.Add(field);
        }

        #region 公共属性


        public string DateFormat
        {
            get { return dateFormat; }
            set { dateFormat = value; }
        }

        /// <summary>
        /// 当前检索项前缀，默认为 And
        /// </summary>
        public SqlPrefixType SqlPrefixType
        {
            get { return sqlPrefixType; }
            set { sqlPrefixType = value; }
        }

        /// <summary>
        /// 默认的参照ID，该项主要用于在UI中设置检索项
        /// </summary>
        public Guid DefineQuery
        {
            get { return defineQuery; }
            set { defineQuery = value; }
        }
        /// <summary>
        /// 检索项ID，用于标识当前检索项的唯一性
        /// </summary>
        public string ItemID
        {
            get { return itemID; }
            set { itemID = value; }
        }

        /// <summary>
        /// 检索项名称，也是标题
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        /// <summary>
        /// 检索的字段名
        /// </summary>
        public string FieldName
        {
            get { return fieldname; }
            set { fieldname = value; }
        }
        /// <summary>
        /// 检索项数据类型
        /// </summary>
        public FilterDataType DataType
        {
            get { return dataType; }
            set { dataType = value; }
        }

        /// <summary>
        /// 数据比较类型
        /// </summary>
        public CompareType CompareType
        {
            get { return compareType; }
            set { compareType = value; }
        }
        private DateCompareType dateCompareType;
        public DateCompareType DateCompareType
        {
            get { return dateCompareType; }
            set { dateCompareType = value; }
        }

        /// <summary>
        /// 比较的第一个值
        /// </summary>
        public object Value1
        {
            get { return value1; }
            set
            {
                if (value == null)
                    value1 = DBNull.Value;
                else
                    value1 = value;
            }
        }
        /// <summary>
        /// 比较的第二个值:只有当为之间比较时该值才有效
        /// </summary>
        public object Value2
        {
            get { return value2; }
            set
            {
                if (value == null)
                    value2 = DBNull.Value;
                else
                    value2 = value;
            }
        }
        private string valueField;
        public string ValueField
        {
            get { return valueField; }
            set { valueField = value; }
        }
        private string value2Field;
        public string Value2Field
        { 
            get{return value2Field;}
            set{value2Field=value;}
        }

        private bool isMultiColumn=false;
        /// <summary>
        /// 是否为多列检索项:注,该项仅限于SearchFields不为空
        /// </summary>
        public bool IsMultiColumn
        {
            get { return isMultiColumn; }
            set { isMultiColumn = value; }
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 获取参考数据源
        /// </summary>
        /// <returns></returns>
        public DataTable GetDataSource()
        {
            if (getDataSource != null)
                return getDataSource();
            else
                return null;            
        }
        #endregion

        #region 生成本项查询过滤条件

        #region 得出SQL语句的表达式

        public string ToSqlExpress()
        {
            return "";
        }
        #endregion

        #region 得出SQL语句过滤条件

        /// <summary>
        /// 以默认数据驱动生成SQL过滤语句
        /// </summary>
        /// <returns></returns>
        public string ToSqlFilter()
        {
            return ToSqlFilter(DbUtils.NewDB());
        }

        /// <summary>
        /// 将当前过滤条件项转换为指定SQL过滤条件字符
        /// </summary>
        /// <returns></returns>
        public string ToSqlFilter(DbUtils DB)
        {
            if (SqlPrefixType==SqlPrefixType.EndByGroup) return string.Empty;

            //以下为兼容旧版设计
            List<string> fs = new List<string>();
            if (!IsMultiColumn) //单项检索
            {
                string[] tfs = FieldName.Split(new char[] { '|' }); //旧版设计 
                if (tfs.Length == 1)
                    return GetItemFitler(FieldName, DB); //多列同时验证
                else
                    fs.AddRange(tfs);
            }
            else
            {
                if (SearchFields == null || SearchFields.Count == 0) //未设置多项列，则还是以FieldName来处理，即当成单项
                {
                    string[] tfs = FieldName.Split(new char[] { '|' }); //旧版设计 
                    if (tfs.Length == 1)
                        return GetItemFitler(FieldName, DB); //多列同时验证
                    else
                        fs.AddRange(tfs);
                }
                else
                {
                    foreach (string key in SearchFields.Keys)
                        fs.Add(SearchFields[key].FieldName);
                }
            }           

            string SqlFilter = "";
            foreach (string f in fs)
            {
               string filter = GetItemFitler(f,DB);
               if (filter.IsNotWhiteSpace())
               {
                   if (CompareType == CompareType.NoComprise || CompareType == CompareType.NoEqual || CompareType == CompareType.NotDBNull)
                   {
                       if (SqlFilter.IsNotWhiteSpace()) SqlFilter += " and ";
                   }
                   else
                   {
                       if (SqlFilter.IsNotWhiteSpace()) SqlFilter += " or ";
                   }
                   SqlFilter += filter;
               }
            }
            return SqlFilter;
        }
        /// <summary>
        /// 获取当前检索项查询条件
        /// </summary>
        /// <param name="item"></param>
        /// <param name="FieldName"></param>
        /// <returns></returns>
        private string GetItemFitler(string FN,DbUtils Db)
        {
            string filter = string.Empty;

            if (string.IsNullOrEmpty(FN))
                return filter;
            else if (Value1 == null)//没有值，表示条件无效
            {
                return filter;
            }
            if (DataType.IsIn<FilterDataType>(FilterDataType.Guid, FilterDataType.OpenWinGuid) && Value1.ToStringValue().IsWhiteSpace())
                Value1 = DBNull.Value;

            if (DataType.IsIn(FilterDataType.Guid, FilterDataType.OpenWinGuid) && Value1.ToStringValue() == Guid.Empty.ToString())
            {
                if (CompareType == CompareType.Equal)
                {
                    Value1 = DBNull.Value;
                    CompareType = CompareType.DBNull;
                }
                else if (CompareType == CompareType.NoEqual)
                {
                    Value1 = DBNull.Value;
                    CompareType = CompareType.NoEqual;
                }
            }

            if (Value1 == DBNull.Value)  //当检索值为空值时（1、未设置该检索项；2、该检索项确实为空值）
            {
                if (ValueField.IsWhiteSpace()) //非值表达式
                {
                    switch (CompareType)
                    {
                        case CompareType.DBNull://这里要为日期型单独列出
                            if (DataType == FilterDataType.String || DataType == FilterDataType.OpenWinString || DataType == FilterDataType.XPath) //or
                                filter = string.Format("({0}{2}{1})", Db.Function.IsNullSqlExp(FN), Db.Function.EqualsSqlExp(FN, "''"), Db.Function.OrSqlExp());
                            else
                                filter = Db.Function.IsNullSqlExp(FN);
                            break;
                        case CompareType.NotDBNull://这里要为日期型单独列出
                            if (DataType == FilterDataType.String || DataType == FilterDataType.OpenWinString || DataType == FilterDataType.XPath)  //and
                                filter = string.Format("({0}{2}{1})", Db.Function.NotIsNullSqlExp(FN), Db.Function.NotEqualsSqlExp(FN, "''"), Db.Function.AndSqlExp());
                            else
                                filter = Db.Function.NotIsNullSqlExp(FN);
                            break;
                    }
                    return filter;
                }
                else  //有值表达式
                {
                    if (Value2==DBNull.Value && Value2Field.IsNotWhiteSpace() && CompareType==CompareType.Between 
                        && DataType.IsIn<FilterDataType>(FilterDataType.DateTime,FilterDataType.Int,FilterDataType.Numeric))
                    {
                        filter=Db.Function.BetweenSqlExp(FN,ValueField,Value2Field);
                    }
                    else
                    {
                        switch (CompareType)
                        {
                            case CompareType.Equal:
                                filter = Db.Function.EqualsSqlExp(FN, ValueField);
                                break;
                            case CompareType.NoEqual:
                                filter = Db.Function.NotEqualsSqlExp(FN, ValueField);
                                break;
                            case CompareType.BigOrEqual:
                                filter = Db.Function.BigThenOrEqualsSqlExp(FN, ValueField);
                                break;
                            case CompareType.LessOrEqual:
                                filter = Db.Function.LessThenOrEqualsSqlExp(FN, ValueField);
                                break;
                            case CompareType.BigThen:
                                filter = Db.Function.BigThenSqlExp(FN, ValueField);
                                break;
                            case CompareType.LessThen:
                                filter = Db.Function.LessThenSqlExp(FN, ValueField);
                                break;                        
                        }
                    }
                    return filter;                    
                }
            }
            else if (Value2 == DBNull.Value && CompareType == CompareType.Between) //在之间，但未设第二个值，则视为无效
                return filter;

            if (Value1 is IList) //值为列表时，即多个值
            {
                if (CompareType == CompareType.Between) //这种情况对于between是无效的
                    return filter;
                else
                {
                    List<string> lists = new List<string>();
                    if (DataType != FilterDataType.DateTime && (CompareType == CompareType.Equal ||  CompareType ==CompareType.NoEqual))
                    {
                        IEnumerable vs = Value1 as IEnumerable;
                        foreach (object v in vs)
                        {
                            if (v != null || v != DBNull.Value || v.ToString() != "") //为实际有效值时
                            {
                                if (DataType == FilterDataType.Guid || DataType == FilterDataType.OpenWinGuid)
                                    lists.Add(Db.Function.GuidValue(v.ToGuid()));
                                else if (DataType == FilterDataType.String || DataType == FilterDataType.OpenWinString || DataType == FilterDataType.XPath)
                                    lists.Add(string.Format("'{0}'", v.SqlEncode()));
                                else
                                    lists.Add(string.Format("{0}", v));                                 
                            }
                        }
                        if (lists.Count == 0) //没有任何数据项
                            return filter;
                        else
                        {
                            if (CompareType==CompareType.Equal)
                                filter = Db.Function.InSqlExp(FN, string.Join(",", lists.ToArray()));
                            else
                                filter = Db.Function.NotInSqlExp(FN, string.Join(",", lists.ToArray()));
                            return filter;
                        }
                    }
                    else
                    {
                        IEnumerable vs = Value1 as IEnumerable;
                        foreach (object v in vs)
                        {
                            string list =GetFilter(FN, v, Db);
                            if (list.IsNotWhiteSpace())
                                lists.Add(list);
                        }
                        if (lists.Count == 0)
                            return filter;
                        else
                        {
                            filter = string.Format("({0})", string.Join(" or ", lists.ToArray()));
                            return filter;
                        }
                    }
                }
            }
            else
            {
                filter = GetFilter(FN, Value1, Db);
                return filter;
            }
        }

        /// <summary>
        /// 针对日期型，单独列出
        /// </summary>
        /// <param name="FieldName"></param>
        /// <param name="V1"></param>
        /// <param name="Db"></param>
        /// <returns></returns>
        private string GetDateFilter(string FN, object V1, DbUtils Db)
        {
            DateTime dtStart,dtEnd;
            if (this.DataType == FilterDataType.String||(this.DataType.IsIn(FilterDataType.Int,FilterDataType.Numeric)&&this.DateCompareType== DateCompareType.Year))
            {
                string vs1 = V1.ToStringValue();
                string vs2 = Value2.ToStringValue();

                if (DateCompareType == DateCompareType.Year)
                {
                    if (vs1.IsWhiteSpace())
                        dtStart = DateTime.MinValue;
                    else if (vs1.Length == 4)
                        dtStart = string.Format("{0}-01-01", vs1).ToDateTime();
                    else
                        dtStart = V1.ToDateTime();

                    if (vs2.IsWhiteSpace())
                        dtEnd = DateTime.MaxValue;
                    else if (vs2.Length == 4)
                        dtEnd = string.Format("{0}-01-01", vs2).ToDateTime();
                    else
                        dtEnd = Value2.ToDateTime();

                }
                else if (DateCompareType == DateCompareType.Month)
                {
                    if (vs1.IsWhiteSpace())
                        dtStart = DateTime.MinValue;
                    else if (vs1.Length == 7)
                        dtStart = string.Format("{0}-01", vs1).ToDateTime();
                    else if (vs1.Length == 6)
                        dtStart = string.Format("{0}-{1}-01", vs1.Substring(0, 4), vs1.Substring(4, 2)).ToDateTime();
                    else
                        dtStart = V1.ToDateTime();

                    if (vs2.IsWhiteSpace())
                        dtEnd = DateTime.MaxValue;
                    else if (vs2.Length == 7)
                        dtEnd = string.Format("{0}-01", vs2).ToDateTime();
                    else if (vs2.Length == 6)
                        dtEnd = string.Format("{0}-{1}-01", vs2.Substring(0, 4), vs2.Substring(4, 2)).ToDateTime();
                    else
                        dtEnd = Value2.ToDateTime();
                }
                else
                {
                    dtStart = V1.ToDateTime();
                    dtEnd = Value2.ToDateTime();
                }
            }
            else
            {
                dtStart = V1.ToDateTime();
                dtEnd = Value2.ToDateTime();
            }

            string yfExp=Db.Function.Year(FN);
            string mfExp=Db.Function.Month(FN);
            string filter = string.Empty;
            if (DataType == FilterDataType.String)
            {
                #region 检索字段类型为字符
                string fm="";
                if (DateFormat.IsWhiteSpace())
                {
                    switch (DateCompareType)
                    {
                        case Filter.DateCompareType.Date:
                            fm = "{0:yyyy-MM-dd}";
                            break;
                        case Filter.DateCompareType.DateTime:
                            fm = "{0:yyyy-MM-dd HH:mm:ss}";
                            break;
                        case Filter.DateCompareType.Month:
                            fm = "{0:yyyy-MM}";
                            break;
                        case Filter.DateCompareType.Year:
                            fm = "{0:yyyy}";
                            break;
                        default:
                            fm = "{0:yyyy-MM-dd}";
                            break;
                    }
                }
                else if (DateFormat.Trim().StartsWith("{0"))
                    fm = DateFormat;
                else
                    fm=string.Concat("{0:", DateFormat,"}");

                string vstr1 =string.Format(fm, dtStart).SqlEncode();
                string vstr2 =string.Format(fm, dtEnd).SqlEncode();
                switch (CompareType)
                { 
                    case Filter.CompareType.Between:
                        if (dtStart == DateTime.MinValue)
                        {
                            filter = string.Format("({0} {1} ({2}))", Db.Function.IsNullSqlExp(FN), Db.Function.OrSqlExp(), Db.Function.BigThenOrEqualsSqlExp(FN, string.Format("'{0}'", vstr1)) + Db.Function.AndSqlExp()
                                           + Db.Function.LessThenOrEqualsSqlExp(FN, string.Format("'{0}'", vstr2)));
                        }
                        else
                        {
                            filter = Db.Function.BigThenOrEqualsSqlExp(FN, string.Format("'{0}'", vstr1)) + Db.Function.AndSqlExp()
                                           + Db.Function.LessThenOrEqualsSqlExp(FN, string.Format("'{0}'", vstr2));
                        }
                        break;
                    case Filter.CompareType.BigOrEqual:
                        filter = Db.Function.BigThenOrEqualsSqlExp(FN, string.Format("'{0}'",vstr1));
                        break;
                    case Filter.CompareType.BigThen:
                        filter = Db.Function.BigThenSqlExp(FN, string.Format("'{0}'", vstr1));
                        break;
                    case Filter.CompareType.Chields:
                        filter = Db.Function.LeftLikeSqlExp(FN, vstr1);
                        break;
                    case Filter.CompareType.Comprise:
                        filter = Db.Function.LikeSqlExp(FN, vstr1);
                        break;
                    case Filter.CompareType.DBNull:
                        filter = Db.Function.IsNullSqlExp(FN)+Db.Function.OrSqlExp()+Db.Function.EqualsSqlExp(FN,"''");
                        break;
                    case Filter.CompareType.Equal:
                        filter = Db.Function.EqualsSqlExp(FN, string.Format("'{0}'", vstr1));
                        break;
                    case Filter.CompareType.LeftComprise:
                        filter = Db.Function.LeftLikeSqlExp(FN, vstr1);
                        break;
                    case Filter.CompareType.LessOrEqual:
                        filter = Db.Function.LessThenOrEqualsSqlExp(FN, string.Format("'{0}'", vstr1));
                        break;
                    case Filter.CompareType.LessThen:
                        filter = Db.Function.LessThenSqlExp(FN, string.Format("'{0}'", vstr1));
                        break;
                    case Filter.CompareType.NoComprise:
                        filter = Db.Function.NotLikeSqlExp(FN, vstr1);
                        break;
                    case Filter.CompareType.NoEqual:
                        filter = Db.Function.NotEqualsSqlExp(FN, string.Format("'{0}'", vstr1));
                        break;
                    case Filter.CompareType.NoLeftComprise:
                        filter = Db.Function.NotLeftLikeSqlExp(FN, vstr1);
                        break;
                    case Filter.CompareType.NoRightComprise:
                        filter = Db.Function.NotRightLikeSqlExp(FN, vstr1);
                        break;
                    case Filter.CompareType.NotDBNull:
                        filter = Db.Function.NotIsNullSqlExp(FN)+Db.Function.AndSqlExp()+Db.Function.NotEqualsSqlExp(FN,"''");
                        break;
                    case Filter.CompareType.RightComprise:
                        filter = Db.Function.RightLikeSqlExp(FN, vstr1);
                        break;
                }
                #endregion
            }
            else if (this.DataType.IsIn(FilterDataType.Int,FilterDataType.Numeric)&&this.DateCompareType== DateCompareType.Year)
            {
                int vstr1 = dtStart.Year;
                int vstr2 = dtEnd.Year;
                switch (CompareType)
                {
                    case Filter.CompareType.Between:
                        if (dtStart == DateTime.MinValue)
                        {
                            filter = string.Format("({0} {1} ({2}))", Db.Function.IsNullSqlExp(FN), Db.Function.OrSqlExp(), Db.Function.BigThenOrEqualsSqlExp(FN,vstr1) + Db.Function.AndSqlExp()
                                           + Db.Function.LessThenOrEqualsSqlExp(FN,vstr2));
                        }
                        else
                        {
                            filter = Db.Function.BigThenOrEqualsSqlExp(FN,vstr1) + Db.Function.AndSqlExp()
                                           + Db.Function.LessThenOrEqualsSqlExp(FN,vstr2);
                        }
                        break;
                    case Filter.CompareType.BigOrEqual:
                        filter = Db.Function.BigThenOrEqualsSqlExp(FN,vstr1);
                        break;
                    case Filter.CompareType.BigThen:
                        filter = Db.Function.BigThenSqlExp(FN, vstr1);
                        break;
                    case Filter.CompareType.Chields:
                        filter = Db.Function.EqualsSqlExp(FN, vstr1);
                        break;
                    case Filter.CompareType.Comprise:
                        filter = Db.Function.EqualsSqlExp(FN, vstr1);
                        break;
                    case Filter.CompareType.DBNull:
                        filter = Db.Function.IsNullSqlExp(FN) ;
                        break;
                    case Filter.CompareType.Equal:
                        filter = Db.Function.EqualsSqlExp(FN,vstr1);
                        break;
                    case Filter.CompareType.LeftComprise:
                        filter = Db.Function.EqualsSqlExp(FN, vstr1);
                        break;
                    case Filter.CompareType.LessOrEqual:
                        filter = Db.Function.LessThenOrEqualsSqlExp(FN, vstr1);
                        break;
                    case Filter.CompareType.LessThen:
                        filter = Db.Function.LessThenSqlExp(FN, vstr1);
                        break;
                    case Filter.CompareType.NoComprise:
                        filter = Db.Function.NotEqualsSqlExp(FN, vstr1);
                        break;
                    case Filter.CompareType.NoEqual:
                        filter = Db.Function.NotEqualsSqlExp(FN, vstr1);
                        break;
                    case Filter.CompareType.NoLeftComprise:
                        filter = Db.Function.NotEqualsSqlExp(FN, vstr1);
                        break;
                    case Filter.CompareType.NoRightComprise:
                        filter = Db.Function.NotEqualsSqlExp(FN, vstr1);
                        break;
                    case Filter.CompareType.NotDBNull:
                        filter = Db.Function.NotIsNullSqlExp(FN);
                        break;
                    case Filter.CompareType.RightComprise:
                        filter = Db.Function.EqualsSqlExp(FN, vstr1);
                        break;
                }
            }
            else
            {
                switch (CompareType)
                {
                    case Filter.CompareType.Between:
                        switch (DateCompareType)
                        {
                            case Filter.DateCompareType.Normal:
                            case Filter.DateCompareType.Date:
                                filter = Db.Function.BigThenOrEqualsSqlExp(FN, Db.Function.DateTimeValue(dtStart.Date)) + Db.Function.AndSqlExp()
                                       + Db.Function.LessThenSqlExp(FN, Db.Function.DateTimeValue(dtEnd.Date.AddDays(1)));
                                break;
                            case Filter.DateCompareType.DateTime:
                                filter = Db.Function.BetweenSqlExp(FN, Db.Function.DateTimeValue(dtStart), Db.Function.DateTimeValue(dtEnd));
                                break;
                            case Filter.DateCompareType.Month://月份比较//(ay>2011 or (ay=2011 and am>=7)) and (ay<2012 or (ay=2012 and am<=3))
                                filter = string.Format("({2} {1} ({3} {0} {4})) {0} ({5} {1} ({6} {0} {7}))",
                                    Db.Function.AndSqlExp(),//0:and
                                    Db.Function.OrSqlExp(),//1:or
                                    Db.Function.BigThenSqlExp(yfExp, dtStart.Year),//2:y>2011
                                    Db.Function.EqualsSqlExp(yfExp, dtStart.Year),//3:y=2011
                                    Db.Function.BigThenOrEqualsSqlExp(mfExp, dtStart.Month),//4:m>=2
                                    Db.Function.LessThenSqlExp(yfExp, dtEnd.Year),//5:y<2012
                                    Db.Function.EqualsSqlExp(yfExp, dtEnd.Year),//6:y=2012
                                    Db.Function.LessThenOrEqualsSqlExp(mfExp, dtEnd.Month));//7:m<=2
                                break;
                            case Filter.DateCompareType.Year:
                                filter = Db.Function.BetweenSqlExp(yfExp, dtStart.Year, dtEnd.Year);
                                break;
                        }
                        break;
                    case Filter.CompareType.BigOrEqual:
                        switch (DateCompareType)
                        {
                            case Filter.DateCompareType.Normal:
                            case Filter.DateCompareType.Date:
                                filter = Db.Function.BigThenOrEqualsSqlExp(FN, Db.Function.DateTimeValue(dtStart.Date));
                                break;
                            case Filter.DateCompareType.DateTime:
                                filter = Db.Function.BigThenOrEqualsSqlExp(FN, Db.Function.DateTimeValue(dtStart));
                                break;
                            case Filter.DateCompareType.Month://月份比较//(ay>2011 or (ay=2011 and am>=7))
                                filter = string.Format("({2} {1} ({3} {0} {4}))",
                                    Db.Function.AndSqlExp(),//0:and
                                    Db.Function.OrSqlExp(),//1:or
                                    Db.Function.BigThenSqlExp(yfExp, dtStart.Year),//2:y>2011
                                    Db.Function.EqualsSqlExp(yfExp, dtStart.Year),//3:y=2011
                                    Db.Function.BigThenOrEqualsSqlExp(mfExp, dtStart.Month));//4:m>=2
                                break;
                            case Filter.DateCompareType.Year:
                                filter = Db.Function.BigThenOrEqualsSqlExp(yfExp, dtStart.Year);
                                break;
                        }
                        break;
                    case Filter.CompareType.BigThen:
                        switch (DateCompareType)
                        {
                            case Filter.DateCompareType.Normal:
                            case Filter.DateCompareType.Date:
                                filter = Db.Function.BigThenSqlExp(FN, Db.Function.DateTimeValue(dtStart.Date));
                                break;
                            case Filter.DateCompareType.DateTime:
                                filter = Db.Function.BigThenSqlExp(FN, Db.Function.DateTimeValue(dtStart));
                                break;
                            case Filter.DateCompareType.Month://月份比较//(ay>2011 or (ay=2011 and am>=7)) and (ay<2012 or (ay=2012 and am<=3))
                                filter = string.Format("({2} {1} ({3} {0} {4}))",
                                    Db.Function.AndSqlExp(),//0:and
                                    Db.Function.OrSqlExp(),//1:or
                                    Db.Function.BigThenSqlExp(yfExp, dtStart.Year),//2:y>2011
                                    Db.Function.EqualsSqlExp(yfExp, dtStart.Year),//3:y=2011
                                    Db.Function.BigThenSqlExp(mfExp, dtStart.Month));//4:m>2
                                break;
                            case Filter.DateCompareType.Year:
                                filter = Db.Function.BigThenSqlExp(yfExp, dtStart.Year);
                                break;
                        }
                        break;
                    case Filter.CompareType.DBNull:
                        filter = Db.Function.IsNullSqlExp(FN);
                        break;
                    case Filter.CompareType.Equal:
                        switch (DateCompareType)
                        {
                            case Filter.DateCompareType.Normal:
                            case Filter.DateCompareType.Date:
                                filter = Db.Function.BigThenOrEqualsSqlExp(FN, Db.Function.DateTimeValue(dtStart.Date)) + Db.Function.AndSqlExp()
                                       + Db.Function.LessThenSqlExp(FN, Db.Function.DateTimeValue(dtStart.Date.AddDays(1)));
                                break;
                            case Filter.DateCompareType.DateTime:
                                filter = Db.Function.EqualsSqlExp(FN, Db.Function.DateTimeValue(dtStart));
                                break;
                            case Filter.DateCompareType.Month://月份比较//(ay>2011 or (ay=2011 and am>=7)) and (ay<2012 or (ay=2012 and am<=3))
                                filter = Db.Function.EqualsSqlExp(yfExp, dtStart.Year) + Db.Function.AndSqlExp() + Db.Function.EqualsSqlExp(mfExp, dtStart.Month);
                                break;
                            case Filter.DateCompareType.Year:
                                filter = Db.Function.EqualsSqlExp(yfExp, dtStart.Year);
                                break;
                        }
                        break;
                    case Filter.CompareType.LessOrEqual:
                        switch (DateCompareType)
                        {
                            case Filter.DateCompareType.Normal:
                            case Filter.DateCompareType.Date:
                                filter = Db.Function.LessThenSqlExp(FN, Db.Function.DateTimeValue(dtStart.AddDays(1).Date));
                                break;
                            case Filter.DateCompareType.DateTime:
                                filter = Db.Function.LessThenOrEqualsSqlExp(FN, Db.Function.DateTimeValue(dtStart));
                                break;
                            case Filter.DateCompareType.Month://月份比较//(ay<2011 or (ay=2011 and am<=7))
                                filter = string.Format("({2} {1} ({3} {0} {4}))",
                                    Db.Function.AndSqlExp(),//0:and
                                    Db.Function.OrSqlExp(),//1:or
                                    Db.Function.LessThenSqlExp(yfExp, dtStart.Year),//2:y<2011
                                    Db.Function.EqualsSqlExp(yfExp, dtStart.Year),//3:y=2011
                                    Db.Function.LessThenOrEqualsSqlExp(mfExp, dtStart.Month));//4:m>=2
                                break;
                            case Filter.DateCompareType.Year:
                                filter = Db.Function.LessThenOrEqualsSqlExp(yfExp, dtStart.Year);
                                break;
                        }
                        break;
                    case Filter.CompareType.LessThen:
                        switch (DateCompareType)
                        {
                            case Filter.DateCompareType.Normal:
                            case Filter.DateCompareType.Date:
                                filter = Db.Function.LessThenSqlExp(FN, Db.Function.DateTimeValue(dtStart.Date));
                                break;
                            case Filter.DateCompareType.DateTime:
                                filter = Db.Function.LessThenSqlExp(FN, Db.Function.DateTimeValue(dtStart));
                                break;
                            case Filter.DateCompareType.Month://月份比较//(ay<2011 or (ay=2011 and am<=7))
                                filter = string.Format("({2} {1} ({3} {0} {4}))",
                                    Db.Function.AndSqlExp(),//0:and
                                    Db.Function.OrSqlExp(),//1:or
                                    Db.Function.LessThenSqlExp(yfExp, dtStart.Year),//2:y<2011
                                    Db.Function.EqualsSqlExp(yfExp, dtStart.Year),//3:y=2011
                                    Db.Function.LessThenSqlExp(mfExp, dtStart.Month));//4:m>=2
                                break;
                            case Filter.DateCompareType.Year:
                                filter = Db.Function.LessThenSqlExp(yfExp, dtStart.Year);
                                break;
                        }
                        break;
                    case Filter.CompareType.NoEqual:
                        switch (DateCompareType)
                        {
                            case Filter.DateCompareType.Normal:
                            case Filter.DateCompareType.Date:
                                filter = Db.Function.LessThenSqlExp(FN, Db.Function.DateTimeValue(dtStart.Date)) + Db.Function.OrSqlExp()
                                       + Db.Function.BigThenOrEqualsSqlExp(FN, Db.Function.DateTimeValue(dtStart.Date.AddDays(1)));
                                break;
                            case Filter.DateCompareType.DateTime:
                                filter = Db.Function.NotEqualsSqlExp(FN, Db.Function.DateTimeValue(dtStart));
                                break;
                            case Filter.DateCompareType.Month://月份比较//(ay>2011 or (ay=2011 and am>=7)) and (ay<2012 or (ay=2012 and am<=3))
                                filter = Db.Function.NotEqualsSqlExp(yfExp, dtStart.Year) + Db.Function.AndSqlExp() + Db.Function.NotEqualsSqlExp(mfExp, dtStart.Month);
                                break;
                            case Filter.DateCompareType.Year:
                                filter = Db.Function.NotEqualsSqlExp(yfExp, dtStart.Year);
                                break;
                        }
                        break;
                    case Filter.CompareType.NotDBNull:
                        filter = Db.Function.NotIsNullSqlExp(FN);
                        break;
                }
            }
            return filter.IsNotWhiteSpace()?string.Format("({0})",filter):filter;
        }

        private string GetFilter(string FN, object V1, DbUtils Db)
        {
            SearchItem item=this;
            string filter = string.Empty;
            
            if (item.DataType == FilterDataType.DateTime) return GetDateFilter(FN, V1, Db);

            if (item.DataType == FilterDataType.String && item.DateCompareType.IsIn(DateCompareType.Date,DateCompareType.DateTime, DateCompareType.Month, DateCompareType.Year)) return GetDateFilter(FN, V1, Db);

            if (item.DataType == FilterDataType.Parament) //如果为自定义参数
            {
                filter = string.Format(FN, V1);
                return filter;
            }
            switch (item.CompareType)
            {
                case CompareType.Equal:
                    if (item.DataType == FilterDataType.Int || item.DataType == FilterDataType.Numeric)
                        filter =Db.Function.EqualsSqlExp(FN,V1);
                    else if (item.DataType == FilterDataType.Bool)
                    {
                        bool fv=V1.ToBoolean();
                        if (fv)
                            filter = Db.Function.EqualsSqlExp(FN,Db.Function.BooleanValue(fv));
                        else
                            filter = Db.Function.EqualsSqlExp(FN,Db.Function.BooleanValue(fv)) + Db.Function.OrSqlExp() + Db.Function.IsNullSqlExp(FN);
                    }
                    else
                    {
                        if (item.DataType == FilterDataType.Guid || item.DataType == FilterDataType.OpenWinGuid)
                        {
                            if (V1.IsNull() || V1.Equals(""))
                                filter = Db.Function.IsNullSqlExp(FN) + Db.Function.OrSqlExp() + Db.Function.EqualsSqlExp(FN, Db.Function.GuidValue(Guid.Empty));
                            else
                                filter = Db.Function.EqualsSqlExp(FN, Db.Function.GuidValue(V1.ToGuid()));
                        }
                        else
                        {
                            if (V1.IsNull() || V1.Equals(""))
                                filter = Db.Function.IsNullSqlExp(FN) + Db.Function.OrSqlExp() + Db.Function.EqualsSqlExp(FN,"''");
                            else
                                filter = Db.Function.EqualsSqlExp(FN,string.Format("'{0}'",V1.SqlEncode()));
                        }
                    }
                    break;
                case CompareType.NoEqual:
                    if (item.DataType == FilterDataType.Int || item.DataType == FilterDataType.Numeric)
                        filter =Db.Function.NotEqualsSqlExp(FN,V1)  + Db.Function.OrSqlExp() +Db.Function.IsNullSqlExp(FN);
                    else if (item.DataType == FilterDataType.Bool)
                    {
                        bool fv=V1.ToBoolean();
                        if (fv)
                            filter = Db.Function.NotEqualsSqlExp(FN,Db.Function.BooleanValue(fv)) + Db.Function.OrSqlExp() + Db.Function.IsNullSqlExp(FN);
                        else
                            filter = Db.Function.NotEqualsSqlExp(FN, Db.Function.BooleanValue(fv)) + Db.Function.AndSqlExp() + Db.Function.NotIsNullSqlExp(FN);
                    }
                    else
                    {
                        if (item.DataType == FilterDataType.Guid || item.DataType == FilterDataType.OpenWinGuid)
                        {
                            if (V1.IsNull() || V1.Equals(""))
                                filter = Db.Function.NotIsNullSqlExp(FN) + Db.Function.AndSqlExp() + Db.Function.NotEqualsSqlExp(FN, Db.Function.GuidValue(Guid.Empty));
                            else
                                filter = Db.Function.NotEqualsSqlExp(FN, Db.Function.GuidValue(V1.ToGuid())) + Db.Function.OrSqlExp() +Db.Function.IsNullSqlExp(FN);
                        }
                        else
                        {
                            if (V1.IsNull() || V1.Equals(""))
                                filter = Db.Function.NotIsNullSqlExp(FN) + Db.Function.AndSqlExp() + Db.Function.NotEqualsSqlExp(FN,"''");
                            else
                                filter = Db.Function.NotEqualsSqlExp(FN, string.Format("'{0}'", V1.SqlEncode())) + Db.Function.OrSqlExp() + Db.Function.IsNullSqlExp(FN); ;
                        }
                    }
                    break;
                case CompareType.Comprise: //包括，只对string有效
                    if (item.DataType == FilterDataType.String || item.DataType == FilterDataType.OpenWinString)
                    {
                        if (V1 == null || V1.Equals("")) //为空，则表示所有,则不计该条件
                            filter = string.Empty;// string.Format("({0} is null or {0} like '%{1}%')", FN, Value1);
                        else
                            filter = Db.Function.LikeSqlExp(FN, V1.SqlEncode());
                    }
                    break;
                case CompareType.LeftComprise: //包括，只对string有效
                    if (item.DataType == FilterDataType.String || item.DataType == FilterDataType.OpenWinString)
                    {
                        if (V1 == null || V1.Equals("")) //为空，则表示所有,则不计该条件
                            filter = string.Empty;// string.Format("({0} is null or {0} like '%{1}%')", FN, Value1);
                        else
                            filter = Db.Function.LeftLikeSqlExp(FN, V1.SqlEncode());
                    }
                    break;
                case CompareType.RightComprise: //包括，只对string有效
                    if (item.DataType == FilterDataType.String || item.DataType == FilterDataType.OpenWinString)
                    {
                        if (V1 == null || V1.Equals("")) //为空，则表示所有,则不计该条件
                            filter = string.Empty;// string.Format("({0} is null or {0} like '%{1}%')", FN, Value1);
                        else
                            filter = Db.Function.RightLikeSqlExp(FN, V1.SqlEncode());
                    }
                    break;
                case CompareType.NoComprise:
                    if (item.DataType == FilterDataType.String || item.DataType == FilterDataType.OpenWinString)
                    {
                        if (V1 == null || V1.Equals("")) //为空值
                            filter = Db.Function.NotIsNullSqlExp(FN) + Db.Function.AndSqlExp() + Db.Function.NotEqualsSqlExp(FN, "''");
                        else
                            filter = Db.Function.NotLikeSqlExp(FN, V1.SqlEncode()) + Db.Function.OrSqlExp() + Db.Function.IsNullSqlExp(FN);
                    }
                    break;
                case CompareType.NoLeftComprise:
                    if (item.DataType == FilterDataType.String || item.DataType == FilterDataType.OpenWinString)
                    {
                        if (V1 == null || V1.Equals("")) //为空值
                            filter = Db.Function.NotIsNullSqlExp(FN) + Db.Function.AndSqlExp() + Db.Function.NotEqualsSqlExp(FN, "''");
                        else
                            filter = Db.Function.NotLeftLikeSqlExp(FN, V1.SqlEncode()) + Db.Function.OrSqlExp() + Db.Function.IsNullSqlExp(FN);
                    }
                    break;
                case CompareType.NoRightComprise:
                    if (item.DataType == FilterDataType.String || item.DataType == FilterDataType.OpenWinString)
                    {
                        if (V1 == null || V1.Equals("")) //为空值
                            filter = Db.Function.NotIsNullSqlExp(FN) + Db.Function.AndSqlExp() + Db.Function.NotEqualsSqlExp(FN, "''");
                        else
                            filter = Db.Function.NotRightLikeSqlExp(FN, V1.SqlEncode()) + Db.Function.OrSqlExp() + Db.Function.IsNullSqlExp(FN);
                    }
                    break;
                case CompareType.BigOrEqual: //只对数值及日期有效或日期型字符有效
                    if (item.DataType == FilterDataType.Int || item.DataType == FilterDataType.Numeric)
                        filter =Db.Function.BigThenOrEqualsSqlExp(FN,V1);                    
                    else if (item.DateCompareType!=DateCompareType.Normal && item.DataType==FilterDataType.String)
                        filter =Db.Function.BigThenOrEqualsSqlExp(FN,Db.Function.StringValue(V1));
                    break;
                case CompareType.LessOrEqual:
                    if (item.DataType == FilterDataType.Int || item.DataType == FilterDataType.Numeric)
                        filter =Db.Function.LessThenOrEqualsSqlExp(FN,V1);
                     else if (item.DateCompareType!=DateCompareType.Normal && item.DataType==FilterDataType.String)
                        filter =Db.Function.LessThenOrEqualsSqlExp(FN,Db.Function.StringValue(V1));
                    break;
                case CompareType.BigThen:
                    if (item.DataType == FilterDataType.Int || item.DataType == FilterDataType.Numeric)
                        filter =Db.Function.BigThenSqlExp(FN,V1);
                     else if (item.DateCompareType!=DateCompareType.Normal && item.DataType==FilterDataType.String)
                        filter =Db.Function.BigThenSqlExp(FN,Db.Function.StringValue(V1));
                    break;
                case CompareType.LessThen:
                    if (item.DataType == FilterDataType.Int || item.DataType == FilterDataType.Numeric)
                        filter = Db.Function.LessThenSqlExp(FN, V1);
                    else if (item.DateCompareType != DateCompareType.Normal && item.DataType == FilterDataType.String)
                        filter = Db.Function.LessThenSqlExp(FN, Db.Function.StringValue(V1));
                    break;
                case CompareType.Between: //对日期，数值，字符有效
                    if (item.DataType == FilterDataType.Int || item.DataType == FilterDataType.Numeric)
                        filter = Db.Function.BetweenSqlExp(FN, V1, value2);
                    else if (item.DateCompareType != DateCompareType.Normal && item.DataType == FilterDataType.String)
                        filter = Db.Function.BetweenSqlExp(FN, Db.Function.StringValue(V1),Db.Function.StringValue(Value2));
                    break;
                case CompareType.Chields://相当于LeftComprise
                    if (item.DataType == FilterDataType.XPath)
                        filter =Db.Function.LeftLikeSqlExp(FN,V1.SqlEncode());
                    break;
                case CompareType.DBNull:
                    if (item.DataType == FilterDataType.String)
                        filter =Db.Function.IsNullSqlExp(FN)+Db.Function.OrSqlExp()+Db.Function.EqualsSqlExp(FN, "''");
                    else
                        filter = Db.Function.IsNullSqlExp(FN);
                    break;
                case CompareType.NotDBNull:
                    if (item.DataType == FilterDataType.String)
                        filter = Db.Function.NotIsNullSqlExp(FN) + Db.Function.AndSqlExp() + Db.Function.NotEqualsSqlExp(FN, "''");
                    else
                        filter = Db.Function.NotIsNullSqlExp(FN);
                    break;
            }
            string sql=filter.IsNotWhiteSpace() ? string.Format("({0})", filter) : filter;
            return sql;
        }

        #endregion
        #endregion

        #region Linq过滤        
        /// <summary>
        /// 获取当前数据的表达式
        /// </summary>
        /// <returns></returns>
        public bool ToLinkFilter(object o)
        {
            bool currtn = true; //当前检索项的值
            
            SearchItem item = this;
            if (item.SqlPrefixType == SqlPrefixType.EndByGroup) return true;


            if (!IsMultiColumn) //单字段
                currtn = GetItemWhereExp(o, this, FieldName);
            else
            {
                List<string> fs = new List<string>();
                if (SearchFields == null || SearchFields.Count == 0)
                    fs.Add(FieldName);
                else
                {
                    foreach (string key in SearchFields.Keys)
                        fs.Add(SearchFields[key].FieldName);
                }
                if (fs.Count <= 1)
                {
                    currtn = GetItemWhereExp(o, item, item.FieldName);
                }
                else
                {
                    bool r = true;
                    for (int j = 0; j < fs.Count; j++)
                    {
                        string f = fs[j];
                        if (j == 0)
                            r = GetItemWhereExp(o, item, f);
                        else
                        {
                            if (item.CompareType == CompareType.NoComprise || item.CompareType == CompareType.NoEqual || item.CompareType == CompareType.NotDBNull)
                                r = r && GetItemWhereExp(o, item, f);
                            else
                                r = r || GetItemWhereExp(o, item, f);
                        }
                    }
                    currtn = r;
                }
            }
            return currtn;
        }
        /// <summary>
        /// 获取指定属性的名称
        /// </summary>
        /// <returns></returns>
        protected object GetProValue(object o, string Name, Type ElemeType)
        {
            return ElemeType.GetProperty(Name).GetValue(o, null);
        }


        /// <summary>
        /// 获取一项的比较值
        /// </summary>
        /// <param name="item"></param>
        /// <param name="FieldName"></param>
        /// <returns></returns>
        private bool GetItemWhereExp(object o, SearchItem item, string FN)
        {
            bool rtn = true;
            if (string.IsNullOrEmpty(FN))
                return true;
            else if (item.Value1 == null)
            {
                return true;
            }
            object curObj = GetProValue(o, FN, o.GetType());

            if (curObj == null || curObj.Equals(DBNull.Value))
            {
                switch (item.CompareType)
                {
                    case CompareType.DBNull:
                        return true;
                    default:
                        return false;
                }
            }

            if (item.Value1 == DBNull.Value)
            {
                if (string.IsNullOrEmpty(item.ValueField))
                {
                    switch (item.CompareType)
                    {
                        case CompareType.DBNull:
                            if (curObj.IsNullOrDBNull())
                                rtn = true;
                            else
                            {
                                if (item.DataType == FilterDataType.String)
                                    rtn = string.IsNullOrEmpty(curObj.ToStringValue());
                                else if (item.DataType == FilterDataType.Guid)
                                    rtn = curObj.ToStringValue() == Guid.Empty.ToString();
                                else if (item.DataType == FilterDataType.DateTime)
                                    rtn = curObj.ToDateTime() == DateTime.MinValue;
                                else
                                    rtn = false;
                            }
                            break;
                        case CompareType.NotDBNull:
                            if (curObj.IsNullOrDBNull())
                                rtn = true;
                            else
                            {
                                if (item.DataType == FilterDataType.String)
                                    rtn = string.IsNullOrEmpty(curObj.ToStringValue());
                                else if (item.DataType == FilterDataType.Guid)
                                    rtn = curObj.ToStringValue() == Guid.Empty.ToString();
                                else if (item.DataType == FilterDataType.DateTime)
                                    rtn = curObj.ToDateTime() == DateTime.MinValue;
                                else
                                    rtn = false;
                            }
                            rtn = !rtn;
                            break;
                    }
                    return rtn;
                }
                else
                {
                    switch (item.CompareType)
                    {
                        case CompareType.Equal:
                            rtn = curObj.Equals(GetProValue(o, item.ValueField, o.GetType()));
                            break;
                        case CompareType.NoEqual:
                            rtn = (!curObj.Equals(GetProValue(o, item.ValueField, o.GetType())));
                            break;
                        case CompareType.BigOrEqual:
                            switch (item.DataType)
                            {
                                case FilterDataType.Int:
                                case FilterDataType.Numeric:
                                    rtn = curObj.ToDouble() >= GetProValue(o, item.ValueField, o.GetType()).ToDouble();
                                    break;
                                case FilterDataType.DateTime:
                                    rtn = curObj.ToDateTime() >= GetProValue(o, item.ValueField, o.GetType()).ToDateTime().Date;
                                    break;
                                default:
                                    rtn = false;
                                    break;
                            }
                            break;
                        case CompareType.LessOrEqual:
                            switch (item.DataType)
                            {
                                case FilterDataType.Int:
                                case FilterDataType.Numeric:
                                    rtn = curObj.ToDouble() <= GetProValue(o, item.ValueField, o.GetType()).ToDouble();
                                    break;
                                case FilterDataType.DateTime:
                                    rtn = curObj.ToDateTime() < GetProValue(o, item.ValueField, o.GetType()).ToDateTime().AddDays(1).Date;
                                    break;
                                default:
                                    rtn = false;
                                    break;
                            }
                            break;
                        case CompareType.BigThen:
                            switch (item.DataType)
                            {
                                case FilterDataType.Int:
                                case FilterDataType.Numeric:
                                    rtn = curObj.ToDouble() > GetProValue(o, item.FieldName, o.GetType()).ToDouble();
                                    break;
                                case FilterDataType.DateTime:
                                    rtn = curObj.ToDateTime() >= GetProValue(o, item.FieldName, o.GetType()).ToDateTime().AddDays(1).Date;
                                    break;
                                default:
                                    rtn = false;
                                    break;
                            }

                            break;
                        case CompareType.LessThen:
                            switch (item.DataType)
                            {
                                case FilterDataType.Int:
                                case FilterDataType.Numeric:
                                    rtn = curObj.ToDouble() < GetProValue(o, item.FieldName, o.GetType()).ToDouble();
                                    break;
                                case FilterDataType.DateTime:
                                    rtn = curObj.ToDateTime() < GetProValue(o, item.FieldName, o.GetType()).ToDateTime().Date;
                                    break;
                                default:
                                    rtn = false;
                                    break;
                            }
                            break;
                    }
                    return rtn;
                }
            }
            else if (item.Value2 == DBNull.Value && item.CompareType == CompareType.Between)
                return rtn;

            if (item.Value1 is IEnumerable)
            {
                if (item.CompareType == CompareType.Between)
                    return rtn;
                else
                {
                    int i = 0;
                    bool crtn = true;
                    IEnumerable lvs = (IEnumerable)item.Value1;
                    if (item.DataType != FilterDataType.DateTime && item.CompareType == CompareType.Equal)
                    {                        
                        foreach (object v in lvs)
                        {
                            if (v != null || v != DBNull.Value || v.ToString() != "")
                            {
                                if (i == 0)
                                    crtn = curObj.Equals(v);
                                else
                                    crtn = crtn || curObj.Equals(v);
                                i++;
                            }
                        }
                        rtn = crtn;
                    }
                    else
                    {
                        foreach (object v in lvs)
                        {
                            if (i == 0)
                                crtn = GetWhereExp(item, curObj, v);
                            else
                                crtn = crtn || GetWhereExp(item, curObj, v);
                        }
                        rtn = crtn;
                    }
                    return rtn;
                }
            }
            else
            {
                rtn = GetWhereExp(item, curObj, item.Value1);
                return rtn;
            }
        }

        private bool GetDateWhereExp(object ProValue, object V1)
        {
            DateTime dtStart = V1.ToDateTime();
            DateTime dtEnd = Value2.ToDateTime();

            DateTime factV = ProValue.ToDateTime();

            bool rtn = true;
            switch (CompareType)
            {
                case Filter.CompareType.Between:
                    switch (DateCompareType)
                    {
                        case Filter.DateCompareType.Normal:
                        case Filter.DateCompareType.Date:
                            rtn = factV >= dtStart.Date && factV < dtEnd.AddDays(1).Date;
                            break;
                        case Filter.DateCompareType.DateTime:
                            rtn = factV >= dtStart && factV <= dtEnd.AddDays(1);
                            break;
                        case Filter.DateCompareType.Month://月份比较//(ay>2011 or (ay=2011 and am>=7)) and (ay<2012 or (ay=2012 and am<=3))
                            rtn = (factV.Year > dtStart.Year || (factV.Year == dtStart.Year && factV.Month >= dtStart.Month)) &&
                                (factV.Year < dtEnd.Year || (factV.Year == dtEnd.Year && factV.Month <= dtEnd.Month));
                            break;
                        case Filter.DateCompareType.Year:
                            rtn = factV.Year >= dtStart.Year && factV.Year < dtEnd.Year;
                            break;
                    }
                    break;
                case Filter.CompareType.BigOrEqual:
                    switch (DateCompareType)
                    {
                        case Filter.DateCompareType.Normal:
                        case Filter.DateCompareType.Date:
                            rtn = factV >= dtStart.Date;
                            break;
                        case Filter.DateCompareType.DateTime:
                            rtn = factV >= dtStart;
                            break;
                        case Filter.DateCompareType.Month://月份比较//(ay>2011 or (ay=2011 and am>=7))
                            rtn = factV.Year > dtStart.Year || (factV.Year == dtStart.Year && factV.Month >= dtStart.Month);
                            break;
                        case Filter.DateCompareType.Year:
                            rtn = factV.Year >= dtStart.Year;
                            break;
                    }
                    break;
                case Filter.CompareType.BigThen:
                    switch (DateCompareType)
                    {
                        case Filter.DateCompareType.Normal:
                        case Filter.DateCompareType.Date:
                            rtn = factV >= dtStart.AddDays(1).Date;
                            break;
                        case Filter.DateCompareType.DateTime:
                            rtn = factV > dtStart;
                            break;
                        case Filter.DateCompareType.Month://月份比较//(ay>2011 or (ay=2011 and am>=7)) and (ay<2012 or (ay=2012 and am<=3))
                            rtn = factV.Year > dtStart.Year || (factV.Year == dtStart.Year && factV.Month > dtStart.Month);
                            break;
                        case Filter.DateCompareType.Year:
                            rtn = factV.Year > dtStart.Year;
                            break;
                    }
                    break;
                case Filter.CompareType.DBNull:
                    rtn = factV.Equals(DateTime.MinValue);
                    break;
                case Filter.CompareType.Equal:
                    switch (DateCompareType)
                    {
                        case Filter.DateCompareType.Normal:
                        case Filter.DateCompareType.Date:
                            rtn = factV >= dtStart.Date && factV < dtStart.AddDays(1).Date;
                            break;
                        case Filter.DateCompareType.DateTime:
                            rtn = (factV == dtStart);
                            break;
                        case Filter.DateCompareType.Month://月份比较//(ay>2011 or (ay=2011 and am>=7)) and (ay<2012 or (ay=2012 and am<=3))
                            rtn = factV.Year == dtStart.Year && factV.Month == dtStart.Month;
                            break;
                        case Filter.DateCompareType.Year:
                            rtn = factV.Year == dtStart.Year;
                            break;
                    }
                    break;
                case Filter.CompareType.LessOrEqual:
                    switch (DateCompareType)
                    {
                        case Filter.DateCompareType.Normal:
                        case Filter.DateCompareType.Date:
                            rtn = factV < dtStart.AddDays(1).Date;
                            break;
                        case Filter.DateCompareType.DateTime:
                            rtn = factV <= dtStart;
                            break;
                        case Filter.DateCompareType.Month://月份比较//(ay<2011 or (ay=2011 and am<=7))
                            rtn = factV.Year < dtStart.Year || (factV.Year == dtStart.Year && factV.Month <= dtStart.Month);
                            break;
                        case Filter.DateCompareType.Year:
                            rtn = factV.Year <= dtStart.Year;
                            break;
                    }
                    break;
                case Filter.CompareType.LessThen:
                    switch (DateCompareType)
                    {
                        case Filter.DateCompareType.Normal:
                        case Filter.DateCompareType.Date:
                            rtn = factV < dtStart.Date;
                            break;
                        case Filter.DateCompareType.DateTime:
                            rtn = factV < dtStart;
                            break;
                        case Filter.DateCompareType.Month://月份比较//(ay<2011 or (ay=2011 and am<=7))
                            rtn = factV.Year < dtStart.Year || (factV.Year == dtStart.Year && factV.Month < dtStart.Month);
                            break;
                        case Filter.DateCompareType.Year:
                            rtn = factV.Year < dtStart.Year;
                            break;
                    }
                    break;
                case Filter.CompareType.NoEqual:
                    switch (DateCompareType)
                    {
                        case Filter.DateCompareType.Normal:
                        case Filter.DateCompareType.Date:
                            rtn = factV < dtStart.Date || factV >= dtStart.AddDays(1).Date;
                            break;
                        case Filter.DateCompareType.DateTime:
                            rtn = !factV.Equals(dtStart);
                            break;
                        case Filter.DateCompareType.Month://月份比较//(ay>2011 or (ay=2011 and am>=7)) and (ay<2012 or (ay=2012 and am<=3))
                            rtn = factV.Year != dtStart.Year && factV.Month != dtStart.Month;
                            break;
                        case Filter.DateCompareType.Year:
                            rtn = factV.Year != dtStart.Year;
                            break;
                    }
                    break;
                case Filter.CompareType.NotDBNull:
                    rtn =! factV.Equals(DateTime.MinValue);
                    break;
            }
            return rtn;        
        }


        private bool GetWhereExp(SearchItem item, object ProValue, object V1)
        {
            bool rtn = true;
            if (item.DataType == FilterDataType.Parament)
            {
                return rtn;
            }
            else if (item.DataType == FilterDataType.DateTime)
                return GetDateWhereExp(ProValue, V1);

            switch (item.CompareType)
            {
                case CompareType.Equal:
                    rtn = ProValue.Equals(V1);
                    break;
                case CompareType.NoEqual:
                    rtn = !ProValue.Equals(V1);
                    break;
                case CompareType.Comprise:
                    if (item.DataType == FilterDataType.String || item.DataType == FilterDataType.OpenWinString)
                    {
                        if (V1 == null || V1.Equals(""))
                            rtn = string.IsNullOrEmpty((string)ProValue);
                        else
                            rtn = ((string)ProValue).IndexOf(V1.ToString()) >= 0;
                    }
                    break;
                case Filter.CompareType.LeftComprise:
                    if (item.DataType == FilterDataType.String || item.DataType == FilterDataType.OpenWinString)
                    {
                        if (V1 == null || V1.Equals(""))
                            rtn = string.IsNullOrEmpty((string)ProValue);
                        else
                            rtn = ((string)ProValue).StartsWith(V1.ToString());
                    }
                    break;
                case Filter.CompareType.RightComprise:
                    if (item.DataType == FilterDataType.String || item.DataType == FilterDataType.OpenWinString)
                    {
                        if (V1 == null || V1.Equals(""))
                            rtn = string.IsNullOrEmpty((string)ProValue);
                        else
                            rtn = ((string)ProValue).EndsWith(V1.ToString());
                    }
                    break;
                case CompareType.NoComprise:
                    if (item.DataType == FilterDataType.String || item.DataType == FilterDataType.OpenWinString)
                    {
                        if (V1 == null || V1.Equals(""))
                            rtn = ((string)ProValue).IsNotEmpty();
                        else
                            rtn = ((string)ProValue).IndexOf(V1.ToString()) ==-1;
                    }
                    break;
                case Filter.CompareType.NoLeftComprise:
                    if (item.DataType == FilterDataType.String || item.DataType == FilterDataType.OpenWinString)
                    {
                        if (V1 == null || V1.Equals(""))
                            rtn = ((string)ProValue).IsNotEmpty();
                        else
                            rtn = !((string)ProValue).StartsWith(V1.ToString());
                    }
                    break;
                case Filter.CompareType.NoRightComprise:
                    if (item.DataType == FilterDataType.String || item.DataType == FilterDataType.OpenWinString)
                    {
                        if (V1 == null || V1.Equals(""))
                            rtn = ((string)ProValue).IsNotEmpty();
                        else
                            rtn = !((string)ProValue).EndsWith(V1.ToString());
                    }
                    break;
                case CompareType.BigOrEqual:
                    if (item.DataType == FilterDataType.Int || item.DataType == FilterDataType.Numeric)
                        rtn = ProValue.ToDouble() >= V1.ToDouble();
                    else if (item.DateCompareType != DateCompareType.Normal && item.DataType == FilterDataType.String)
                        rtn = ((string)ProValue).CompareTo(V1)>=0;
                    break;
                case CompareType.LessOrEqual:
                    if (item.DataType == FilterDataType.Int || item.DataType == FilterDataType.Numeric)
                        rtn =ProValue.ToDouble()<=V1.ToDouble();
                    else if (item.DateCompareType != DateCompareType.Normal && item.DataType == FilterDataType.String)
                        rtn = ((string)ProValue).CompareTo(V1) <= 0;
                    break;
                case CompareType.BigThen:
                    if (item.DataType == FilterDataType.Int || item.DataType == FilterDataType.Numeric)
                        rtn =ProValue.ToDouble()>V1.ToDouble();
                    else if (item.DateCompareType != DateCompareType.Normal && item.DataType == FilterDataType.String)
                        rtn = ((string)ProValue).CompareTo(V1) > 0;
                    break;
                case CompareType.LessThen:
                    if (item.DataType == FilterDataType.Int || item.DataType == FilterDataType.Numeric)
                        rtn =ProValue.ToDouble()<V1.ToDouble();
                    else if (item.DateCompareType != DateCompareType.Normal && item.DataType == FilterDataType.String)
                        rtn = ((string)ProValue).CompareTo(V1) < 0;
                    break;
                case CompareType.Between:
                    if (item.DataType == FilterDataType.Int || item.DataType == FilterDataType.Numeric)
                        rtn=ProValue.ToDouble()>=V1.ToDouble() && ProValue.ToDouble()<=Value2.ToDouble();
                    else if (item.DateCompareType != DateCompareType.Normal && item.DataType == FilterDataType.String)
                        rtn = ((string)ProValue).CompareTo(V1) >= 0 && ((string)ProValue).CompareTo(Value2) <= 0;;
                    break;
                case CompareType.Chields:
                    if (item.DataType == FilterDataType.XPath)
                        rtn = ((string)ProValue).StartsWith(Value1.ToString());
                    break;
                case CompareType.DBNull:
                    rtn = ProValue.Equals(DBNull.Value);
                    break;
                case CompareType.NotDBNull:
                    rtn = !ProValue.Equals(DBNull.Value);
                    break;
            }
            return rtn;
        }
        #endregion
    }    
}