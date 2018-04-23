using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace RS.Core.Data
{
    /// <summary>
    /// 采用分页方式获取的数据源
    /// </summary>
    public class PagedDataSource : ICollection
    {
        List<object> Datas;

        List<object> CurrentPageDatas;

        // 摘要: 
        //     初始化 System.Web.UI.WebControls.PagedDataSource 类的新实例。
        public PagedDataSource() {
            Datas = new List<object>();
            CurrentPageDatas = new List<object>();
        }

        /// <summary>
        /// 获取要从数据源使用的项数。
        /// </summary>
        public int Count => CurrentPageDatas.Count;

        private int _CurrentPageIndex = 0;
        /// <summary>
        /// 获取或设置当前页的索引
        /// </summary>
        public int CurrentPageIndex { get {
                int index = _CurrentPageIndex;
                if (PageSize >= DataSourceCount) //则只有一页
                    index = 0;
                else if (PageCount > 0 && _CurrentPageIndex > PageCount)
                    index= PageCount - 1;
                else if (_CurrentPageIndex < 0)
                    index = 0;
                return index;
            }
            set {
                int oldIndex = CurrentPageIndex; //原来的页
                int newIndex = value;//新页
                if (PageSize >= DataSourceCount) //则只有一页
                    newIndex = 0;
                else if (PageCount > 0 && newIndex > PageCount)
                    newIndex = PageCount - 1;
                else if (newIndex < 0)
                    newIndex = 0;

                _CurrentPageIndex = newIndex;

                if (oldIndex!=newIndex)//页索引有变化，重新设置当前数据索引
                {
                    ResetPage();
                }
            }
        }

        private IEnumerable _dataSource;//数据源
        /// <summary>
        /// 获取或设置数据源
        /// </summary>
        public IEnumerable DataSource
        {
            get
            {
                return _dataSource;
            }
            set
            {
                _dataSource = value;
                Datas = new List<object>();
                if (value != null)
                {
                    foreach (object o in value)
                    {
                        Datas.Add(o);
                    }
                }
                ResetPage();
            }
        }
        /// <summary>
        /// 获取数据源中的项数。
        /// </summary>
        public int DataSourceCount => Datas.Count;

        /// <summary>
        /// 获取一个值，该值指示是否同步对数据源的访问（线程安全）。
        /// </summary>
        public bool IsSynchronized { get; } = true;
        /// <summary>
        /// 获取显示数据源中的所有项所需要的总页数。
        /// </summary>
        public int PageCount
        {
            get {
                if (PageSize <= 0) //不分页
                    return DataSourceCount > 0 ? 1 : 0;

                return Math.Ceiling((decimal)DataSourceCount / (decimal)PageSize).ToInt();
            }
        }

        private int pageSize = 0;
        /// <summary>
        ///  获取或设置要在单页上显示的项数。
        /// </summary>
        public int PageSize { get {
                return pageSize;
            } set
            {
                bool ischange = pageSize != value;
                pageSize = value;
                if (ischange) ResetPage();
            } } 
        public object SyncRoot => this;


        /// <summary>
        /// 从 System.Array 中的指定索引位置开始，将数据源中的所有项复制到指定的 System.Array。
        /// </summary>
        /// <param name="array">从零开始的 System.Array，它接收来自数据源的复制项。</param>
        /// <param name="index">指定的 System.Array 中接收复制内容的第一个位置。</param>
        public void CopyTo(Array array, int index) {
            ((ICollection)Datas).CopyTo(array, index);
        }
        /// <summary>
        /// 返回一个实现了 System.Collections.IEnumerator 的对象，该对象包含数据源中的所有项。
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator() {
            
            return CurrentPageDatas.GetEnumerator();
        }

        /// <summary>
        /// 根据当前设置重新分页
        /// 1、在数据源改变时
        /// 2、在当前页索引改变时
        /// 3、在页大小改变时
        /// </summary>
        private void ResetPage()
        {
            int index = CurrentPageIndex * PageSize;
            CurrentPageDatas = Datas.GetRange(index, PageSize);
        }
    }
}
