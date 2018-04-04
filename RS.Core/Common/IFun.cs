using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;

namespace RS.Core
{
    /// <summary>
    /// 数据库驱动对应的函数接口
    /// </summary>
    public interface IFun
    {
        #region 特转换处理函数
        
        /// <summary>
        /// 将指定数据类型转换为实际数据库数据类型字符表达式
        /// </summary>
        /// <param name="type">数据类型</param>
        /// <param name="MaxLength">长度，默认为0，如小于或等于0，则不计</param>
        /// <param name="Decim">小数位，如小于或等于0，则不计，该类只限数据</param>
        /// <returns></returns>
        string DbTypeMapping(DbType type,int MaxLength=0,int Decim=0);

        /// <summary>
        /// 转换指定表名为标准SQL语句表名
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        string ConvertTable(string TableName);
        /// <summary>
        ///  转换指定表名为标准SQL语句表名
        /// </summary>
        /// <param name="ProcedureName"></param>
        /// <returns></returns>
        string ConvertProcedure(string ProcedureName);
        /// <summary>
        ///  转换指定表名为标准SQL语句表名
        /// </summary>
        /// <param name="ViewName"></param>
        /// <returns></returns>
        string ConvertView(string ViewName);
        /// <summary>
        ///  转换指定表名为标准SQL语句表名
        /// </summary>
        /// <param name="FunName"></param>
        /// <returns></returns>
        string ConvertFunction(string FunName);
        /// <summary>
        ///  转换指定字段名为标准SQL语句字段名
        /// </summary>
        /// <param name="FieldName"></param>
        /// <returns></returns>
        string ConvertField(string FieldName);
        string ConvertField(string TableName,string FieldName);
        /// <summary>
        /// 转换参数名为数据驱动所认参数名
        /// </summary>
        /// <param name="ParameterName"></param>
        /// <returns></returns>
        string ConvertParameter(string ParameterName);
        /// <summary>
        /// 指定日期值的SQL表达式,对于DateTime.MinValue 和DateTime.MaxValue ，应该作为空值处理
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        string DateTimeValue(DateTime time);
        
        /// <summary>
        /// 获取Guid类型字段表达式
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string GuidValue(Guid id);
        /// <summary>
        /// 获取Guid类型字段表达式
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string BooleanValue(bool b);
        string StringValue(object v);

        DbParameter CreateParameter(string Name, object Value);
        #endregion

        #region 相关日常函数表达式，主要用于构造SQL语句
        //聚合函数
        string Avg(string ValueExp);
        string Count(string FieldExp);
        string Max(string FieldExp);
        string Min(string FieldExp);
        string Sum(string FieldExp);
        //日期和时间函数
        string Datename(DatepartType datepart, string FieldExp);
        string DateAdd(DatepartType datepart,int AddValue,string FieldExp);
        string Datediff(DatepartType datepart, string BeginFieldExp, string EndFieldExp);
        string Datepart(DatepartType datepart,string FieldExp);
        string Day(string FieldExp);
        string Getdate();
        string Isdate(string FieldExp);
        string Month(string FieldExp);
        string Year(string FieldExp);
        //数学函数
        string Abs(string FieldExp);
        string IsNumeric(string FieldExp);
        //字符串函数
        string Charindex(string StrExp,string FieldExp,int StartIndex=0);
        string Left(string FieldExp,int len);
        string Len(string FieldExp);
        string Lower(string FieldExp);
        string LTrim(string FieldExp);
        string Replace(string FieldExp,string oldStrExp,string newStrExp);
        string Replicate(string Exp,int len);
        string Right(string FieldExp,int len);
        string RTrim(string FieldExp);
        string Space(int len);
        /// <summary>
        /// 以SQLServer为标准，索引是从1开始
        /// </summary>
        /// <param name="FieldExp"></param>
        /// <param name="BeginIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        string Substring(string FieldExp,int BeginIndex,int length);
        string Substring(string FieldExp, string BeginIndexExp, string LengthExp);
        string Upper(string FieldExp);
        #endregion
        #region 一些语法表达式
        string CaseWhen(string elseValueExp="",params CaseWhenUtils[] CaseWhens);
        #endregion
        #region SQL语句中相关过滤字符表达式
        string LikeSqlExp(string FieldExp, object ValueExp);
        string LeftLikeSqlExp(string FieldExp, object ValueExp);
        string RightLikeSqlExp(string FieldExp, object ValueExp);

        string NotLikeSqlExp(string FieldExp, object ValueExp);
        string NotLeftLikeSqlExp(string FieldExp, object ValueExp);
        string NotRightLikeSqlExp(string FieldExp, object ValueExp);

        string IsNullSqlExp(string FieldExp);
        string NotIsNullSqlExp(string FieldExp);

        string EqualsSqlExp(string FieldExp, object ValueExp);
        string NotEqualsSqlExp(string FieldExp, object ValueExp);

        string BigThenSqlExp(string FieldExp, object ValueExp);
        string BigThenOrEqualsSqlExp(string FieldExp, object ValueExp);

        string LessThenSqlExp(string FieldExp, object ValueExp);
        string LessThenOrEqualsSqlExp(string FieldExp, object ValueExp);

        string BetweenSqlExp(string FieldExp, object BeginValueExp, object EndValueExp);
        string NotBetweenSqlExp(string FieldExp, object BeginValueExp, object EndValueExp);
        string InSqlExp(string FieldExp, string InContentExp);
        string InSqlExp(string FieldExp, List<double> InListExp);
        string InSqlExp(string FieldExp, List<Guid> InValues);
        string InSqlExp(string FieldExp, List<string> InValues);

        string NotInSqlExp(string FieldExp, string InContentExp);
        string NotInSqlExp(string FieldExp, List<double> InListExp);
        string NotInSqlExp(string FieldExp, List<Guid> InValues);
        string NotInSqlExp(string FieldExp, List<string> InValues);       
        #endregion

        #region 用于连接的表达式
        string AndSqlExp();
        string OrSqlExp();
        #endregion

        string Convert(TypeCode Type, string FieldExp);

        string Top(int z);

        string GetPageSQL(string SelectCommand, PageInfo page, IDbDriver db, List<OrderItem> Orders = null);
        string GetPageSQL(string SelectCommand, int PageIndex, int PageSize, List<OrderItem> Orders=null);
    }

    public enum DatepartType
    { 
        /// <summary>
        /// 年
        /// </summary>
        Year, 
        /// <summary>
        /// 月
        /// </summary>
        Month ,
        /// <summary>
        /// 日
        /// </summary>
        Day ,
        /// <summary>
        /// 时
        /// </summary>
        Hour ,
        /// <summary>
        /// 分
        /// </summary>
        Minute,
        /// <summary>
        /// 秒
        /// </summary>
        Second ,
        /// <summary>
        /// 毫秒
        /// </summary>
        Millisecond 
    }

    public sealed class CaseWhenUtils
    {
        public static CaseWhenUtils NewCaseWhen(string whenExp, string thenExp)
        {
            return new CaseWhenUtils { 
             WhenExp=whenExp,
              ThenExp=thenExp
            };            
        }
        public string WhenExp { get; set; }
        public string ThenExp { get; set; }
    }
}
