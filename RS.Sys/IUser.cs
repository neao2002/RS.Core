using System;

namespace RS.Sys
{
    /// <summary>
    /// 当前系统操作受众用户标准接口
    /// </summary>
    public interface IUser
    {
        /// <summary>
        /// 用户ID(用户唯一ID，用于标识用户唯一性)
        /// 该属于用于确认用户身份和权限控制,根据应用随机实现
        /// </summary>
        string UserID { get; set; }
        /// <summary>
        /// 用户账号
        /// </summary>
        string UserNo { get; set; }
        /// <summary>
        /// 用户姓名
        /// </summary>
        string UserName { get; set; }

        /// <summary>
        /// 用户类型（具体用户由应用根据需求自行控制，这里只是实现接口）
        /// </summary>
        int UserType { get; set; }
        /// <summary>
        /// 是否禁用(主要是针对于授权的）
        /// </summary>
        bool IsBidden { get; set; }
        /// <summary>
        /// 是否超级管理员（这个主要是针对有授权用户才使用，即同样是登录用户，但有不同权限）
        /// </summary>
        bool IsGoladUser { get; set; }
        /// <summary>
        /// 应用实际用户实体序例化字符串，以便进行本地化保存
        /// </summary>
        string ExtendInfo { get; set; }
        /// <summary>
        /// 应用实际用户对象实体，从本地化获取时是通过解析实
        /// </summary>
        object Tag { get; set; }
    }
}
