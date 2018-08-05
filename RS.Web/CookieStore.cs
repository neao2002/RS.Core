using System;
using System.Collections.Generic;
using System.Text;
using RS.Data;

namespace RS.Web
{
    internal class CookieStore : DALStoreBase
    {
        public CookieStore(IDbContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// 保存Cookie信息，并得到ID
        /// 注意，这里不能使用事务锁
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public JsonReturn SaveCookie(Guid CookieID, string CookieName, string CookieValue, DateTime ExpireTime)
        {
            DateTime today = DateTime.Now;
            if (ExpireTime < today) return JsonReturn.RunFail("无法存储Cookie信息：该Cookie已失效");

            //先检测是否存在该CookieID
            if (CookieID != Guid.Empty) //当前该Cookie还保存，可能是存放的另外一个
            {
                //删除现有保存的Cookie
                db.ExecuteCommand(string.Format("delete from TSys_LoginUserCookie where CookieID='{0}'", CookieID));
            }
            else
                CookieID = Guid.NewGuid();//创建一个新的CookieID,用于标识该Cookie

            try
            {
                DynamicObj Cookie = new DynamicObj();
                Cookie.Set("CookieID", CookieID);
                Cookie.Set("UserInfoEntity", CookieValue);
                Cookie.Set("Expires", ExpireTime);
                Cookie.Set("CookieName", CookieName);
                Cookie.Set("ReadTime", DateTime.Now);

                //仅为单表，暂不进行事务处理
                if (db.SaveNewObject(Cookie, "TSys_LoginUserCookie"))
                    return JsonReturn.RunSuccess(CookieID);
                else
                    return JsonReturn.RunFail("无法存储Cookie信息");
            }
            catch
            {
                return JsonReturn.RunFail("存储Cookie信息时出现异常");
            }
        }
        /// <summary>
        /// 更新会话，每当客户端时行请求时，先更新一下会话,以便使会话永远保持最新
        /// </summary>
        /// <param name="SessionID"></param>
        private CookieStore RemoveExpiresCookie()
        {
            //为防止锁定，这里不启用事务
            DateTime today = DateTime.Now;
            //选清空已失效的会话
            db.ExecuteCommand($"delete from TSys_LoginUserCookie where Expires<{db.Function.DateTimeValue(today)}");
            return this;
        }
        /// <summary>
        /// 根据指定会话ID获取会话信息,如果指定会话不在，则返回空
        /// </summary>
        /// <param name="SessionID"></param>
        /// <returns></returns>
        public string GetCookieValue(Guid CookieID)
        {
            string json = "";
            RemoveExpiresCookie();

            DynamicObj rec = db.GetDynamicObj(string.Format("select * from TSys_LoginUserCookie with(nolock) where CookieID='{0}'", CookieID));
            if (rec != null)
            {
                json = rec.Get<string>("UserInfoEntity");

                DateTime readTime = rec.Get<DateTime>("ReadTime");
                DateTime Expires = rec.Get<DateTime>("Expires");

                //取到两者之前间隔，则可知道Cookie保存长度
                TimeSpan ts = Expires - readTime;

                Expires.AddTicks(ts.Ticks);

                //再次更新
                db.ExecuteCommand(string.Format("update TSys_LoginUserCookie set ReadTime={0},Expires={1} where CookieID='{2}'", db.Function.DateTimeValue(DateTime.Now), db.Function.DateTimeValue(Expires), CookieID));
            }
            return json;
        }

        public void DeleteCookie(Guid CookieID)
        {
            if (CookieID!=Guid.Empty)
                db.ExecuteCommand(string.Format("delete from TSys_LoginUserCookie where CookieID='{0}'", CookieID));
        }
    }
}
