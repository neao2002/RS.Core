using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;

namespace RS.Cache
{
    /// <summary>
	/// ������
	/// </summary>
    [Serializable]
    public sealed class CacheItem<TParam,TKey,TValue>:IDisposable
	{
        
        //�����ʵ�ʴ洢���������Ϣ
        private CacheItemInfoCollection<TParam, TKey, TValue> myCacheItem;
        //������̭�㷨
		private IEliminateMethod _IEliminateMethod;
        //��󻺴������
        private int maxSize=0;

        /// <summary>
        /// ���캯����Ĭ�ϻ������������̭�㷨
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
        /// ���캯����ָ�����������㷨,���໺���ǲ������Զ�ͬ�����ܣ�Ĭ�Ͽɱ���50000����¼
        /// </summary>
        /// <param name="eliminateMethod"></param>
        internal CacheItem(IEliminateMethod eliminateMethod)
            : this()
		{
			_IEliminateMethod=eliminateMethod;
            maxSize = 50000;
		}

        /// <summary>
        /// ���캯����ָ�����������㷨,���໺���ǲ������Զ�ͬ������,��ͬʱָ��������Դ��ȡ����ֵ��ί��,Ĭ�Ͽɱ���50000����¼
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
        /// ���캯����ָ��ͬ�����ʱ�估��������ͬ����ί�У�ֻ�ʺ����ָ��½Ͽ����
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
            //�Զ�����ͬ������
            syncUtil.Start();
        }
        /// <summary>
        /// ���캯����ָ��ͬ�����ʱ�估��������ͬ����ί�У�ֻ�ʺ����ָ��½Ͽ����
        /// ͨ����ʱ���ʽ������ʱ������
        /// ��ʱ�����ø�ʽ���£�
        /// 1����ʱ������0|h|1,��ʾ��ÿСʱ����,0|m|1 ��ʾ��ÿ���Ӽ���,0|s|1 ��ʾ��ÿ���Ӽ���,0|ms|1 ��ʾ��ÿ���뼤��
        /// 2��ʱ�㴥����1|17:30;17:12;02:36
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
            //�Զ�����ͬ������
            syncUtil.Start();
        }
        /// <summary>
        /// ���캯����ָ��ͬ�����¼��ʱ�估ͬ��ʱ��ȡ����Դ�ķ���ί��(����ͬ��)
        /// </summary>
        /// <param name="GapSpan">������ͬ��ʱ��</param>
        /// <param name="method">ͬ����ȡ���ݵ�ί�У�ֻ��ȡ������ĳһ��</param>
        /// <param name="CacheDatas">��ʼҪ�����ֵ</param>
        internal CacheItem(TimeSpan GapSpan, Func<TParam,TValue> method):this()
        {
            myCacheItem.GapSpanTime = GapSpan;
            GetCacheDataMethod = method;
        }

        /// <summary>
        /// ���캯����ָ��ͬ�����¼��ʱ�估ͬ��ʱ��ȡ����Դ�ķ���ί��(����ͬ��)
        /// </summary>
        /// <param name="GapSpan">������ͬ��ʱ��</param>
        /// <param name="method">ͬ����ȡ���ݵ�ί�У�ֻ��ȡ������ĳһ��</param>
        /// <param name="CacheDatas">��ʼҪ�����ֵ</param>
        internal CacheItem(TimeSpan GapSpan, Func<TParam, TValue> method, Dictionary<TKey,TValue> CacheDatas)
        {
            myCacheItem = CacheItemInfoCollection<TParam, TKey, TValue>.CreateDatas(CacheDatas);
            myCacheItem.Owner = this;

            myCacheItem.GapSpanTime = GapSpan;
            GetCacheDataMethod = method;
        }
        /// <summary>
        /// ��������
        /// </summary>
        ~CacheItem()
        {
            Dispose();
        }
        #region ����ͬ��ִ�еķ���

        /// <summary>
        /// ͬ�������е����ݣ�ע�⣬���ﲢ����ʵ��ͬ���������Զ���ǻ�����Ĺ���ʱ��
        /// </summary>
        private void SyncData()
        {
            if (GetAllMethod != null)
            {
                Dictionary<TKey, TValue> o = GetAllMethod();
                if (o != null)
                {//δʵ�ʻ�ȡ������
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

        #region �������������

        private ThreadSvrUtil syncUtil;

        internal IEliminateMethod EliminateMethod { get { return _IEliminateMethod; } }

        /// <summary>
        /// ��󻺴������,�����Ϊ0���������ʾ������󲻻��Զ��Ƴ�
        /// </summary>
        public int MaxSize
        {
            get { return maxSize; }
            set { maxSize = value; }
        }
        /// <summary>
        /// ��ȡ���л������
        /// </summary>
        private Func<Dictionary<TKey,TValue>> GetAllMethod
        {
            get;
            set;
        }
        /// <summary>
        /// ��ȡ����л����ʵ�ʶ�������Դ�ķ�������Ҫ���ڽ��ж�ʱͬ��
        /// </summary>        
        private Func<TParam,TValue> GetCacheDataMethod
        {
            get;
            set;
        }

        /// <summary>
        /// ���������
        /// </summary>
        public string CacheName
        {
            get;
            internal set;
        }

        #endregion

        #region ������ط���
        /// <summary>
        /// ������л�����
        /// </summary>
		public void Clear()
		{
			lock(myCacheItem)
			{
				myCacheItem.Clear();
			}
		}
        /// <summary>
        /// ���õ�ǰ����
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
        /// �ⲿ�����������±������ָ����������
        /// </summary>
        /// <param name="id">������ID</param>
        /// <param name="callDataMethod">��ȡ��ֵ���ڸ��»����ί�з���</param>
        /// <param name="paraObj">ί�з����Ĳ���</param>
        public void UpdateCacheValue(TKey id,Func<TParam,TValue> callDataMethod,TParam paraObj)
		{
            myCacheItem.SetItem(id,callDataMethod,paraObj);
		}
        /// <summary>
        /// �ⲿ�����������±������ָ����������:�����������Ҫ�л�ȡ����ί��
        /// </summary>
        /// <param name="id">�����ֵ</param>
        /// <param name="paraObj">��ȡ����Դί�еĲ���</param>
        public void UpdateCacheValue(TKey id,TParam paraObj)
        {
            myCacheItem.SetItem(id, GetCacheDataMethod, paraObj);
        }
        /// <summary>
        /// �ⲿ�����������±������ָ���������ݣ������������Ҫ�л�ȡ����ί�С��������ֵ��ί�в�����һ��
        /// </summary>
        /// <param name="paraObj">����ֵ</param>
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
        /// ��ȡָ��IDֵ�Ļ���ֵ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TValue GetCacheValue(TKey id)
        {
            return myCacheItem.GetItem(id, GetCacheDataMethod, id.Evaluate<TParam>());
        }

        /// <summary>
        /// ��ȡָ��ID�Ļ���ֵ�����ֵ�����ڣ����ָ�������л�ȡ
        /// </summary>
        /// <param name="id"></param>
        /// <param name="callmethod"></param>
        /// <returns></returns>
        public TValue GetCacheValue(TKey id, Func<TParam, TValue> callmethod)
        {
            return myCacheItem.GetItem(id, callmethod, id.Evaluate<TParam>());
        }

		/// <summary>
		/// �ӻ�����ȡ��¼����������в����ڣ���ô�����ݿ�/�����ļ���ȡ
		/// </summary>
		/// <param name="id"></param>
		/// <param name="callmethod">�����ݿ�����ݵ�ί��</param>
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
        /// ������̭�㷨�Ƴ�һ���
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
        /// ��ȡ��ǰ���л�����
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
