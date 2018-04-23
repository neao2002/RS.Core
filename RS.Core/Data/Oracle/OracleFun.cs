using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data.OracleClient;
using System.Data;

namespace RS.Core.Data
{
    internal class OracleFun : IFun
    {
        /// <summary>
        /// 将指定数据类型转换为实际数据库数据类型字符表达式
        /// </summary>
        /// <param name="type">数据类型</param>
        /// <param name="MaxLength">长度，默认为0，如小于或等于0，则不计</param>
        /// <param name="Decim">小数位，如小于或等于0，则不计，该类只限数据</param>
        /// <returns></returns>
        public string DbTypeMapping(DbType type, int MaxLength = 0, int Decim = 0)
        {
            switch (type)
            {
                case DbType.AnsiString:
                    return "varchar" + (MaxLength > 0 ? string.Format("({0})", MaxLength) : "");
                case DbType.Binary:
                    return "BLOB";
                case DbType.Byte:
                    return "byte";
                case DbType.Boolean:
                    return "smallint";
                case DbType.Currency:
                    return "decimal(18,2)";
                case DbType.Date:
                    return "datetime";
                case DbType.DateTime:
                    return "datetime";
                case DbType.Decimal:
                    return "decimal" + (MaxLength > 0 ? string.Format("({0},{1})", MaxLength, Decim < 0 ? 0 : Decim) : "(18,2)");
                case DbType.Double:
                    return "numeric" + (MaxLength > 0 ? string.Format("({0},{1})", MaxLength, Decim < 0 ? 0 : Decim) : "(18,2)");
                case DbType.Guid:
                    return "VARCHAR(36)";
                case DbType.Int16:
                    return "int";
                case DbType.Int32:
                    return "int";
                case DbType.Int64:
                    return "long";
                case DbType.Object:
                    return "CLOB";
                case DbType.SByte:
                    return "int";
                case DbType.Single:
                    return "numeric" + (MaxLength > 0 ? string.Format("({0},{1})", MaxLength, Decim < 0 ? 0 : Decim) : "(10,2)");
                case DbType.String:
                    return "VARCHAR2" + (MaxLength > 0 ? string.Format("({0})", MaxLength) : "");
                case DbType.Time:
                    return "datetime";
                case DbType.UInt16:
                    return "int";
                case DbType.UInt32:
                    return "int";
                case DbType.UInt64:
                    return "long";
                case DbType.VarNumeric:
                    return "numeric" + (MaxLength > 0 ? string.Format("({0},{1})", MaxLength, Decim < 0 ? 0 : Decim) : "(18,2)");
                case DbType.AnsiStringFixedLength:
                    return "CLOB";
                case DbType.StringFixedLength:
                    return "NCLOB";
                case DbType.Xml:
                    return "CLOB";
                case DbType.DateTime2:
                    return "datetime";
                case DbType.DateTimeOffset:
                    return "datetime";
                default:
                    return "varchar(254)";
            }

        }


        public string ConvertTable(string TableName)
        {
            return string.Format("\"{0}\"", TableName);
        }

        public string ConvertProcedure(string ProcedureName)
        {
            return ProcedureName;
        }

        public string ConvertView(string ViewName)
        {
            return ViewName;
        }
        public string ConvertField(string TableName, string FieldName)
        {
            return string.Format("\"{0}\".\"{1}\"", TableName, FieldName);
        }
        public string ConvertField(string FieldName)
        {
            return string.Format("\"{0}\"", FieldName);
        }
        public string ConvertFunction(string FunName)
        {
            return string.Format("{0}", FunName);
        }
        public string ConvertParameter(string ParameterName)
        {
            if (ParameterName.IsNotWhiteSpace() && ParameterName.StartsWith(":")) return ParameterName;
            return string.Format(":{0}", ParameterName);
        }
        public string DateTimeValue(DateTime time)
        {
            if (time == DateTime.MinValue || time == DateTime.MaxValue)
                return "null";
            else
                return string.Format("to_date('{0:yyyy-MM-dd HH:mm:ss}','yyyy-mm-dd hh24:mi:ss')", time);
        }

        public string GuidValue(Guid id)
        {
            if (id==Guid.Empty)
                return "null";
            else
            return string.Format("'{0}'", id);
        }

        public string BooleanValue(bool b)
        {
            return b?"1":"0";
        }
        public DbParameter CreateParameter(string Name, object Value)
        {
            object v = Value;
            if (v == null) v = DBNull.Value;
            return new OracleParameter(ConvertParameter(Name), v);
        }

        public string Avg(string ValueExp)
        {
            return string.Format("Avg({0})", ValueExp);
        }

        public string Count(string FieldExp)
        {
            return string.Format("Count({0})", FieldExp);
        }

        public string Max(string FieldExp)
        {
            return string.Format("Max({0})", FieldExp);
        }

        public string Min(string FieldExp)
        {
            return string.Format("Min({0})", FieldExp);
        }

        public string Sum(string FieldExp)
        {
            return string.Format("Sum({0})", FieldExp);
        }
        private string ConvertDatepart(DatepartType datepart)
        {
            switch (datepart)
            {
                case DatepartType.Day:
                    return "day";
                case DatepartType.Hour:
                    return "hour";
                case DatepartType.Millisecond:
                    return "millisecond";
                case DatepartType.Minute:
                    return "minute";
                case DatepartType.Month:
                    return "month";
                case DatepartType.Second:
                    return "second";
                case DatepartType.Year:
                    return "year";
                default:
                    return "";
            }
        }
        public string Datename(DatepartType datepart, string FieldExp)
        {
            return string.Format("Datename({0},{1})", ConvertDatepart(datepart), FieldExp);
        }

        public string DateAdd(DatepartType datepart, int AddValue, string FieldExp)
        {
            return string.Format("DateAdd({0},{1},{2})", ConvertDatepart(datepart), AddValue, FieldExp);
        }

        public string Datediff(DatepartType datepart, string BeginFieldExp, string EndFieldExp)
        {
            return string.Format("Datediff({0},{1},{2})", ConvertDatepart(datepart), BeginFieldExp, EndFieldExp);
        }

        public string Datepart(DatepartType datepart, string FieldExp)
        {
            return string.Format("Datepart({0},{1})", ConvertDatepart(datepart), FieldExp);
        }

        public string Day(string FieldExp)
        {
            return string.Format("Day({0})", FieldExp);
        }

        public string Getdate()
        {
            return "getdate()";
        }

        public string Isdate(string FieldExp)
        {
            return string.Format("Isdate({0})", FieldExp);
        }

        public string Month(string FieldExp)
        {
            return string.Format("Month({0})", FieldExp);
        }

        public string Year(string FieldExp)
        {
            return string.Format("Year({0})", FieldExp);
        }

        public string Abs(string FieldExp)
        {
            return string.Format("Abs({0})", FieldExp);
        }

        public string Charindex(string StrExp, string FieldExp, int StartIndex = 0)
        {
            return string.Format("Charindex({0},{1},{2})", StrExp, FieldExp, StartIndex);
        }

        public string Left(string FieldExp, int len)
        {
            return string.Format("Left({0},{1})", FieldExp, len);
        }

        public string Len(string FieldExp)
        {
            return string.Format("Len({0})", FieldExp);
        }

        public string Lower(string FieldExp)
        {
            return string.Format("Lower({0})", FieldExp);
        }

        public string LTrim(string FieldExp)
        {
            return string.Format("LTrime({0})", FieldExp);
        }

        public string Replace(string FieldExp, string oldStrExp, string newStrExp)
        {
            return string.Format("Replace({0},{1},{2})", FieldExp, oldStrExp, newStrExp);
        }

        public string Replicate(string Exp, int len)
        {
            return string.Format("Replicate({0},{1})", Exp, len);
        }

        public string Right(string FieldExp, int len)
        {
            return string.Format("Right({0},{1})", FieldExp, len);
        }

        public string RTrim(string FieldExp)
        {
            return string.Format("RTrim({0})", FieldExp);
        }

        public string Space(int len)
        {
            return string.Format("Space({0})", len);
        }

        public string Substring(string FieldExp, int BeginIndex, int length)
        {
            return string.Format("Substring({0},{1},{2})", FieldExp, BeginIndex, length);
        }

        public string Upper(string FieldExp)
        {
            return string.Format("Upper({0})", FieldExp);
        }

        public string CaseWhen(string elseValueExp = "", params CaseWhenUtils[] CaseWhens)
        {
            if (!CaseWhens.HasElement()) return "";
            List<string> items = new List<string>();
            foreach (CaseWhenUtils cw in CaseWhens)
            {
                items.Add(string.Format("when {0} then {1}", cw.WhenExp, cw.ThenExp));
            }
            string elseW = elseValueExp.IsNull() ? "" : string.Format(" else {0} ", (elseValueExp.IsWhiteSpace() ? "''" : elseValueExp));

            return string.Concat(" case ", string.Join(" ", items.ToArray()), elseW, " end ");
        }

        public string LikeSqlExp(string FieldExp, object Value)
        {
            return string.Format("{0} like '%{1}%'", FieldExp, Value);
        }

        public string LeftLikeSqlExp(string FieldExp, object Value)
        {
            return string.Format("{0} like '{1}%'", FieldExp, Value);
        }

        public string RightLikeSqlExp(string FieldExp, object Value)
        {
            return string.Format("{0} like '%{1}'", FieldExp, Value);
        }

        public string NotLikeSqlExp(string FieldExp, object Value)
        {
            return string.Format("{0} not like '%{1}%'", FieldExp, Value);
        }

        public string NotLeftLikeSqlExp(string FieldExp, object Value)
        {
            return string.Format("{0} not like '{1}%'", FieldExp, Value);
        }

        public string NotRightLikeSqlExp(string FieldExp, object Value)
        {
            return string.Format("{0} not like '%{1}'", FieldExp, Value);
        }

        public string IsNullSqlExp(string FieldExp)
        {
            return string.Format("{0} is null", FieldExp);
        }

        public string NotIsNullSqlExp(string FieldExp)
        {
            return string.Format("{0} is not null", FieldExp);
        }

        public string EqualsSqlExp(string FieldExp, object ValueExp)
        {
            return string.Format("{0} = {1}", FieldExp, ValueExp);
        }

        public string NotEqualsSqlExp(string FieldExp, object ValueExp)
        {
            return string.Format("{0} <> {1}", FieldExp, ValueExp);
        }

        public string BigThenSqlExp(string FieldExp, object ValueExp)
        {
            return string.Format("{0} > {1}", FieldExp, ValueExp);
        }

        public string BigThenOrEqualsSqlExp(string FieldExp, object ValueExp)
        {
            return string.Format("{0} >= {1}", FieldExp, ValueExp);
        }

        public string LessThenSqlExp(string FieldExp, object ValueExp)
        {
            return string.Format("{0} < {1}", FieldExp, ValueExp);
        }

        public string LessThenOrEqualsSqlExp(string FieldExp, object ValueExp)
        {
            return string.Format("{0} <= {1}", FieldExp, ValueExp);
        }

        public string BetweenSqlExp(string FieldExp, object BeginValueExp, object EndValueExp)
        {
            return string.Format("{0} between {1} and {1}", FieldExp, BeginValueExp, EndValueExp);
        }

        public string NotBetweenSqlExp(string FieldExp, object BeginValueExp, object EndValueExp)
        {
            return string.Format("{0} not between {1} and {1}", FieldExp, BeginValueExp, EndValueExp);
        }
        public string InSqlExp(string FieldExp, string InContentExp)
        {
            return string.Format("{0} in ({1})", FieldExp, InContentExp);
        }
        public string InSqlExp(string FieldExp, List<double> InListExp)
        {
            return string.Format("{0} in ({1})", FieldExp, string.Join(",", InListExp.ConvertAll<string>(s => string.Concat("'", s, "'")).ToArray()));
        }
        public string InSqlExp(string FieldExp, List<Guid> InValues)
        {
            return string.Format("{0} in ({1})", FieldExp, string.Join(",", InValues.ConvertAll<string>(s => string.Concat("'", s, "'")).ToArray()));
        }
        public string InSqlExp(string FieldExp, List<string> InValues)
        {
            return string.Format("{0} in ({1})", FieldExp, string.Join(",", InValues.ConvertAll<string>(s => string.Concat("'", s, "'")).ToArray()));
        }

        public string NotInSqlExp(string FieldExp, string InContentExp)
        {
            return string.Format("({0} is null or {0} not in ({1}))", FieldExp, InContentExp);
        }
        public string NotInSqlExp(string FieldExp, List<double> InListExp)
        {
            return string.Format("({0} is null or {0} not in ({1}))", FieldExp, string.Join(",", InListExp.ConvertAll<string>(s => string.Concat("'", s, "'")).ToArray()));
        }
        public string NotInSqlExp(string FieldExp, List<Guid> InValues)
        {
            return string.Format("({0} is null or {0} not in ({1}))", FieldExp, string.Join(",", InValues.ConvertAll<string>(s => string.Concat("'", s, "'")).ToArray()));
        }
        public string NotInSqlExp(string FieldExp, List<string> InValues)
        {
            return string.Format("({0} is null or {0} not in ({1}))", FieldExp, string.Join(",", InValues.ConvertAll<string>(s => string.Concat("'", s, "'")).ToArray()));
        }


        public string AndSqlExp()
        {
            return " and ";
        }

        public string OrSqlExp()
        {
            return " or ";
        }


        public string StringValue(object v)
        {
            return string.Format("'{0}'", v.SqlEncode());
        }
        public string IsNumeric(string FieldExp)
        {
            return string.Format("IsNumeric({0})", FieldExp);
        }

        public string Convert(TypeCode TypeExp, string FieldExp)
        {
            return string.Format("Convert({0},{1})", TypeExp, FieldExp);
        }

        public string Top(int z)
        {
            if (z <= 0)
                return "";
            else
                return string.Format(" top {0} ", z);
        }

        public string Substring(string FieldExp, string BeginIndexExp, string LengthExp)
        {
            return string.Concat("substring(", FieldExp, ",", BeginIndexExp, ",", LengthExp, ")");
        }

        #region 分页相关
        public string GetPageSQL(string SelectCommand, PageInfo page, IDbDriver db, List<OrderItem> Orders = null)
        {
            if (Orders == null) Orders = new List<OrderItem>();
            if (page != null && page.PageSize > 0)
            {
                page.Totals = db.GetScalar(string.Format("SELECT COUNT(1) FROM ({0}) t1", SelectCommand)).ToLong();
                int pageIndex = page.PageIndex < 0 ? 0 : page.PageIndex;

                long bIndex = page.PageSize * pageIndex;
                long eIndex = page.PageSize * (pageIndex + 1);

                    string sql = string.Format(@"SELECT * FROM (
                SELECT totalA.*, rownum as rnum FROM  ({0}) totalA
            ) totalB WHERE rnum > {1} AND rnum <= {2}", SelectCommand, bIndex, eIndex);
                return sql;
            }
            else
               return SelectCommand;
        }
        public string GetPageSQL(string SelectCommand, int PageIndex, int PageSize, List<OrderItem> Orders = null)
        {
            if (Orders == null) Orders = new List<OrderItem>();
            PageInfo page = PageInfo.CreateNew(PageIndex, PageSize);
            if (page != null && page.PageSize > 0)
            {
                int pageIndex = page.PageIndex < 0 ? 0 : page.PageIndex;

                long bIndex = page.PageSize * pageIndex ;
                long eIndex = page.PageSize * (pageIndex + 1);

                string sql = string.Format(@"SELECT * FROM (
                SELECT totalA.*, rownum as rnum FROM  ({0}) totalA
            ) totalB WHERE rnum > {1} AND rnum <= {2} order by rnum", SelectCommand, bIndex, eIndex);
                return sql;
            }
            else
                return SelectCommand;
        }
        #endregion
    }
}
