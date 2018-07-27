using System;
using System.Collections.Generic;
using System.Text;

namespace RS.Sys.Auth
{
    public interface IRoleInfo
    {
        /// <summary>
        /// 角色ID，唯一标识
        /// </summary>
        string RoleID
        {
            get;
            set;
        }

        /// <summary>
        ///  角色名称，用于标识角色的名称
        /// </summary>
        string RoleName
        {
            get;
            set;
        }

        /// <summary>
        ///  角色类型
        /// </summary>
        RoleTypeEnum RoleType
        {
            get;
            set;
        }

        /// <summary>
        ///  上级部门角色
        ///  这样角色与角色之前形成树形层次关系
        /// </summary>
        string ParentID
        {
            get;
            set;
        }
        /// <summary>
        ///  是否停用
        /// </summary>
        bool IsBidden
        {
            get;
            set;
        }


        /// <summary>
        /// 获取或设置角色与功能权限关联表明细对象集
        /// </summary>
        List<IRoleAuth> GetRoleAuths();

        /// <summary>
        /// 获取或设置角色用户关联表明细对象集
        /// </summary>
        List<IRoleUser> GetRoleUsers();
    }


    #region 定义属性角色类型所需的枚举
    public enum RoleTypeEnum
    {
        /// <summary>
        /// 组织部门(对部门的授权只是确认部门的功能权限范围)
        /// </summary>
        DeptRole = 0,
        /// <summary>
        /// 用户角色
        /// </summary>
        UserRole = 1,
        /// <summary>
        /// 用户角色级别(实际授权中，并不需要对该角色时行授权，也不需要对该用户赋该角色)
        /// </summary>
        Positions = 2
    }
    #endregion 
}
