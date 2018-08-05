using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace RS.Web
{
    public static class WebHelper
    {
        private static SessionUrlGet UrlGetSession;
        static WebHelper()
        {
            UrlGetSession = new SessionUrlGet();
        }
        public static HttpContext Current
        {
            get
            {
                object factory = Global.Services.GetService(typeof(IHttpContextAccessor));
                HttpContext context = ((HttpContextAccessor)factory).HttpContext;
                return context;
            }
        }

        #region 用于站内Get 传递参数相关方法
        /// <summary>
        /// 在进行get之前设置参数
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="s"></param>
        public static void SetGetUrlParams(string Key, object s)
        {
            DynamicObj curParams = UrlGetSession[Current.Session.Id];
            curParams.Set(Key, s);
        }
        /// <summary>
        /// 传递后取到参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static T GetUrlParams<T>(string Key)
        {
            DynamicObj curParams = UrlGetSession[Current.Session.Id];
            return curParams.Get<T>(Key);
        }
        #endregion
    }
}
