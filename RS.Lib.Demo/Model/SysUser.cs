using System;
using System.Collections.Generic;
using System.Text;

namespace RS.Lib.Demo
{
    public class SysUser:  AppUserBase<ISysUserProvider>, IAppUser
    {
        public Guid UserID { get; set; }
        public string UserNo { get; set; }
        public string UserName { get; set; }
        public bool IsStop { get; set; }
        /// <summary>
        /// 用户审核状态:0-未审核，1-已提交待审,2-已审核生效，可以进行登录操作,3-账号已失效
        /// </summary>
        public int AuditStatus { get; set; }
        /// <summary>
        /// 是否为超级管理员,1为超级管理员，其它为系统操作员（根据权限不同，操作不同功能）
        /// </summary>
        public bool IsGlobalAdmin { get; set; }

        public bool IsLogined()
        {
            return UserID != Guid.Empty && AuditStatus == 2 && !IsStop;
        }


    }
}
