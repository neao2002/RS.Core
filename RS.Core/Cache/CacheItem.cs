using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;

namespace RS.Cache
{
    /// <summary>
	/// 缓存项
	/// </summary>
    [Serializable]
    public sealed class CacheItem<TParam,TKey,TValue>:IDisposable
	{
        
        //缓存项，实际存储缓存对像信息
        private CacheItemInfoCollection<TParam, TKey, TValue> myCacheItem;
        //缓存淘汰算法
		private IEliminateMethod _IEliminateMethod;
        //最大缓存对象数
        private int maxSize=0;

        /// <summary>
        /// 构造函数：默认缓存项，不设置淘汰算法
        /// </summary>
        internal CacheItem()
        {
            myCacheItem = new CacheItemInfoCollection<TParam, TKey, TValue>
            {
                Owner = this
            };
        }
        internal CacheItem(Func<TParam,TValue> method):this()
        {
            GetCacheDataMethod = method;
        }
        /// <summary>
        /// 构造函数：指定缓存对象的算法,本类缓存是不进行自动同步功能，默认可保存50000条记录
        /// </summary>
        /// <param name="eliminateMethod"></param>
        internal CacheItem(IEliminateMethod eliminateMethod)
            : this()
		{
			_IEliminateMethod=eliminateMethod;
            maxSize = 50000;
		}

        /// <summary>
        /// 构造函数：指定缓存对象的算法,本类缓存是不进行自动同步功能,并同时指定从数据源获取单项值的委托,默认可保存50000条记录
        /// </summary>
        /// <param name="eliminateMethod"></param>
        /// <param name="method"></param>
        internal CacheItem(IEliminateMethod eliminateMethod, Func<TParam,TValue> method):this()
        {
            _IEliminateMethod = eliminateMethod;
            GetCacheDataMethod = method;
            maxSize = 50000;
        }

        internal CacheItem(Func<TParam, TValue> method, Dictionary<TKey, TValue> CacheDatas)
        {
            GetCacheDataMethod = method;
            myCacheItem = CacheItemInfoCollection<TParam, TKey, TValue>.CreateDatas(CacheDatas);
            myCacheItem.Owner = this;
        }

        /// <summary>
        /// 构造函数：指定同步间隔时间及所有数据同步的委托，只适合那种更新较快的项
        /// </summary>
        /// <param name="GapSpan"></param>
        /// <param name="method"></param>
        internal CacheItem(TimeSpan GapSpan, Func<Dictionary<TKey, TValue>> method)
        {
            GetAllMethod = method;
            if (method != null)
                myCacheItem = CacheItemInfoCollection<TParam, TKey, TValue>.CreateDatas(method());
            else
                myCacheItem = new CacheItemInfoCollection<TParam, TKey, TValue>();

            myCacheItem.Owner = this;

            syncUtil = ThreadSvrUtil.CreateSvrUtil(GapSpan, SyncData);
            //自动启动同步服务
            syncUtil.Start();
        }
        /// <summary>
        /// 构造函数：指定同步间隔时间及所有数据同步的委托，只适合那种更新较快的项
        /// 通过定时表达式创建定时激活器
        /// 定时器设置格式如下：
        /// 1、定时触发：0|h|1,表示按每小时激活,0|m|1 表示按每分钟激活,0|s|1 表示按每秒钟激活,0|ms|1 表示按每毫秒激活
        /// 2、时点触发：1|17:30;17:12;02:36
        /// </summary>
        /// <param name="GapSpanExp"></param>
        /// <returns></returns>
        internal CacheItem(string GapSpanExp, Func<Dictionary<TKey, TValue>> method, Dictionary<TKey, TValue> items)
        {
            GetAllMethod = method;
            if (items != null)
                myCacheItem = CacheItemInfoCollection<TParam, TKey, TValue>.CreateDatas(items);
            else
                myCacheItem = new CacheItemInfoCollection<TParam, TKey, TValue>();
            myCacheItem.Owner = this;

            ThreadTimer timer = ThreadTimer.CreateThreadTimerByExp(GapSpanExp);
            timer.Method = SyncData;
            syncUtil = new ThreadSvrUtil(timer);
            //自动启动同步服务
            syncUtil.Start();
        }
        /// <summary>
        /// 构造函数：指定同步更新间隔时间及同步时获取数据源的方法委托(单项同步)
        /// </summary>
        /// <param name="GapSpan">缓存间隔同步时间</param>
        /// <param name="method">同步获取数据的委托，只获取缓存中某一项</param>
        /// <param name="CacheDatas">初始要缓存的值</param>
        internal CacheItem(TimeSpan GapSpan, Func<TParam,TValue> method):this()
        {
            myCacheItem.GapSpanTime = GapSpan;
            GetCacheDataMethod = method;
        }

        /// <summary>
        /// 构造函数：指定同步更新间隔时间及同步时获取数据源的方法委托(单项同步)
        /// </summary>
        /// <param name="GapSpan">缓存间隔同步时间</param>
        /// <param name="method">同步获取数据的委托，只获取缓存中某一项</param>
        /// <param name="CacheDatas">初始要缓存的值</param>
        internal CacheItem(TimeSpan GapSpan, Func<TParam, TValue> method, Dictionary<TKey,TValue> CacheDatas)
        {
            myCacheItem = CacheItemInfoCollection<TParam, TKey, TValue>.CreateDatas(CacheDatas);
            myCacheItem.Owner = this;

            myCacheItem.GapSpanTime = GapSpan;
            GetCacheDataMethod = method;
        }
        /// <summary>
        /// 析构函数
        /// </summary>
        ~CacheItem()
        {
            Dispose();
        }
        #region 缓存同步执行的方法

        /// <summary>
        /// 同步缓存中的数据，注意，这里并不是实际同步，而是自动标记缓存项的过期时间
        /// </summary>
        private void SyncData()
        {
            if (GetAllMethod != null)
            {
                Dictionary<TKey, TValue> o = GetAllMethod();
                if (o != null)
                {//未实际获取到数据
                    CacheItemInfoCollection<TParam, TKey, TValue> datas = CacheItemInfoCollection<TParam, TKey, TValue>.CreateDatas(o);
                    datas.Owner = this;
                    lock (myCacheItem)
                    {
                        myCacheItem = datas;
                    }
                }
            }
        }
        #endregion

        #region 缓存项相关属性

        private ThreadSvrUtil syncUtil;

        internal IEliminateMethod EliminateMethod { get { return _IEliminateMethod; } }

        /// <summary>
        /// 最大缓存对象数,如果设为0或负数，则表示缓存对象不会自动移除
        /// </summary>
        public int MaxSize
        {
            get { return maxSize; }
            set { maxSize = value; }
        }
        /// <summary>
        /// 获取所有缓存项方法
        /// </summary>
        private Func<Dictionary<TKey,TValue>> GetAllMethod
        {
            get;
            set;
        }
        /// <summary>
        /// 获取需进行缓存的实际对象数据源的方法，主要用于进行定时同步
        /// </summary>        
        private Func<TParam,TValue> GetCacheDataMethod
        {
            get;
            set;
        }

        /// <summary>
        /// 本项缓存名称
        /// </summary>
        public string CacheName
        {
            get;
            internal set;
        }

        #endregion

        #region 缓存相关方法
        /// <summary>
        /// 清除所有缓存项
        /// </summary>
		public void Clear()
		{
			lock(myCacheItem)
			{
				myCacheItem.Clear();
			}
		}
        /// <summary>
        /// 重置当前缓存
        /// </summary>
        public void Reload()
        {
            SyncData();
        }
        public void Reload(Dictionary<TKey, TValue> datas)
        {
            CacheItemInfoCollection<TParam, TKey, TValue> tmp = CacheItemInfoCollection<TParam, TKey, TValue>.CreateDatas(datas);
            tmp.Owner=this;
            lock (myCacheItem)
            {
                myCacheItem.Clear();
                myCacheItem = tmp;
            }
        }

        /// <summary>
        /// 外部程序主动更新本项缓存中指定缓存内容
        /// </summary>
        /// <param name="id">缓存项ID</param>
        /// <param name="callDataMethod">获取新值用于更新缓存的委托方法</param>
        /// <param name="paraObj">委托方法的参数</param>
        public void UpdateCacheValue(TKey id,Func<TParam,TValue> callDataMethod,TParam paraObj)
		{
            myCacheItem.SetItem(id,callDataMethod,paraObj);
		}
        /// <summary>
        /// 外部程序主动更新本项缓存中指定缓存内容:这种情况必须要有获取方法委托
        /// </summary>
        /// <param name="id">缓存键值</param>
        /// <param name="paraObj">获取数据源委托的参数</param>
        public void UpdateCacheValue(TKey id,TParam paraObj)
        {
            myCacheItem.SetItem(id, GetCacheDataMethod, paraObj);
        }
        /// <summary>
        /// 外部程序主动更新本项缓存中指定缓存内容：这种情况必须要有获取方法委托、缓存项键值和委托参数是一致
        /// </summary>
        /// <param name="paraObj">参数值</param>
        public void UpdateCacheValue(TParam paraObj)
        {
            myCacheItem.SetItem(paraObj.Evaluate<TKey>(), GetCacheDataMethod, paraObj);
        }

        public void UpdateCacheValue(TKey key,TValue newValue)
        {
            myCacheItem.SetItem(key, newValue);
        }

        public void DeleteCacheValue(TKey key)
        {
            myCacheItem.SetItem(key, default(TValue), null, default(TParam));
        }

        
        /// <summary>
        /// 获取指定ID值的缓存值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TValue GetCacheValue(TKey id)
        {
            return myCacheItem.GetItem(id, GetCacheDataMethod, id.Evaluate<TParam>());
        }

        /// <summary>
        /// 获取指定ID的缓存值，如果值不存在，则从指定方法中获取
        /// </summary>
        /// <param name="id"></param>
        /// <param name="callmethod"></param>
        /// <returns></returns>
        public TValue GetCacheValue(TKey id, Func<TParam, TValue> callmethod)
        {
            return myCacheItem.GetItem(id, callmethod, id.Evaluate<TParam>());
        }

		/// <summary>
		/// 从缓存中取记录，如果缓存中不存在，那么在数据库/数据文件中取
		/// </summary>
		/// <param name="id"></param>
		/// <param name="callmethod">从数据库读数据的委托</param>
		/// <returns></returns>
        public TValue GetCacheValue(TKey id, Func<TParam, TValue> callmethod, TParam objs)
		{
            return myCacheItem.GetItem(id, callmethod, objs);
		}

        public void InsertCacheValue(TKey id, TValue ItemValue)
        {
            myCacheItem.InsertItem(id, ItemValue);
        }

        /// <summary>
        /// 按照淘汰算法移除一项缓存
        /// </summary>
        internal void DelCacheValue()
		{
			TKey ID=(TKey)_IEliminateMethod.GetPoorWeightID();
			lock(myCacheItem)
			{
				myCacheItem.Remove(ID);
			}
		}
        /// <summary>
        /// 获取当前所有缓存项
        /// </summary>
        /// <returns></returns>
        public List<TValue> ToList()
        {
            return myCacheItem.ToList();
        }
        #endregion

        public void Dispose()
        {
            if (syncUtil!=null) syncUtil.Stop();
            if (myCacheItem!=null) Clear();
            myCacheItem = null;
        }
    }
}
