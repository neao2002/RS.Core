using System;
using System.Collections.Generic;
using System.Text;

namespace RS.Web
{
    internal class AppSessionFactory<U, T> : IAppSessionFactory where U : IAppUser where T : IAppUserProvider
    {
        #region 用户会话相关

        /// <summary>
        /// 保存各用户会话的环境信息
        /// </summary>
        private AppSession<T> AppSession;
        private SessionUrlGet UrlGetSession;
        private CookieFactory<T> factory;
        private IAppSessionProvider<U> Provider;
        private IAppSessionOptions Options;

        public AppSessionFactory(IAppSessionProvider<U> provider)
        {
            Options = provider.GetOptions();
            factory = new CookieFactory<T>(provider);
            AppSession = new AppSession<T>(Options);
            UrlGetSession = new SessionUrlGet();
            Provider = provider;
        }



        public IAppOperator UserLogin(IAppUser user)
        {
            return SaveUserSession(user);
        }
        /// <summary>
        /// 更新持久化保存
        /// </summary>
        /// <param name="appUser"></param>
        public IAppOperator SaveUserSession(IAppUser user)
        {
            IUserSession<T> appOperator = GetCurrUserSession();
            LoginUpdate(appOperator, (U)user);

            //进行持久化保存
            factory.SaveUserSession(appOperator, DateTime.Now.AddMinutes(Options.SessionCookieExpires));

            //session中也要同步更新
            AppSession.SetUserSession(appOperator.CacheKey, appOperator);
            return appOperator;
        }
        /// <summary>
        /// 用户注销后终止会话操作
        /// </summary>
        public IAppSessionFactory EndUserSession()
        {
            IUserSession<T> appOperator = GetCurrUserSession();
            Logout(appOperator);

            //从Cookie中移除
            factory.RemoveUserCookie(appOperator.SessionID);

            //再从会话容器中删除


            return this;
        }

        #region 加载并获取当前请求会话用户对象，只会加载一次
        public IAppOperator GetAppOperator()
        {
            return LoadAndGetUserSession();
        }
        /// <summary>
        /// 获取当前会话用户对象，如果没有找到，则从Cookie中查找
        /// </summary>
        /// <returns></returns>
        private IUserSession<T> LoadAndGetUserSession()
        {
            //先取本次缓存中的，如果没有，再次取这里的
            string sessionid = WebHelper.Current.Session.Id;
            IUserSession<T> userSession = AppSession.GetUserSession(sessionid);
            if (userSession == null) //会话中没有（这里有可能是重启了一个新的会话，导致新的ID无法定位到旧的会话对象中,则从Cookie中获取
            {
                userSession = factory.GetUserSession(AppSession);

                if (userSession == null) //cookie中也未取到，则视为未登录，创建一个全新操作用户
                {
                    userSession = AppSession.AddNewUserSession(sessionid);
                }
                else //更新会话对像ID，并重新保存到Session中去
                {
                    AppSession.SetUserSession(sessionid, userSession);
                }
            }
            return userSession;
        }
        #endregion
        private UserSession<T> GetCurrUserSession()
        {
            return (UserSession<T>)App.GetService<IAppOperatorUser<T>>();
        }


        #endregion


        #region 内部方法


        /// <summary>
        /// 用户登录后，更新当前操作用户为已登录用户信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userType"></param>
        /// <returns></returns>
        private IUserSession<T> LoginUpdate(IUserSession<T> opertor, U user)
        {
            opertor.UserID = Provider.GetUserID(user);
            opertor.UserName = Provider.GetUserName(user);
            opertor.UserNo = Provider.GetUserNo(user);
            opertor.UserType = Provider.UserType;
            opertor.User = user;
            return opertor;
        }

        public IUserSession<T> Logout(IUserSession<T> opertor)
        {
            opertor.UserID = "";
            opertor.UserName = "";
            opertor.UserNo = "";
            opertor.UserType = 0;
            opertor.User = null;
            return opertor;
        }
        #endregion
    }

    public interface IAppSessionFactory
    {
        IAppOperator GetAppOperator();

        IAppOperator UserLogin(IAppUser user);


        IAppOperator SaveUserSession(IAppUser user);

        IAppSessionFactory EndUserSession();
    }
}
