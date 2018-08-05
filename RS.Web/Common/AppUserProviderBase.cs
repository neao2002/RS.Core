using System;
using System.Collections.Generic;
using System.Text;

namespace RS.Web
{
    /// <summary>
    /// 默认用户会话服务提供者
    /// </summary>
    public abstract class AppUserProviderBase<U,T> : IAppSessionProvider<U> where U : IAppUser where T:IAppUserProvider
    {
        public AppUserProviderBase(int userType, string CookieNameKey)
        {
            UserCookieNameKey = CookieNameKey;
            UserType = userType;
        }
        public virtual string UserCookieNameKey { get; private set; }

        private IAppSessionOptions options = null;
        public virtual IAppSessionOptions GetOptions()
        {
            if (options == null)
            {
                options = UserSessionOptions.CreateOptions(UserCookieNameKey, json => ParseUser(json));
            }
            return options;

        }

        private IAppSessionFactory factory = null;
        public IAppSessionFactory GetFactory()
        {

            if (factory == null)
                factory = new AppSessionFactory<U,T>(this);
            return factory;

        }

        public virtual int UserType { get; protected set; }

        public object GetService(Type serviceType)
        {
            return App.GetServiceProvider().GetService(serviceType);
        }

        public abstract string GetUserID(U user);


        public abstract bool IsBidden(U user);

        /// <summary>
        /// 默认是不需要区分是否为超级管理员
        /// </summary>
        /// <param name="appOperator"></param>
        /// <returns></returns>
        public abstract bool IsGlobalUser(U user);

        public abstract bool IsLogined(U user);


        public abstract string GetUserNo(U user);

        public abstract string GetUserName(U user);

        public virtual U ParseUser(string json)
        {
            return LibHelper.JSON.Deserialize<U>(json);
        }

        public virtual IAppOperator GetAppOperator()
        {
            return GetFactory().GetAppOperator();
        }

        public IAppOperator Login(IAppUser user)
        {
            return GetFactory().UserLogin(user);
        }

        public IAppUserProvider Logout()
        {
            GetFactory().EndUserSession();
            return this;
        }

        public IAppOperator SaveAppOperator(IAppUser user)
        {
            return GetFactory().SaveUserSession(user);
        }
    }
}
