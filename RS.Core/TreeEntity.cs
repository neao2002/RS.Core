using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Reflection;

namespace RS
{
    /// <summary>
    /// 标准树型实体
    /// </summary>
    public class TreeContainer
    {
        public TreeContainer(object o)
        {
            obj = o;
            Childs = new TreeEntityList(this);
        }
        /// <summary>
        /// 该节点上级所属节点
        /// </summary>
        public TreeContainer Parent { get; set; }
        /// <summary>
        /// 该节点对象
        /// </summary>
        public TreeEntityList Childs { get; set; }
        private object obj;
        /// <summary>
        /// 当前对象实体
        /// </summary>
        public object Current { get { return obj; } }

        /// <summary>
        /// 获取当前所外层次,顶层为0
        /// </summary>
        public int Depth
        {
            get {
                if (Parent != null)
                    return Parent.Depth + 1;
                else
                    return 0;
            }
        }
        /// <summary>
        /// 是否为最后一节点
        /// </summary>
        public bool IsLastNode
        {
            get 
            {
                if (Parent != null)
                {
                    return Parent.Childs.IndexOf(this) == Parent.Childs.Count - 1;
                }
                else
                    return false;
            }
        }

        public List<TreeContainer> ToList()
        {
            List<TreeContainer> lists = new List<TreeContainer>();
            lists.Add(this);
            lists.AddRange(this.Childs);
            return lists;
        }
    }

}
