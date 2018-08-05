using System;
using System.Collections.Generic;
using System.Text;
using RS.Data;

namespace RS.Web
{
    public interface ICookieService:IAppService
    {
        /// <summary>
        /// 保存指定ID和名称的Cookie
        /// </summary>
        /// <param name="CookieID"></param>
        /// <param name="CookieName"></param>
        /// <param name="CookieValue"></param>
        /// <param name="ExpireTime"></param>
        /// <returns></returns>
        JsonReturn SaveCookie(Guid CookieID, string CookieName, string CookieValue, DateTime ExpireTime);

        /// <summary>
        /// 获取指定ID的Cookie
        /// </summary>
        /// <param name="CookieID"></param>
        /// <returns></returns>
        string GetCookieValue(Guid CookieID);

        void DeleteCookie(Guid CookieID);
    }

    internal class CookieService : AppServiceBase<CookieStore>, ICookieService
    {
        public CookieService(IDbContext dbContext) : base(dbContext)
        {
        }

        public string GetCookieValue(Guid CookieID)
        {
            return store.GetCookieValue(CookieID);
        }

        public JsonReturn SaveCookie(Guid CookieID, string CookieName, string CookieValue, DateTime ExpireTime)
        {
            return store.SaveCookie(CookieID, CookieName, CookieValue, ExpireTime);
        }
        public void DeleteCookie(Guid CookieID)
        {
            store.DeleteCookie(CookieID);
        }
    }
}
