using RS.Data;
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

        /// <summary>
        /// 创建数据业务实体对象
        /// 本方法主要是为底层服务数据处理时提供对象创建方法
        /// </summary>
        /// <typeparam name="T">业务数据处理对象</typeparam>
        /// <param name="dbContext">数据库上下文对象/param>
        /// <returns>返回业务数据对象实例</returns>
        public static T CreateDALStore<T>(IDbContext dbContext) where T : DALStoreBase
        {
            return (T)Activator.CreateInstance(typeof(T), dbContext);
        }
    }
}
