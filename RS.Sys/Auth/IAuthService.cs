using System;
using System.Collections.Generic;
using System.Text;


namespace RS.Sys.Auth
{
    /// <summary>
    /// 系统权限控制服务
    /// (采用依赖注入方式，获取到系统用户及角色授权信息)
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// 获取指定ID的模块功能实体
        /// </summary>
        /// <param name="ModuleFunID"></param>
        /// <returns></returns>
        IModuleFun GetModuleFun(string ModuleFunID);

        bool CheckIsAuth(string ModuleFunID, string FunID);
    }
}
