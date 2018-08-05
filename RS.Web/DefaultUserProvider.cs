using System;
using System.Collections.Generic;
using System.Text;

namespace RS.Web
{
    public interface IDefaultUserProvider:IAppUserProvider
    { }
    /// <summary>
    /// 默认用户供应者
    /// 适合于单用户群体Web系统
    /// </summary>
    public class DefaultUserProvider<T> : AppUserProviderBase<T, IDefaultUserProvider>, IDefaultUserProvider where T:DefaultUser
    {        
        public DefaultUserProvider() : base(0, "User")
        {
        }
        public DefaultUserProvider(int UserType,string CookieNameKey) : base(UserType,CookieNameKey)
        {
        }
        public override string GetUserID(T user)
        {
            return user.UserNo;
        }

        public override string GetUserName(T user)
        {
            return user.UserName;
        }

        public override string GetUserNo(T user)
        {
            return user.UserNo;
        }

        public override bool IsBidden(T user)
        {
            return false;
        }

        public override bool IsGlobalUser(T user)
        {
            return false;
        }

        public override bool IsLogined(T user)
        {
            return user.IsLogined();
        }
    }
    public class DefaultUser : AppUserBase<IDefaultUserProvider>, IAppUser
    {
        public virtual string UserNo { get; set; }
        public virtual string UserName { get; set; }
        public bool IsLogined()
        {
            return UserNo.IsNotWhiteSpace();
        }
    }
}
