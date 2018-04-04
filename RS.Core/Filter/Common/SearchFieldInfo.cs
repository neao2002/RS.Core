using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RS.Core.Filter
{
    /// <summary>
    /// 检索字段信息类
    /// </summary>
    public class SearchFieldInfo
    {
        /// <summary>
        /// 检索的字段名
        /// </summary>
        public string FieldName { get; set; }
        /// <summary>
        /// 该项字段的标题说明
        /// </summary>
        public string Caption { get; set; }
    }

    /// <summary>
    /// 检索字段集合
    /// </summary>
    public class SearchFieldCollection:Dictionary<string,SearchFieldInfo>
    {
        private SearchFieldInfo activeField;
        /// <summary>
        /// 当前激活的检索字段,默认为第一个字段
        /// </summary>
        public SearchFieldInfo ActiveField
        {
            get {
                if (activeField == null)
                {
                    if (Count > 0)
                        return this.Values.ElementAt<SearchFieldInfo>(0);
                    else //为了防止出错，这里返回一个空字段实例
                        return new SearchFieldInfo { FieldName = "", Caption = ""};
                }
                else
                    return activeField;
            }
            set
            {
                if (value==null) return;
                activeField = value;
                this[activeField.FieldName]=value;
            }
        }

        public void Add(SearchFieldInfo field)
        {
            if (field.IsNotNull()) this[field.FieldName] = field;
        }
        /// <summary>
        /// 批量增加检索字段，这里会自动过滤字段名相同的字段
        /// </summary>
        /// <param name="Fields"></param>
        public void AddRange(IEnumerable<SearchFieldInfo> Fields)
        {
            foreach (SearchFieldInfo sf in Fields)
            {
                if (sf.IsNotNull()) this[sf.FieldName] = sf;
            }
        }
    }
}
