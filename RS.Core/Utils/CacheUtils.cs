using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RS.Lib.Cache
{
    /// <summary>
    /// Cache总线管理器,负责管理应用系统中所有CacheItem
    /// </summary>
    public sealed class CacheUtils
    {
        #region CacheUtils 本身对象属性、方法定义，不能外公开
        private Dictionary<string, object> CacheTable;
        private CacheUtils()
        {
            CacheTable = new Dictionary<string, object>();
        }
        /// <summary>
        /// 获取指定键值缓存
        /// </summary>
        /// <param name="CacheName"></param>
        /// <returns></returns>
        private CacheItem<TParam, TKey, TValue> GetCacheItem<TParam, TKey, TValue>(string CacheName)
        {
            CacheItem<TParam, TKey, TValue> item = null;
            if (CacheTable.ContainsKey(CacheName))
            {
                try {
                    item = CacheTable[CacheName] as CacheItem<TParam,TKey,TValue>;
                }
                catch { }
            }
            return item;
        }

        private void SetCacheItem<TParam, TKey, TValue>(string CacheName, CacheItem<TParam, TKey, TValue> item)
        {
            lock (CacheTable)
            {
                CacheTable[CacheName] = item;
            }
        }
        #endregion
        #region 静态方法:缓存获取相关方法
        private static CacheUtils instance;
        private static object tmp = new object();
        private static CacheUtils Instance
        {
            get 
            {
                if (instance == null)
                {
                    lock (tmp)
                    {
                        if (instance==null) instance = new CacheUtils();
                    }
                }
                return instance;
            }
        }
        /// <summary>
        /// 打开指定缓存,如缓存项不存在，则自动创建该项缓存
        /// </summary>
        /// <param name="CacheName">缓存项键值</param>
        /// <returns>该项缓存</returns>
        public static CacheItem<TParam, TKey, TValue> OpenCache<TParam, TKey, TValue>(string CacheName)
        {
            CacheItem<TParam, TKey, TValue> item = Instance.GetCacheItem<TParam, TKey, TValue>(CacheName);
            if (item == null)
            {
                item = new CacheItem<TParam, TKey, TValue>();
                Instance.SetCacheItem(CacheName, item);
            }
            return item;
        }
        public static CacheItem<TParam, TKey, TValue> OpenCache<TParam, TKey, TValue>(string CacheName, DelegateOfSynchronous<TParam, TValue> method)
        {
            CacheItem<TParam, TKey, TValue> item = Instance.GetCacheItem<TParam, TKey, TValue>(CacheName);
            if (item == null)
            {
                item = new CacheItem<TParam, TKey, TValue>(method);
                Instance.SetCacheItem(CacheName, item);
            }
            return item;
        }

        /// <summary>
        /// 打开指定缓存,如缓存项不存在，则自动创建该项缓存
        /// </summary>
        /// <param name="CacheName">缓存项键值</param>
        /// <param name="eliminateMethod">缓存淘汰算法</param>
        /// <returns>该项缓存</returns>
        public static CacheItem<TParam, TKey, TValue> OpenCache<TParam, TKey, TValue>(string CacheName, IEliminateMethod eliminateMethod)
        {
            CacheItem<TParam, TKey, TValue> item = Instance.GetCacheItem<TParam, TKey, TValue>(CacheName);
            if (item == null)
            {
                item = new CacheItem<TParam, TKey, TValue>(eliminateMethod);
                Instance.SetCacheItem(CacheName, item);
            }
            return item;
        }
        /// <summary>
        /// 打开指定缓存,如缓存项不存在，则自动创建该项缓存
        /// </summary>
        /// <param name="CacheName">缓存项键值</param>
        /// <param name="eliminateMethod">缓存淘汰算法</param>
        /// <param name="method">获取单项值的委托</param>
        /// <returns>该项缓存</returns>
        public static CacheItem<TParam, TKey, TValue> OpenCache<TParam, TKey, TValue>(string CacheName, IEliminateMethod eliminateMethod, DelegateOfSynchronous<TParam, TValue> method)
        {
            CacheItem<TParam, TKey, TValue> item = Instance.GetCacheItem<TParam, TKey, TValue>(CacheName);
            if (item == null)
            {
                item = new CacheItem<TParam, TKey, TValue>(eliminateMethod, method);
                Instance.SetCacheItem(CacheName, item);
            }
            return item;
        }
        /// <summary>
        /// 打开指定缓存,如缓存项不存在，则自动创建该项缓存
        /// </summary>
        /// <param name="CacheName">缓存项键值</param>
        /// <param name="GapSpan">缓存同步时间间隔</param>
        /// <param name="method">同步获取所有数据源的委托</param>
        /// <returns>该项缓存</returns>
        public static CacheItem<TParam, TKey, TValue> OpenCache<TParam, TKey, TValue>(string CacheName, TimeSpan GapSpan, DelegateOfSynchronousAll<TKey,TValue> method)
        {
            CacheItem<TParam, TKey, TValue> item = Instance.GetCacheItem<TParam, TKey, TValue>(CacheName);
            if (item == null)
            {
                item = new CacheItem<TParam, TKey, TValue>(GapSpan, method);
                Instance.SetCacheItem(CacheName, item);
            }
            return item;
        }
        /// <summary>
        /// 打开指定缓存,如缓存项不存在，则自动创建该项缓存
        /// </summary>
        /// <typeparam name="TParam"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="CacheName">缓存项键值</param>
        /// <param name="GapSpanExp">缓存同步时间间隔，定时器设置格式如下：
        /// 1、定时触发：0|h|1,表示按每小时激活,0|m|1 表示按每分钟激活,0|s|1 表示按每秒钟激活,0|ms|1 表示按每毫秒激活
        /// 2、时点触发：1|17:30;17:12;02:36</param>
        /// <param name="method">同步获取所有数据源的委托</param>
        /// <param name="datas">初始值</param>
        /// <returns></returns>
        public static CacheItem<TParam, TKey, TValue> OpenCache<TParam, TKey, TValue>(string CacheName, string GapSpanExp, DelegateOfSynchronousAll<TKey, TValue> method, Dictionary<TKey, TValue> datas)
        {
            CacheItem<TParam, TKey, TValue> item = Instance.GetCacheItem<TParam, TKey, TValue>(CacheName);
            if (item == null)
            {
                item = new CacheItem<TParam, TKey, TValue>(GapSpanExp, method, datas);
                Instance.SetCacheItem(CacheName, item);
            }
            return item;
        }
        /// <summary>
        /// 打开指定缓存,如缓存项不存在，则自动创建该项缓存
        /// </summary>
        /// <param name="CacheName">缓存项键值</param>
        /// <param name="GapSpan">单项缓存过期时间间隔</param>
        /// <param name="method">过期时获取单项数据的委托</param>
        /// <returns>该项缓存</returns>
        public static CacheItem<TParam, TKey, TValue> OpenCache<TParam, TKey, TValue>(string CacheName, TimeSpan GapSpan, DelegateOfSynchronous<TParam, TValue> method)
        {
            CacheItem<TParam, TKey, TValue> item = Instance.GetCacheItem<TParam, TKey, TValue>(CacheName);
            if (item == null)
            {
                item = new CacheItem<TParam, TKey, TValue>(GapSpan, method);
                Instance.SetCacheItem(CacheName, item);
            }
            return item;
        }
        /// <summary>
        /// 打开指定缓存,如缓存项不存在，则自动创建该项缓存
        /// </summary>
        /// <param name="CacheName">缓存项键值</param>
        /// <param name="GapSpan">同步时间间隔</param>
        /// <param name="method">获取单项数据源的委托</param>
        /// <param name="CacheDatas">初始缓存数据</param>
        /// <returns>该项缓存</returns>
        public static CacheItem<TParam, TKey, TValue> OpenCache<TParam, TKey, TValue>(string CacheName, TimeSpan GapSpan, DelegateOfSynchronous<TParam, TValue> method, Dictionary<TKey,TValue> CacheDatas)
        {
            CacheItem<TParam, TKey, TValue> item = Instance.GetCacheItem<TParam, TKey, TValue>(CacheName);
            if (item == null)
            {
                item = new CacheItem<TParam, TKey, TValue>(GapSpan, method, CacheDatas);
                Instance.SetCacheItem(CacheName, item);
            }
            return item;
        }
        /// <summary>
        /// 打开指定缓存,如缓存项不存在，则自动创建该项缓存
        /// </summary>
        /// <param name="CacheName">缓存项键值</param>
        /// <param name="method">获取单项数据源的委托</param>
        /// <param name="CacheDatas">初始缓存数据</param>
        /// <returns>该项缓存</returns>
        public static CacheItem<TParam, TKey, TValue> OpenCache<TParam, TKey, TValue>(string CacheName, DelegateOfSynchronous<TParam, TValue> method, Dictionary<TKey, TValue> CacheDatas)
        {
            CacheItem<TParam, TKey, TValue> item = Instance.GetCacheItem<TParam, TKey, TValue>(CacheName);
            if (item == null)
            {
                item = new CacheItem<TParam, TKey, TValue>(method, CacheDatas);
                Instance.SetCacheItem(CacheName, item);
            }
            return item;
        }
        #endregion
    }
}
