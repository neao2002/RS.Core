using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;

namespace RS.Core.Data
{
    public class SqlFun : IFun
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
                    return "Binary";
                case DbType.Byte:
                    return "tinyint";
                case DbType.Boolean:
                    return "bit";
                case DbType.Currency:
                    return "money";
                case DbType.Date:
                    return "datetime";
                case DbType.DateTime:
                    return "datetime";
                case DbType.Decimal:
                    return "decimal" + (MaxLength > 0 ? string.Format("({0},{1})", MaxLength, Decim < 0 ? 0 : Decim) : "(18,2)");
                case DbType.Double:
                    return "numeric" + (MaxLength > 0 ? string.Format("({0},{1})", MaxLength, Decim < 0 ? 0 : Decim) : "(18,2)");
                case DbType.Guid:
                    return "uniqueidentifier";
                case DbType.Int16:
                    return "int";
                case DbType.Int32:
                    return "int";
                case DbType.Int64:
                    return "bigint";
                case DbType.Object:
                    return "ntext";
                case DbType.SByte:
                    return "smallint";
                case DbType.Single:
                    return "numeric" + (MaxLength > 0 ? string.Format("({0},{1})", MaxLength, Decim < 0 ? 0 : Decim) : "(10,2)");
                case DbType.String:
                    return "nvarchar" + (MaxLength > 0 ? string.Format("({0})", MaxLength) : "");
                case DbType.Time:
                    return "datetime";
                case DbType.UInt16:
                    return "int";
                case DbType.UInt32:
                    return "int";
                case DbType.UInt64:
                    return "bigint";
                case DbType.VarNumeric:
                    return "numeric" + (MaxLength > 0 ? string.Format("({0},{1})", MaxLength, Decim < 0 ? 0 : Decim) : "(18,2)");
                case DbType.AnsiStringFixedLength:
                    return "text";
                case DbType.StringFixedLength:
                    return "ntext";
                case DbType.Xml:
                    return "xml";
                case DbType.DateTime2:
                    return "datetime";
                case DbType.DateTimeOffset:
                    return "datetime";
                default:
                    return "nvarchar(254)";
            }

        }

        public string ConvertTable(string TableName)
        {
            if (TableName.Contains("..") || TableName.ToLower().Contains("with(nolock)") || TableName.Contains(".dbo.")||TableName.Contains("[")||TableName.Contains("]"))
                return TableName;
            else
                return string.Format("[{0}]", TableName);
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
            return string.Format("{0}.[{1}]",ConvertTable(TableName), FieldName);
        }
        public string ConvertField(string FieldName)
        {
            return string.Format("[{0}]", FieldName);
        }
        public string ConvertFunction(string FunName)
        {
            return string.Format("dbo.{0}", FunName);
        }
        public string ConvertParameter(string ParameterName)
        {
            if (ParameterName.IsNotWhiteSpace() && ParameterName.StartsWith("@")) return ParameterName;
            return string.Format("@{0}", ParameterName);
        }
        public string DateTimeValue(DateTime time)
        {
            if (time == DateTime.MinValue || time == DateTime.MaxValue)
                return "null";
            else if (time < new DateTime(1799, 1, 1) || time > new DateTime(9999, 12, 31))
                return "null";
            else
                return string.Format("Convert(datetime,'{0:yyyy-MM-dd HH:mm:ss}')", time);
        }

        public DbParameter CreateParameter(string Name, object Value)
        {
            object v = Value;
            if (v == null) v = DBNull.Value;
            return new SqlParameter(ConvertParameter(Name),v);
        }
        public string GuidValue(Guid id)
        {
            if (id == Guid.Empty)
                return "null";
            else
                return string.Format("'{0}'", id);
        }

        public string BooleanValue(bool b)
        {
            return b ? "1" : "0";
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
            string typeExp = "";
            switch (TypeExp)
            {
                case TypeCode.Boolean:
                    typeExp = "bit";
                    break;
                case TypeCode.Byte:
                    typeExp = "char";
                    break;
                case TypeCode.Char:
                    typeExp = "char";
                    break;
                case TypeCode.DateTime:
                    typeExp = "datetime";
                    break;
                case TypeCode.Decimal:
                    typeExp = "decimal(18,2)";
                    break;
                case TypeCode.Single:
                case TypeCode.Double:
                    typeExp = "numeric(18,2)";
                    break;
                case TypeCode.SByte:
                case TypeCode.Int16:
                    typeExp = "int";
                    break;
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    typeExp = "numeric(18,0)";
                    break;
                default:
                    typeExp = "nvarchar(254)";
                    break;
            }

            return string.Format("Convert({0},{1})", typeExp, FieldExp);
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
        public string GetPageSQL(string Sql, int PageIndex, int PageSize, List<OrderItem> Orders = null)
        {
            PageInfo page = PageInfo.CreateNew(PageIndex, PageSize);
            if (Orders == null) Orders = new List<OrderItem>();
            string SelectCommand = Sql;
            if (page != null && page.PageSize > 0)
            {
                //移除sql中的order by

                //增加对with的支持
                #region 增加对With的支持
                string withPerstr = "";
                if (SelectCommand.Trim().ToLower().StartsWith("with ")) //视其为with表达式
                {
                    string alias = "";
                    int b = SelectCommand.IndexOf("(");
                    int e = 0;
                    if (b > "with ".Length + " as ".Length) //为有效with表达式
                    {
                        withPerstr = SelectCommand.Substring(0, b).ToLower();
                        if (withPerstr.StartsWith("with ") && withPerstr.StartsWith(" as ") && withPerstr.Length > 9)
                            alias = withPerstr.Substring(5, withPerstr.Length - 9);

                        e = GetKHEndIndex(0, SelectCommand);
                        if (e == -1) //则视为无效，不理会
                            withPerstr = "";
                        else if (e >= SelectCommand.Length - 1) //为最后一个字符
                        {
                            SelectCommand = "select * from " + alias;
                        }
                        else
                        {
                            withPerstr = Sql.Substring(0, e + 1);
                            SelectCommand = Sql.Substring(e + 1);
                        }
                    }
                }
                #endregion
                string sql = "";
                string temp = SelectCommand.ToLower();
                int pos = temp.LastIndexOf(" order by ");
                int len = " order by ".Length;
                if (pos == -1)
                {
                    pos = temp.LastIndexOf("\r\norder by ");
                    len = "\r\norder by ".Length;
                    if (pos == -1)
                    {
                        pos = temp.LastIndexOf("\norder by ");
                        len = "\norder by ".Length;
                        if (pos == -1)
                        {
                            pos = temp.LastIndexOf("\norder by\r\n");
                            len = "\norder by\r\n".Length;
                            if (pos == -1)
                            {
                                pos = temp.LastIndexOf("\norder by\r");
                                len = "\norder by\r".Length;
                            }
                        }
                    }
                }

                int lstI = GetLastIndex(temp, "from");
                if (pos > 0 && lstI > pos) pos = -1;
                lstI = GetLastIndex(temp, "select");
                if (pos > 0 && lstI > pos) pos = -1;
                lstI = GetLastIndex(temp, "where");
                if (pos > 0 && lstI > pos) pos = -1;
                lstI = GetLastIndex(temp, "group by");
                if (pos > 0 && lstI > pos) pos = -1;

                if (pos > -1)
                {
                    sql = SelectCommand.Substring(0, pos);
                    string orderbys = SelectCommand.Substring(pos + len);
                    AppendOrders(orderbys, Orders);
                }
                else
                {
                    sql = SelectCommand;
                }

                if (Orders.Count == 0) //未设置
                {
                    pos = temp.IndexOf(" from ");
                    if (pos == -1)
                    {
                        pos = temp.IndexOf("\r\nfrom ");
                        if (pos == -1) pos = temp.IndexOf("\nfrom ");
                    }
                    if (pos > 7 && temp.StartsWith("select "))
                    {
                        string flist = temp.Substring(7, pos - 7);
                        string[] arr = flist.Split(',');
                        foreach (string f in arr)
                        {
                            if (f.IsNotWhiteSpace())
                            {
                                string vf = f;
                                int bindex = f.ToLower().IndexOf(" as ");
                                if (bindex > 0 && bindex + 4 < f.Length)
                                {
                                    vf = f.Substring(bindex + 4);
                                }
                                AppendOrders(vf, Orders);
                                break;
                            }
                        }
                    }
                }



                int pageIndex = page.PageIndex < 0 ? 0 : page.PageIndex;

                long bIndex = page.PageSize * pageIndex;
                long eIndex = page.PageSize * (pageIndex + 1);

                sql = string.Format(@"{4} 
select * from (
    select row_number() over({0}) as RecordNumberNo,totalAbyPage.* from ({1}) totalAbyPage
) totalBbyPage where RecordNumberNo>{2} and RecordNumberNo<={3}", DBHelper.GetOrderASCInfo(Orders), sql, bIndex, eIndex, withPerstr);

                
                return sql;
            }
            else
                return SelectCommand;
        }

        private int GetLastIndex(string str,string cstr)
        {
            int i = str.LastIndexOf(string.Format(" {0} ",cstr));
            if (i >= 0) return i;

            i = str.LastIndexOf(string.Format("\r\n{0} ", cstr));
            if (i >= 0) return i;

            i = str.LastIndexOf(string.Format("\n{0} ", cstr));
            if (i >= 0) return i;

            i = str.LastIndexOf(string.Format("\r\n{0}\r\n", cstr));
            if (i >= 0) return i;

            i = str.LastIndexOf(string.Format("\n{0}\r", cstr));
            if (i >= 0) return i;
            return i;
        }
        public string GetPageSQL(string Sql, PageInfo page, IDbDriver db, List<OrderItem> Orders = null)
        {
            string SelectCommand = Sql;
            if (Orders == null) Orders = new List<OrderItem>();
            if (page != null && page.PageSize > 0)
            {
                #region 增加对With的支持
                string withPerstr = "";
                if (SelectCommand.Trim().ToLower().StartsWith("with ")) //视其为with表达式
                {
                    string alias = "";
                    int b = SelectCommand.IndexOf("(");
                    int e = 0;
                    if (b > "with ".Length + " as ".Length) //为有效with表达式
                    {
                        withPerstr = SelectCommand.Substring(0, b).ToLower();
                        if (withPerstr.StartsWith("with ") && withPerstr.StartsWith(" as ") && withPerstr.Length > 9)
                            alias = withPerstr.Substring(5, withPerstr.Length - 9);

                        e = GetKHEndIndex(0, SelectCommand);
                        if (e == -1) //则视为无效，不理会
                            withPerstr = "";
                        else if (e >= SelectCommand.Length - 1) //为最后一个字符
                        {
                            SelectCommand = "select * from " + alias;
                        }
                        else
                        {
                            withPerstr = Sql.Substring(0, e + 1);
                            SelectCommand = Sql.Substring(e + 1);
                        }
                    }
                }
                #endregion

                //移除sql中的order by
                string sql = "";
                string temp = SelectCommand.ToLower();
                int pos = temp.LastIndexOf(" order by "); //最后一个order by ，可能这个order by是 selece中的一项，也可能是子查询中一项，所以这里还要区分其后面是否还有 from,where,)

                int len = " order by ".Length;
                if (pos == -1)
                {
                    pos = temp.LastIndexOf("\r\norder by ");
                    len = "\r\norder by ".Length;
                    if (pos == -1)
                    {
                        pos = temp.LastIndexOf("\norder by ");
                        len = "\norder by ".Length;

                        if (pos==-1)
                        {
                            pos = temp.LastIndexOf("\norder by\r\n");
                            len = "\norder by\r\n".Length;
                            if (pos == -1)
                            {
                                pos = temp.LastIndexOf("\norder by\r");
                                len = "\norder by\r".Length;
                            }
                        }
                    }
                }

                int lstI = GetLastIndex(temp, "from");
                if (pos > 0 && lstI > pos) pos = -1;
                lstI = GetLastIndex(temp, "select");
                if (pos > 0 && lstI > pos) pos = -1;
                lstI = GetLastIndex(temp, "where");
                if (pos > 0 && lstI > pos) pos = -1;
                lstI = GetLastIndex(temp, "group by");
                if (pos > 0 && lstI > pos) pos = -1;



                if (pos > -1)
                {
                    sql = SelectCommand.Substring(0, pos);
                    string orderbys = SelectCommand.Substring(pos + len);
                    AppendOrders(orderbys, Orders);
                }
                else
                    sql = SelectCommand;

                if (Orders.Count == 0) //未设置
                {
                    pos = temp.IndexOf(" from ");
                    if (pos == -1)
                    {
                        pos = temp.IndexOf("\r\nfrom ");
                        if (pos == -1) pos = temp.IndexOf("\nfrom ");
                    }
                    if (pos > 7 && temp.StartsWith("select "))
                    {
                        string flist = temp.Substring(7, pos - 7);
                        string[] arr = flist.Split(',');
                        foreach (string f in arr)
                        {
                            string vf = f;
                            if (f.IsNotWhiteSpace())
                            {
                                int bindex=f.ToLower().IndexOf(" as ");
                                if (bindex>0 &&bindex+4<f.Length)
                                {
                                    vf=f.Substring(bindex+4);
                                }
                                AppendOrders(vf, Orders);
                                break;
                            }
                        }
                    }
                }

                page.Totals = db.GetScalar(string.Format("{1}SELECT COUNT(1) FROM ({0}) t1", sql, withPerstr)).ToLong();

                //处理当前页索引
                int totalPages= ( page.Totals / page.PageSize).ToInt() +1;

                if (page.PageIndex > totalPages - 1) page.PageIndex = totalPages - 1;


                //获取合计信息
                if (page.StatInfos.IsNotNull() && page.StatInfos.Count > 0)
                {
                    page.TotalSubObj = GetFootObj(page, withPerstr, sql, db);
                }
                else
                    page.TotalSubObj = null;


                int pageIndex = page.PageIndex < 0 ? 0 : page.PageIndex;

                long bIndex = page.PageSize * pageIndex;
                long eIndex = page.PageSize * (pageIndex + 1);

                sql = string.Format(@"{4} 
select * from (
    select row_number() over({0}) as RecordNumberNo,totalAbyPage.* from ({1}) totalAbyPage
) totalBbyPage where RecordNumberNo>{2} and RecordNumberNo<={3} order by RecordNumberNo ", DBHelper.GetOrderASCInfo(Orders), sql, bIndex, eIndex, withPerstr);

                return sql;
            }
            else
                return SelectCommand;
        }


        /// <summary>
        /// 设置合计行
        /// </summary>
        internal DynamicObj GetFootObj(PageInfo page,string with,string SelectCommand,IDbDriver db)
        {
            Dictionary<string, TotalType> TotalSubs = page.StatInfos;
            List<string> fields = new List<string>();
            foreach (string field in TotalSubs.Keys)
            {
                TotalType type = TotalSubs[field];
                if (field.IsNotWhiteSpace())
                {
                    if (type != TotalType.Count)
                    {
                        fields.Add(string.Format("isnull({1}(case when isnumeric({0})=1 then {0} else 0 end),0) as {0}", field, type == TotalType.Avg ? "avg" : "sum"));
                    }
                    else
                    {
                        if (type == TotalType.Count)
                            fields.Add(string.Format("{0} as {1}",page.Totals,field));
                        else
                            fields.Add(string.Format("0 as {0}", field));
                    }
                }
                else
                {
                    continue;
                }
            }
            if (SelectCommand == "" || !fields.HasElement()) return null;
            string sql = string.Format("{0} select {1} from ({2}) as t0",with, string.Join(",", fields), SelectCommand);

            return db.GetDynamicObj(sql);            
        }

        private void AppendOrders(string orderby, List<OrderItem> sortItems)
        {
            if (orderby.IsWhiteSpace()) return;
            string[] arr = orderby.Split(',');
            foreach (string a in arr)
            {
                string ua = a.Trim();
                if (ua.Length > 5 && ua.ToLower().EndsWith(" desc"))
                {
                    ua = ua.Substring(0, ua.Length - 5);
                    if (ua.IsWhiteSpace()) continue;
                    int index = ua.IndexOf(".");
                    if (index > 0 && index < ua.Length - 2)
                        ua = ua.Substring(index + 1);

                    sortItems.Add(new OrderItem() { FieldName = ua, SortingMode = SortingMode.DESC });
                }
                else if (ua.Length > 4 && ua.ToLower().EndsWith(" asc"))
                {
                    ua = ua.Substring(0, ua.Length - 4);
                    if (ua.IsWhiteSpace()) continue;
                    int index = ua.IndexOf(".");
                    if (index > 0 && index < ua.Length - 2)
                        ua = ua.Substring(index + 1);

                    sortItems.Add(new OrderItem() { FieldName = ua, SortingMode = SortingMode.ASC });
                }
                else if (ua.IsNotWhiteSpace())
                {
                    int index = ua.IndexOf(".");
                    if (index > 0 && index < ua.Length - 2)
                        ua = ua.Substring(index + 1);
                    sortItems.Add(new OrderItem() { FieldName = ua, SortingMode = SortingMode.ASC });
                }
            }
        }

        /// <summary>
        /// 获取指定字符串中双括号内容(最外层括号）
        /// </summary>
        /// <returns></returns>
        private int GetKHEndIndex(int startIndex, string context)
        {
            int bIndex = -1, eIndex = -1;
            bIndex = context.IndexOf("(", startIndex);
            if (bIndex == -1 || bIndex == context.Length - 1) //表示没有括号，中止
            {
                return -1;
            }
            int sI = bIndex + 1;
            int cbIndex = context.IndexOf("(", sI);
            int ceIndex = context.IndexOf(")", sI);

            while (cbIndex >= 0) //有子级,则进行递归
            {
                if (cbIndex < ceIndex) //成对
                {
                    ceIndex = GetKHEndIndex(sI, context);
                    if (ceIndex >= 0 && ceIndex < context.Length - 1)
                    {
                        sI = ceIndex + 1;
                        cbIndex = context.IndexOf("(", sI);
                        ceIndex = context.IndexOf(")", sI);
                    }
                    else
                        break;
                }
                else
                {
                    break;
                }
            }
            //无子级
            eIndex = ceIndex;
            return eIndex;
        }


        #endregion
    }
}
