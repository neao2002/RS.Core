using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace RS.Web
{
    internal class CookieFactory<T> where T:IAppUserProvider
    {
        private IAppSessionProvider Provider;
        IAppSessionOptions Options;
        public CookieFactory(IAppSessionProvider provider)
        {
            Options = provider.GetOptions();
            Provider = provider;
        }
        /// <summary>
        /// 获取保存在当前库
        /// </summary>
        /// <returns></returns>
        public IUserSession<T> GetUserSession(AppSession<T> appSession)
        {
            string CookieValue = GetCookies(Options.SessionCookieName);
            if (CookieValue.IsWhiteSpace()) //找不到Cookie，则表示过期了
                return null;



            DynamicObj cookie = LibHelper.JSON.DeserializeDynamicObj(CookieValue.Decrypt());
            Guid SessionID = cookie.Get<Guid>("SessionID");
            
            //再次从会话Session中查找，找到，就直接返回，没找到，则从Cookie中加载
            IUserSession<T> u = appSession.GetUserSession(SessionID);
            if (u != null) return u;
            string User;
            if (Options.IsSqlSaveCookie)
            {
                User= App.GetAppService<ICookieService>().GetCookieValue(SessionID).Decrypt();
            }
            else
            {
                User = cookie.Get<string>("User");
            }            
            try
            {
                u = LibHelper.JSON.Deserialize<UserSession<T>>(User);
                if (u != null)
                {
                    Func<string, IAppUser> parseUser = Options.ParseUser;
                    if (parseUser != null && u.ExtendInfo.IsNotWhiteSpace()) //还原真实的用户信息
                    {
                        IAppUser au = parseUser(u.ExtendInfo);
                        if (au != null)
                        {
                            u.User = au;
                        }
                        else
                            u.User = null;//则当前操作者并没有用户信息，视为游客

                        u.ExtendInfo = "";
                    }
                }
            }
            catch(Exception ex)
            {
                Loger.Error("读取Cookie用户信息出错误" + ex.Message);
            }
            return u;
        }

        public CookieFactory<T> SaveUserSession(IUserSession<T> userSession, DateTime ExpireTime)
        {
            object cv;
            string CookieValue = "";
            if (Options.IsSqlSaveCookie) //是保存到数据库中,则Cookie只保存ID,及一空对象体
            {
                cv = new { SessionID = userSession.SessionID, User = "" };

                //产生一个新的复本，用于保存
                IAppOperator u =GetClone( userSession);
                if (u.User != null)
                    u.ExtendInfo = LibHelper.JSON.Serialize(u.User);
                else
                    u.ExtendInfo = "";

                u.User = null;

                CookieValue = LibHelper.JSON.Serialize(u).Encrypt(); //保存到Cookie的为加密对象

                ICookieService svr = App.GetAppService<ICookieService>();
                svr.SaveCookie(userSession.SessionID, Options.SessionCookieName, CookieValue, ExpireTime);
            }
            else
            {
                //产生一个新的复本，用于保存
                IAppOperator u =GetClone(userSession);
                if (u.User != null)
                    u.ExtendInfo = LibHelper.JSON.Serialize(u.User);
                else
                    u.ExtendInfo = "";

                u.User = null;
                cv = new { SessionID =userSession.SessionID, User = LibHelper.JSON.Serialize(u) };
            }

            //实际Cookie只保存cv;

            CookieValue = LibHelper.JSON.Serialize(cv).Encrypt(); //保存到Cookie的为加密对象

            //本地保存Cookie
            SetCookies(Options.SessionCookieName, CookieValue, ExpireTime);

            return this;
        }

        /// <summary>
        /// 设置本地cookie
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>  
        /// <param name="minutes">过期时长，单位：分钟</param>      
        public void SetCookies(string key, string value,DateTime ExpireTime)
        {
            HttpContext page = WebHelper.Current;
            WebHelper.Current.Response.Cookies.Append(key, value, new CookieOptions
            {
                //Domain=page.Request.Host.Value,                
                IsEssential=Options.IsEssential,
                HttpOnly=Options.HttpOnly,
                Expires =ExpireTime
            });
        }

        public void RemoveUserCookie(Guid SessionID)
        {
            if (Options.IsSqlSaveCookie) //如果是数据库存储
            {
                App.GetAppService<ICookieService>().DeleteCookie(SessionID);
            }

            //将Cookie移除
            DeleteCookies(Options.SessionCookieName);
        }

        /// <summary>
        /// 删除指定的cookie
        /// </summary>
        /// <param name="key">键</param>
        public void DeleteCookies(string key)
        {
            WebHelper.Current.Response.Cookies.Delete(key);
        }

        /// <summary>
        /// 获取cookies
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>返回对应的值</returns>
        public string GetCookies(string key)
        {
            WebHelper.Current.Request.Cookies.TryGetValue(key, out string value);
            if (string.IsNullOrEmpty(value))
                value = string.Empty;
               

            return value;
        }

        /// <summary>
        /// 获取指定用户的复本
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        private IUserSession<T> GetClone(IUserSession<T> user)
        {
            return new UserSession<T>(user.CacheKey,user.SessionID)
            {
                UserID = user.UserID,
                UserName = user.UserName,
                UserNo = user.UserNo,
                UserType = user.UserType,
                User = user.User,
                SessionID = user.SessionID
            };
        }
    }
   
    /// <summary>
    /// 框架Session项选
    /// </summary>
    public class UserSessionOptions : IAppSessionOptions
    {
        /// <summary>
        /// 构建函数不对外公开
        /// </summary>
        protected UserSessionOptions()
        { }
        /// <summary>
        /// 会话Cookie名称
        /// </summary>
        public string SessionCookieName { get; protected set; }= $"{App.AppSystemID}_SessionCookieName";
        /// <summary>
        /// Session会话保存分钟
        /// </summary>
        public int SessionExpires { get; set; } = 20;//默认20分钟
        /// <summary>
        /// 会话Cookie有效期（分钟）
        /// </summary>
        public int SessionCookieExpires { get; set; } = 24 * 60;
        /// <summary>
        /// 是否采用数据库方式保存Cookie值，如是的话，则Cookie中只保存唯一ID
        /// 如果采用数据库方式保存，则必须要在主应用库增加Cookie相关操作表
        /// </summary>
        public bool IsSqlSaveCookie { get; set; } = false;
        /// <summary>
        /// 客户端是否可以访问Cookie
        /// </summary>
        public bool HttpOnly { get; set; } = true;
        /// <summary>
        /// Cookie是否非常重要
        /// </summary>
        public bool IsEssential { get; set; } = false;

        public Func<string, IAppUser> ParseUser{ get; private set; }
        /// <summary>
        /// 创建该群体下用户角色健值
        /// </summary>
        /// <param name="UserTypeSessionKey"></param>
        /// <returns></returns>
        public static UserSessionOptions CreateOptions(string UserTypeSessionKey,Func<string,IAppUser> ParseUser)
        {
            return new UserSessionOptions() {
                SessionCookieName = $"{App.AppSystemID}_{UserTypeSessionKey}SCookie",
                ParseUser=json=> ParseUser(json)
            };
        }
    }
}
