using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Transactions;

namespace RS.Data
{
    /// <summary>
    /// 数据访问对象静态类
    /// 用于创建访问对象相关方法,采用单例模式，以便库类全局助手类调用
    /// </summary>
    public static class DBHelper
    {
        #region 项目内部静态方法
        internal static void AppentSort(List<OrderItem> sortItems,string orderby)
        {
            if (orderby.IsWhiteSpace()) return;
            string[] arr = orderby.Split(',');
            foreach (string a in arr)
            {
                string ua = a.Trim();
                if (ua.Length>5 && ua.ToLower().EndsWith(" desc"))
                {
                    ua = ua.Substring(0, ua.Length - 5);
                    if (ua.IsWhiteSpace()) continue;

                    sortItems.Insert(0, new OrderItem() { FieldName = ua, SortingMode = SortingMode.DESC });
                }
                else if (ua.Length>4 && ua.ToLower().EndsWith(" asc"))
                {
                    ua = ua.Substring(0, ua.Length - 4);
                    if (ua.IsWhiteSpace()) continue;

                    sortItems.Insert(0, new OrderItem() { FieldName = ua, SortingMode = SortingMode.ASC });
                }
                else if (ua.IsNotWhiteSpace())
                    sortItems.Insert(0, new OrderItem() { FieldName = ua, SortingMode = SortingMode.ASC });
            }
        }
        private static void RemoveSameSort(List<OrderItem> sortItems)
        {
            int i = 0, j = 0;
            while (i < sortItems.Count)
            {
                j = i + 1;
                while (j < sortItems.Count)
                {
                    OrderItem item = sortItems[j];
                    if (sortItems[i].FieldName.ToLower() == item.FieldName.ToLower())
                        sortItems.Remove(item);
                    else
                        j++;
                }
                i++;
            }
        }
        internal static string GetOrderASCInfo(List<OrderItem> sortItems)
        {
            //移除相同的排序项
            RemoveSameSort(sortItems);

            string list = string.Join(",", sortItems.ConvertAll<string>(s => string.Format("{0} {1}", s.FieldName, s.SortingMode == SortingMode.DESC ? "DESC" : "")));
            if (list.IsWhiteSpace())
                return string.Empty;
            else
                return string.Concat("order by ", list);
        }

        internal static string GetOrderDESCInfo(List<OrderItem> sortItems)
        {
            RemoveSameSort(sortItems);

            string list = string.Join(",", sortItems.ConvertAll<string>(s => string.Format("{0} {1}", s.FieldName, s.SortingMode == SortingMode.ASC ? "DESC" : "")));
            if (list.IsWhiteSpace())
                return string.Empty;
            else
                return string.Concat("order by ", list);
        
        }
        #endregion

        #region 对象外部方法
        #region 以下为分布式事务相关方法
        public static JsonReturn RunTransactionScope(Action method)
        {
            JsonReturn jr;
            //开启事务，执行操作(注意，这里一定要开启分布式事务，以便应用多业务
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    if (method != null)
                    {
                        method();
                    }
                    jr = JsonReturn.RunSuccess(true);
                }
                catch (Exception ex)
                {
                    jr = JsonReturn.RunFail(ex.Message);
                }
                if (jr.IsSuccess) scope.Complete();
            };
            return jr;
        }
        public static JsonReturn RunTransactionScope(Action method, TransactionScopeOption scopeOption)
        {
            JsonReturn jr;
            //开启事务，执行操作(注意，这里一定要开启分布式事务，以便应用多业务
            using (TransactionScope scope = new TransactionScope(scopeOption))
            {
                try
                {
                    if (method != null)
                    {
                        method();
                    }
                    jr = JsonReturn.RunSuccess(true);
                }
                catch (Exception ex)
                {
                    jr = JsonReturn.RunFail(ex.Message);
                }
                if (jr.IsSuccess) scope.Complete();
            };
            return jr;
        }
        public static JsonReturn RunTransactionScope(Action method, TransactionScopeOption scopeOption, TimeSpan scopeTimeout)
        {
            JsonReturn jr;
            //开启事务，执行操作(注意，这里一定要开启分布式事务，以便应用多业务
            using (TransactionScope scope = new TransactionScope(scopeOption, scopeTimeout))
            {
                try
                {
                    if (method != null)
                    {
                        method();
                    }
                    jr = JsonReturn.RunSuccess(true);
                }
                catch (Exception ex)
                {
                    jr = JsonReturn.RunFail(ex.Message);
                }
                if (jr.IsSuccess) scope.Complete();
            };
            return jr;
        }

        public static JsonReturn RunTransactionScope(Func<JsonReturn> method)
        {
            JsonReturn cr;
            //开启事务，执行操作(注意，这里一定要开启分布式事务，以便应用多业务
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    if (method != null)
                    {
                        cr = method();
                    }
                    else
                    {
                        cr = JsonReturn.RunFail("未执行实际数据操作方法");
                    }
                }
                catch (Exception ex)
                {
                    cr = JsonReturn.RunFail(ex.Message, ex);
                }
                if (cr.IsSuccess) scope.Complete();
            };
            return cr;
        }
        public static JsonReturn RunTransactionScope(Func<JsonReturn> method, TransactionScopeOption scopeOption)
        {
            JsonReturn cr;
            //开启事务，执行操作(注意，这里一定要开启分布式事务，以便应用多业务
            using (TransactionScope scope = new TransactionScope(scopeOption))
            {
                try
                {
                    if (method != null)
                    {
                        cr = method();
                    }
                    else
                    {
                        cr = JsonReturn.RunFail("未执行实际数据操作方法");
                    }
                }
                catch (Exception ex)
                {
                    cr = JsonReturn.RunFail(ex.Message, ex);
                }
                if (cr.IsSuccess) scope.Complete();
            };
            return cr;
        }
        public static JsonReturn RunTransactionScope(Func<JsonReturn> method, TransactionScopeOption scopeOption, TimeSpan scopeTimeout)
        {
            JsonReturn cr;
            //开启事务，执行操作(注意，这里一定要开启分布式事务，以便应用多业务
            using (TransactionScope scope = new TransactionScope(scopeOption, scopeTimeout))
            {
                try
                {
                    if (method != null)
                    {
                        cr = method();
                    }
                    else
                    {
                        cr = JsonReturn.RunFail("未执行实际数据操作方法");
                    }
                }
                catch (Exception ex)
                {
                    cr = JsonReturn.RunFail(ex.Message, ex);
                }
                if (cr.IsSuccess) scope.Complete();
            };
            return cr;
        }
        #endregion

        #endregion

        #region 记录转对象

        public static T CreateObject<T>(IDataReader dr) where T : new()
        {
            if (typeof(T) == typeof(DynamicObj) || typeof(T) == typeof(Dictionary<string, object>))
            {
                return (T)(object)DynamicObj.ToDynamic(dr);
            }


            T entity = new T();
            Type t = typeof(T);
            for (int i = 0; i < dr.FieldCount; i++)
            {
                string fn = dr.GetName(i);
                PropertyInfo p = t.GetProperty(fn,BindingFlags.Public|BindingFlags.Instance|BindingFlags.SetField);
                if (p != null && p.CanWrite)
                {
                    object v = dr.GetValue(i);
                    p.SetValue(entity, v.GetObjectValue(p.PropertyType), null);
                }
            }
            return entity;
        }

        public static T CreateObject<T>(DbDataReader dr, Dictionary<string, string> FieldMap2Property) where T : new()
        {
            T entity = new T();
            Type t = typeof(T);
            for (int i = 0; i < dr.FieldCount; i++)
            {
                string fn = dr.GetName(i);
                if (FieldMap2Property.ContainsKey(fn))
                    fn = FieldMap2Property[fn];

                PropertyInfo p = t.GetProperty(fn);
                if (p != null && p.CanWrite)
                {
                    object v = dr.GetValue(i);
                    p.SetValue(entity, v.GetObjectValue(p.PropertyType), null);
                }
            }

            return entity;
        }
        /// <summary>
        /// 根据指定对象属性集创建对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="dr">记录集</param>
        /// <param name="ps">对象属性集：以对象属性名为健值</param>
        /// <returns></returns>
        public static  T CreateObject<T>(DbDataReader dr, DynamicObj ps) where T : new()
        {
            return CreateObjectRec<T>(dr, ps, null);
        }

        internal static  T CreateObjectRec<T>(DbDataReader dr, DynamicObj ps, Action<DbDataReader> OtherSetMethod) where T : new()
        {
            T entity = new T();
            for (int i = 0; i < dr.FieldCount; i++)
            {
                string fn = dr.GetName(i);

                PropertyInfo p = ps.Get(fn) as PropertyInfo;
                if (p != null && p.CanWrite)
                {
                    object v = dr.GetValue(i);
                    p.SetValue(entity, v.GetObjectValue(p.PropertyType), null);
                }

                if (OtherSetMethod != null)
                {
                    OtherSetMethod(dr);
                }
            }
            return entity;
        }
        public static T CreateObject<T>(DbDataReader dr, DynamicObj ps, Dictionary<string, string> FieldMap2Property) where T : new()
        {
            T entity = new T();
            for (int i = 0; i < dr.FieldCount; i++)
            {
                string fn = dr.GetName(i);
                if (FieldMap2Property.ContainsKey(fn))
                    fn = FieldMap2Property[fn];

                PropertyInfo p = ps.Get(fn) as PropertyInfo;
                if (p != null && p.CanWrite)
                {
                    object v = dr.GetValue(i);
                    p.SetValue(entity, v.GetObjectValue(p.PropertyType), null);
                }
            }
            return entity;
        }
        /// <summary>
        /// 将指定对象类型转为
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DynamicObj GetObjectPropertys(Type type)
        {
            DynamicObj obj = new DynamicObj();
            PropertyInfo[] ps = type.GetProperties();
            foreach (PropertyInfo p in ps)
            {
                obj.Set(p.Name, p);
            }
            return obj;
        }
        #endregion
        
        #region 扩展方法
        /// <summary>
        /// 将值转为可保存的值，主要是将一些值视为DBNull
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static object ConvertValue(object value)
        {
            object o;
            if (value.IsNull())
                return DBNull.Value;
            else if (value is Guid && (Guid)value == Guid.Empty)
                o = DBNull.Value;
            else if (value is DateTime && ((DateTime)value == DateTime.MinValue || (DateTime)value == DateTime.MaxValue))
                o = DBNull.Value;
            else if (value is string && string.IsNullOrEmpty((string)value))
                o = DBNull.Value;
            else
                o = value;
            return o;
        }



        public static object GetObjectValue(object v, Type t)
        {
            return v.GetObjectValue(t);
        }
        #endregion
    }
}
