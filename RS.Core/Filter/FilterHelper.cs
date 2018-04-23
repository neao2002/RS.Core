using RS.Core.Filter;
using System;
using System.Collections.Generic;
using System.Text;

namespace RS.Core
{
    public static class FilterHelper
    {
        public static IFilter CreateIFilter(IDbDriver db = null)
        {
            IFilter filter= new EmptySqlFilter(db);           
            return filter;
        }


        /// <summary>
        /// 获取与指定SQL语句链接的字符
        /// </summary>
        /// <param name="Filter"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static string GetFilter(this IFilter Filter, string prefix)
        {
            if (Filter == null) return string.Empty;
            string filter = Filter.ToSqlFilter();
            if (filter.IsNotWhiteSpace())
                return string.Format("{0} {1}", prefix, filter);
            else
                return string.Empty;
        }
        /// <summary>
        /// 获取与指定SQL语句链接的字符
        /// </summary>
        /// <param name="Filter"></param>
        /// <param name="Db"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static string GetFilter(this IFilter Filter, IDbDriver Db, string prefix)
        {
            if (Filter == null) return string.Empty;
            string filter = Filter.ToSqlFilter(Db);
            if (filter.IsNotWhiteSpace())
                return string.Format("{0} {1}", prefix, filter);
            else
                return string.Empty;
        }


        /// <summary>
        /// 将指定数据类型转为查询数据类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static FilterDataType ToFilterDataType(this Type type)
        {
            if (type == typeof(string))
            {
                return FilterDataType.String;
            }
            else if (type == typeof(Guid))
            {
                return FilterDataType.Guid;
            }
            else if (type == typeof(DateTime))
            {
                return FilterDataType.DateTime;
            }
            else if (type == typeof(bool))
            {
                return FilterDataType.Bool;
            }
            else
                return FilterDataType.Numeric;
        }
    }
}
