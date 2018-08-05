using RS.Lib.Demo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RS.Web.MvcDemo
{
    public class SysUserProvider : AppUserProviderBase<SysUser, ISysUserProvider>, ISysUserProvider
    {
        public SysUserProvider() : base(0,"SysUser")
        {
        }

        public override IAppSessionOptions GetOptions()
        {
            IAppSessionOptions options = base.GetOptions();
            options.IsSqlSaveCookie = true;
            return options;
        }
        public override string GetUserID(SysUser user)
        {
            return user.UserID.ToString();
        }

        public override string GetUserName(SysUser user)
        {
            return user.UserName;
        }

        public override string GetUserNo(SysUser user)
        {
            return user.UserNo;
        }

        public override bool IsBidden(SysUser user)
        {
            return user.IsStop;
        }

        public override bool IsGlobalUser(SysUser user)
        {
            return user.IsGlobalAdmin;
        }

        public override bool IsLogined(SysUser user)
        {
            //只有有效用户才可登录，其它都视为未登录
            return user.IsLogined();
        }

    }
}
