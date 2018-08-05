using System;
using System.Collections.Generic;
using System.Text;

namespace RS
{ 
    /// <summary>
    /// 操作用户标准接口
    /// 在同一请求只，系统只能是一个用户，不能为多个用户
    /// 操作用户包含用户基本信息资料及当前用户端基本环境信息
    /// </summary>
    public interface IAppOperator
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
        /// 应用实际用户实体序例化字符串，以便进行本地化保存
        /// </summary>
        string ExtendInfo { get; set; }
        /// <summary>
        /// 应用实际用户对象实体，从本地化获取时是通过解析实
        /// </summary>
        IAppUser User { get; set; }

        bool IsLogined();
    }

    public interface IAppOperatorUser<T>:IAppOperator where T : IAppUserProvider
    { }
}
