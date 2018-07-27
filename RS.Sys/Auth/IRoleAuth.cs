using System;
using System.Collections.Generic;
using System.Text;

namespace RS.Sys.Auth
{
    /// <summary>
    /// 功能角色关联对象
    /// </summary>
    public interface IRoleAuth
    {
        /// <summary>
        ///  角色
        /// </summary>
        string RoleID
        {
            get;
            set;
        }
        /// <summary>
        ///  具体功能点，可为空，如为空，则表示指定模块功能
        /// </summary>
        string FunID
        {
            get;
            set;
        }
        /// <summary>
        /// 所属模块功能
        /// </summary>
        string ModuleFunID { get; set; }
    }

    /// <summary>
    /// 用户所属角色关联对象
    /// </summary>
    public interface IRoleUser
    {
        string UserID { get; set; }
        string RoleID { get; set; }
    }
}
