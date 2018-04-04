using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;

namespace RS.Core
{
    /// <summary>
    /// 集合相关扩展方法
    /// </summary>
    public static class ListUtils
    {
        public static object GetValue(this IDictionary lists, object Key)
        {
            if (lists.Contains(Key))
                return lists[Key];
            else
                return null;
        }

        /// <summary>
        /// 获取指定集合中指定公共属性值集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <param name="Senders"></param>
        /// <param name="ProperName"></param>
        /// <returns></returns>
        public static List<T> ToProperValues<T, B>(this List<B> Senders, string ProperName)
        {
            List<T> lists = new List<T>();
            foreach (B o in Senders)
            {
                Type typ = o.GetType();
                PropertyInfo objProperty = typ.GetProperty(ProperName);
                lists.Add((T)objProperty.GetValue(o, null));
            }
            return lists;
        }
        /// <summary>
        /// 将数组转为List集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this T[] arr)
        {
            if (arr == null) return null;
            List<T> lists = new List<T>();
            lists.AddRange(arr);
            return lists;
        }

        /// <summary>
        /// 指定数组是否有元素
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static bool HasElement(this Array arr)
        {
            return (arr != null && arr.Length > 0);
        }
        /// <summary>
        /// 指定集合是否有元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool HasElement<T>(this List<T> list)
        {
            return (list != null && list.Count > 0);
        }


        public static void ForEach<T>(this IEnumerable items,Action<T> method)
        {
            foreach (T item in items)
            {
                method(item);
            }
        }
    }
}
