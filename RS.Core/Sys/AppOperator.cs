using System;
using System.Collections.Generic;
using System.Text;

namespace RS
{
    /// <summary>
    /// 应用操作人员
    /// 本对象包含操作人员信息，及其对应业务用户实体情况
    /// </summary>
    public class AppOperator : IAppOperator
    {
        protected AppOperator()
        { }
        /// <summary>
        /// 用户ID(用户唯一ID，用于标识用户唯一性)
        /// 该属于用于确认用户身份和权限控制,根据应用随机实现
        /// </summary>
        public string UserID {get;set;}
        /// <summary>
        /// 用户账号
        /// </summary>
        public string UserNo { get;set; } = "";
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string UserName { get;set; } = "";
        /// <summary>
        /// 用户类型（具体用户由应用根据需求自行控制，这里只是实现接口）
        /// </summary>
        public int UserType { get; set; } = 0;
        /// <summary>
        /// 扩展属性信息（字数不能过长）
        /// </summary>
        public string ExtendInfo { get;set; } = "";
        /// <summary>
        /// 自定义对象
        /// </summary>
        public IAppUser User { get;set; }

        public bool IsLogined()
        {
            return User != null && User.IsLogined();
        }
    }


}
