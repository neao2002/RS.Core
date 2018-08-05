using System;

namespace RS.Sys.Auth
{
    /// <summary>
    /// 授权控制类，注意，在公共库中是没有任何作用，必须要外部系统注册后才有效果
    /// </summary>
    public static class PowerHelper
    {
        static Func<IAppOperator,string,bool> methodForCheckModuleFun;
        static Func<IAppOperator, Guid, bool> methodForCheckFun;
        static Func<IAppOperator, string, string, bool> methodForCheckMFandFun;
        static Func<IAppOperator> methodForGetUser;
        public static IAppOperator GetAppEnvironment()
        {
            if (methodForGetUser != null)
                return methodForGetUser();
            else
                return null;
        }

        /// <summary>
        /// 注册权限判定方法
        /// </summary>
        public static void RegiserPowerMethod(Func<IAppOperator> _methodForGetUser,Func<IAppOperator, string,bool> _methodForCheckModuleFun,Func<IAppOperator, Guid,bool> _methodForCheckFun,Func<IAppOperator, string,string,bool> _methodForCheckMFandFun)
        {            
            methodForGetUser = _methodForGetUser;

            methodForCheckFun = _methodForCheckFun;
            methodForCheckMFandFun = _methodForCheckMFandFun;
            methodForCheckModuleFun = _methodForCheckModuleFun;
        }

        /// <summary>
        /// 获取是否具有指定模块功能基本权限：根据功能点编号
        /// </summary>
        /// <returns></returns>
        public static bool CheckIsPower(this IAppOperator u, string ModuleFunCode)
        {
            if (methodForCheckModuleFun != null)
                return methodForCheckModuleFun(u, ModuleFunCode);
            else
                return true;
        }

        /// <summary>
        /// 获取是否具有指定模块功能基本权限：根据功能点编号
        /// </summary>
        /// <param name="u">当前操作用户</param>
        /// <param name="FunID">功能点ID</param>
        /// <returns>true-有权限；false-无权限</returns>
        public static bool CheckIsPower(this IAppOperator u, Guid FunID)
        {
            if (methodForCheckFun != null)
                return methodForCheckFun(u, FunID);
            else
                return true;
        }
        /// <summary>
        /// 获取是否具有指定模块功能基本查看权限
        /// </summary>
        /// <param name="u"></param>
        /// <param name="mf"></param>
        /// <returns></returns>
        public static bool CheckIsPower(this IAppOperator u,string ModuleFunCode,string FunCode)
        {
            if (methodForCheckMFandFun != null)
                return methodForCheckMFandFun(u, ModuleFunCode, FunCode);
            else
                return true;
        }
    }
}
