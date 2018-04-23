using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RS.Core;

namespace RS.Core.Cache
{
    /// <summary>
    /// 缓存实际项内容
    /// </summary>
    internal class CacheItemInfo<TParam, TKey, TValue>
    {
        private CacheItemInfoCollection<TParam, TKey, TValue> Owner;
        public CacheItemInfo(TValue value, DateTime time, CacheItemInfoCollection<TParam, TKey, TValue> _owner)
        {
            CacheItemValue = value;
            UpdateTime = time;
            Owner = _owner;
        }
        /// <summary>
        /// 缓存项值
        /// </summary>
        public TValue CacheItemValue { get; set; }
        /// <summary>
        /// 最近更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 是否已过期
        /// </summary>
        public bool IsOverdue 
        {
            get 
            {
                if (Owner.GapSpanTime == null || Owner.GapSpanTime.Equals(TimeSpan.Zero)) return false; //如没有过期间隔时间，则表示不用进行同步

                DateTime time = DateTime.Now;//获取当前时间
                TimeSpan ts = time - UpdateTime;//获取当前时间与该项最近更新时间间隔
                return ts.TotalMilliseconds > Owner.GapSpanTime.TotalMilliseconds;
            }
        }
    }
    /// <summary>
    /// 缓存项集合，存储实际缓存值
    /// </summary>
    internal class CacheItemInfoCollection<TParam, TKey, TValue> : Dictionary<TKey, CacheItemInfo<TParam, TKey, TValue>>
    {
        /// <summary>
        /// 缓存淘汰算法
        /// </summary>
        public CacheItem<TParam, TKey, TValue> Owner { get; set; }
        /// <summary>
        /// 最近同步时间，通过该时间，各缓存项便可知是否已过期
        /// </summary>
        public TimeSpan GapSpanTime { get; set; }
        /// <summary>
        /// 根据指定初始需缓存的数据集合创建缓存值集
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        public static CacheItemInfoCollection<TParam, TKey, TValue> CreateDatas(Dictionary<TKey, TValue> datas)
        {
            CacheItemInfoCollection<TParam, TKey, TValue> items = new CacheItemInfoCollection<TParam, TKey, TValue>();
            if (datas.IsNotNull())
            {
                foreach (TKey key in datas.Keys)
                    items[key]= new CacheItemInfo<TParam, TKey, TValue>(datas[key], DateTime.Now, items);
            }            
            return items;
        }

        public List<TValue> ToList()
        {
            return Values.ToList().ConvertAll<TValue>(s => s.CacheItemValue);
        }


        public void InsertItem(TKey key,TValue value)
        {
            if (ContainsKey(key))
            {
                CacheItemInfo<TParam,TKey,TValue> item=this[key];
                this[key].CacheItemValue = value;
                item.UpdateTime = DateTime.Now;

                if (Owner.EliminateMethod != null) //本项缓存有淘汰算法
                    Owner.EliminateMethod.UpdateWeight(key);
            }
            else
                this[key]= new CacheItemInfo<TParam, TKey, TValue>(value, DateTime.Now, this);
        }
        /// <summary>
        /// 获取指定键值的缓存对象，如没有，则从指定委托及参数中获取
        /// </summary>
        /// <param name="key"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public TValue GetItem(TKey key,Func<TParam, TValue> method, TParam obj)
        {            
            CacheItemInfo<TParam, TKey, TValue> item = null;
            if (this.ContainsKey(key)) //有该值
            {
                //为了线程安全，在取值时，增加防异常处理
                try {                    
                    item = this[key];
                }
                catch { 
                    //有异常，表示该项此时已被其它用户移除了集合                    
                }
            }
            if (item != null) //该项在缓存中
            {
                if (item.IsOverdue) //该缓存值已过期
                {
                    item.CacheItemValue = method(obj);//则直接从数据源中更新该缓存值
                    item.UpdateTime = DateTime.Now;

                    if (Owner.EliminateMethod != null) //本项缓存有淘汰算法
                        Owner.EliminateMethod.UpdateWeight(key);
                }
                return item.CacheItemValue;
            }
            else
            {
                if (method == null) return default(TValue);
                TValue o = method(obj);
                if (o == null) return o;
                lock (this)
                {   
                    if (Owner.MaxSize> 0)
                    {
                        if (Owner.EliminateMethod != null)
                        {
                            int i = this.Count;

                            if (i >= Owner.MaxSize)
                            {
                                Owner.DelCacheValue();
                                Owner.EliminateMethod.DelPoorWeight();
                            }
                        }
                        this[key] = new CacheItemInfo<TParam, TKey, TValue>(o, DateTime.Now, this);

                        if (Owner.EliminateMethod != null) Owner.EliminateMethod.InsertWeight(key);
                    }
                }
                return o;
            }            
        }

        public void SetItem(TKey key, TValue newValue)
        {
            SetItem(key, newValue, null, key.Evaluate<TParam>());
        }
        public void SetItem(TKey key, Func<TParam, TValue> method)
        {
            SetItem(key,default(TValue),method, key.Evaluate<TParam>());
        }
        public void SetItem(TKey key, Func<TParam, TValue> method,TParam obj)
        {
            SetItem(key, default(TValue), method, obj);
        }
        /// <summary>
        /// 更新指定键的缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public void SetItem(TKey key,TValue newValue, Func<TParam, TValue> method, TParam obj)
        {
            TValue v = method.IsNotNull() ? method(obj) : newValue;
            if (v == null || v.Equals(default(TValue))) //表示移除
            {
                lock (this)
                {
                    Remove(key);
                    if (Owner.EliminateMethod != null) //本项缓存有淘汰算法
                        Owner.EliminateMethod.DelWeight(key);
                }
            }
            else
            {
                lock (this)
                {
                    this[key] = new CacheItemInfo<TParam, TKey, TValue>(v, DateTime.Now, this);
                }
            }
        }
    }
}
