using System;
using System.Collections.Generic;
using System.Text;

namespace RS.Core.Filter
{
    /// <summary>
    /// 用于设置自定义弹出窗口查询的接口
    /// </summary>
    /// <returns></returns>
    public delegate void DelegateOfSetControl(object sender);
    public delegate void DelegateOfSetControlForUrl(object sender,string sqlurl);
    public delegate void DelegateOfSetControlForForm(object sender,Type FrmType);
    public delegate void DelegateOfSetControlForQuery(object sender,Guid QueryID);
}
