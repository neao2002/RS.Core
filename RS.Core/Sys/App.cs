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
        #region 静态方法，用于基于本组件框架标识当前应用系统唯一标识ID
        /// <summary>
        /// 检测输入的ID是否合法
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static bool IsValid(string id)
        {
            //^[0-9a-zA-Z|_|-]{1,}$  数字+字母
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"^[0-9a-zA-Z|_|-]{1,}$");
            if (reg.IsMatch(id))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        internal static void InitAppID(string id)
        {
            if (IsValid(id))
                AppSystemID = id;
            else
                throw new Exception("No Valid AppID");
        }
        /// <summary>
        /// 系统标识ID，用于标明当前系统与其它系统的区别，ID号必须为字母、数字、或下划线或横线
        /// </summary>
        public static string AppSystemID { get; private set; } = "RSCore";

        /// <summary>
        /// 用于注册当前应用ID，作为应用的唯一标识
        /// </summary>
        /// <param name="register"></param>
        /// <param name="SystemID"></param>
        /// <returns></returns>
        public static IAppRegister RegAppSystemID(this IAppRegister register, string SystemID)
        {
            App.InitAppID(SystemID);
            return register;
        }
        #endregion

        /// <summary>
        /// 当前请求服务提供对象（用于获取会话注册服务对象实例）
        /// </summary>
        public static Func<IServiceProvider> GetServiceProvider { get; internal set; }

        /// <summary>
        /// 获取指定类型的应用服务对象实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetAppService<T>() where T:IAppService
        {
            return GetService<T>();
        }

        public static T GetService<T>()
        {
            IServiceProvider RequestServices = null;
            if (GetServiceProvider != null)
                RequestServices = GetServiceProvider();
            else if (Global.Services != null)
                RequestServices = Global.Services;

            if (RequestServices == null)
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
