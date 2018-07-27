using System;
using System.Collections.Generic;
using System.Text;

namespace RS
{
    public interface IContextAccessor
    {
        /// <summary>
        /// 获取当前会话供应者
        /// </summary>
        /// <returns></returns>
        IServiceProvider GetProvider();
    }
}
