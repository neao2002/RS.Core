using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace RS.Web
{
    /// <summary>
    /// 会话容器
    /// </summary>
    class UserSessionContainer<T> where T:IAppUserProvider
    {
        DynamicObj UserSessions;//保存当前所有会话操作用户对象信息
        IAppSessionOptions Options;
        public UserSessionContainer(IAppSessionOptions sessionOptions)
        {
            Options = sessionOptions;
            UserSessions = new DynamicObj(StringComparer.InvariantCultureIgnoreCase);
        }
        public IUserSession<T> GetUserSession(string Id)
        {
            string key =$"{Options.SessionCookieName}_{Id}";
            if (Options.IsSqlSaveCookie)  //Session是采用分布式保存，则会话直接按系统Session控制,这种多用于多应用服务器
            {
                return WebHelper.Current.Session.Get<UserSession<T>>(key);
            }
            else//这种只适合单应用服务器
            {
                return UserSessions.Get<UserSession<T>>(key);
            }
        }

        public void SetUserSession(string SessionID, IUserSession<T> session)
        {
            string key = $"{Options.SessionCookieName}_{SessionID}";
            if (Options.IsSqlSaveCookie)
                WebHelper.Current.Session.Set(key, session);
            else
                UserSessions.Set(key, session);
        }

        /// <summary>
        /// 根据会话ID获取会话对象
        /// </summary>
        /// <param name="SessionID"></param>
        /// <returns></returns>
        public IUserSession<T> GetUserSession(Guid SessionID)
        {
            if (Options.IsSqlSaveCookie)
                return null;
            else
            {
                KeyValuePair<string, object> item = UserSessions.First(p => ((IUserSession<T>)(p.Value)).SessionID == SessionID);
                if (item.IsNotNull())
                    return item.Value as IUserSession<T>;
                else
                    return null;
            }
        }
    }


    class AppSession<T> where T : IAppUserProvider
    {
        UserSessionContainer<T> Container;
        public AppSession(IAppSessionOptions sessionOptions)
        {
            Container = new UserSessionContainer<T>(sessionOptions);
        }
        public IUserSession<T> GetUserSession(string SessionID)
        {
            IUserSession<T> ae = null;
            try
            {
                ae = Container.GetUserSession(SessionID);
            }
            catch { }
            return ae;
        }

        public void SetUserSession(string Id, IUserSession<T> session)
        {            
            try
            {
                Container.SetUserSession(Id, session);
            }
            catch { }
            session.CacheKey = Id;
        }

        /// <summary>
        /// 创建一个新的会话用户
        /// </summary>
        /// <param name="SessionID"></param>
        /// <returns></returns>
        public IUserSession<T> AddNewUserSession(string Id)
        {
            IUserSession<T> session = new UserSession<T>(Id,Guid.NewGuid());
            SetUserSession(Id, session);
            return session;
        }

        /// <summary>
        /// 根据会话ID获取会话对象
        /// </summary>
        /// <param name="SessionID"></param>
        /// <returns></returns>
        public IUserSession<T> GetUserSession(Guid SessionID)
        {
            IUserSession<T> user =null;
            try
            {
                user=Container.GetUserSession(SessionID);
            }
            catch { }
            return user;
        }

        public string GetCurrentSessionID()
        {
            HttpContext page = WebHelper.Current;
            if (page != null && page.Session != null)
                return page.Session.Id;
            else
                return "";
        }
    }
}
