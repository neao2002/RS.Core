using System;
using System.Collections.Generic;
using System.Text;

namespace RS.Sys
{
    /// <summary>
    /// 应用授权服务，用于对指定业务系统进行权限控权判断
    /// </summary>
    public interface IAuthService:IAppService
    {
        /// <summary>
        /// 判断当前访问用户是否有权限，这里实质是判是否为有效用户，即是已登录用户
        /// 只应用于不需要对具体模块功能进行授权的业务系统，如网站个人中心等，只要登录就可操作
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        bool IsHasPower(IAppOperator user);
        /// <summary>
        /// 判断指定用户对指定模块功能是否具备访问
        /// </summary>
        /// <param name="user">操作用户</param>
        /// <param name="ModuleFunID">模块功能</param>
        /// <returns>是否具体权限</returns>
        bool IsHasPower(IAppOperator user,string ModuleFunID);
        /// <summary>
        /// 判断指定用户对指定模块功能和功能点是否具备访问
        /// 注意：首先必须要用该功能权限，然后才必须要用功能点权限
        /// </summary>
        /// <param name="user">操作用户</param>
        /// <param name="ModuleFunID">模块功能</param>
        /// <param name="FunID">该模块功能下具体功能点</param>
        /// <returns>是否具体权限</returns>
        bool IsHasPower(IAppOperator user, string ModuleFunID, string FunID);
    }
}
