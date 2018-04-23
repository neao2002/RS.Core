using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace RS.Core
{
    public class TreeEntityList:List<TreeContainer>
    {
        private TreeContainer Owner=null;
        public TreeEntityList()
        { }
        public TreeEntityList(TreeContainer owner)
        {
            Owner = owner;
        }
        /// <summary>
        /// 将对象添加到 System.Collections.Generic.List<T> 的结尾处。
        /// </summary>
        /// <param name="item"></param>
        public new void Add(TreeContainer item)
        {
            item.Parent = Owner;
            base.Add(item);
        }
        /// <summary>
        /// 将指定集合的元素添加到 System.Collections.Generic.List<T> 的末尾。
        /// </summary>
        /// <param name="collection"></param>
        public new void AddRange(IEnumerable<TreeContainer> collection)
        {
            foreach (TreeContainer item in collection)
            {
                item.Parent = Owner;
                base.Add(item);
            }
        }
        /// <summary>
        /// 将元素插入 System.Collections.Generic.List<T> 的指定索引处。
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public new void Insert(int index, TreeContainer item)
        {
            item.Parent = Owner;
            base.Insert(index, item);
        }
        /// <summary>
        /// 将集合中的某个元素插入 System.Collections.Generic.List<T> 的指定索引处。
        /// </summary>
        /// <param name="index"></param>
        /// <param name="collection"></param>
        public new void InsertRange(int index, IEnumerable<TreeContainer> collection)
        {
            foreach (TreeContainer item in collection)
            {
                item.Parent = Owner;                
            }
            base.InsertRange(index, collection);
        }

        public List<TreeContainer> ToList()
        {
            List<TreeContainer> items = new List<TreeContainer>();
            foreach(TreeContainer tc in this){
                items.Add(tc);
                items.AddRange(tc.Childs.ToList());
            }
            return items;
        }
    }
}
