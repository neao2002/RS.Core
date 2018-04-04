using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace RS.Core.Data
{
    public class DbHelper
    {
        public static void AppentSort(List<OrderItem> sortItems,string orderby)
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
        public static string GetOrderASCInfo(List<OrderItem> sortItems)
        {
            //移除相同的排序项
            RemoveSameSort(sortItems);

            string list = string.Join(",", sortItems.ConvertAll<string>(s => string.Format("{0} {1}", s.FieldName, s.SortingMode == SortingMode.DESC ? "DESC" : "")));
            if (list.IsWhiteSpace())
                return string.Empty;
            else
                return string.Concat("order by ", list);
        }

        public static string GetOrderDESCInfo(List<OrderItem> sortItems)
        {
            RemoveSameSort(sortItems);

            string list = string.Join(",", sortItems.ConvertAll<string>(s => string.Format("{0} {1}", s.FieldName, s.SortingMode == SortingMode.ASC ? "DESC" : "")));
            if (list.IsWhiteSpace())
                return string.Empty;
            else
                return string.Concat("order by ", list);
        
        }
    }
}
