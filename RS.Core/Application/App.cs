using System;
using System.Collections.Generic;
using System.Text;

namespace RS
{
    /// <summary>
    /// 业务应用操作处理类
    /// </summary>
    public static class App
    {
        /// <summary>
        /// 当前请求服务提供对象（用于获取会话注册服务对象实例）
        /// </summary>
        public static Func<IServiceProvider> GetServiceProvider { get; internal set; }

        /// <summary>
        /// 获取指定类型的应用服务对象实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetService<T>() where T:IAppService
        {
            IServiceProvider RequestServices=null;
            if (GetServiceProvider != null)
                RequestServices = GetServiceProvider();
            else if (Global.Services != null)
                RequestServices = Global.Services;

            if (RequestServices==null)
                throw new ArgumentNullException("未注册请求域服务提供对象");

            return (T)RequestServices.GetService(typeof(T));
        }
    }
}
