using System;
using System.Collections.Generic;
using System.Text;

namespace RS.Web
{
    public interface IAppSessionOptions
    {
        /// <summary>
        /// 会话Cookie名称
        /// </summary>
        string SessionCookieName { get; }
        /// <summary>
        /// Session会话保存分钟
        /// </summary>
        int SessionExpires { get; set; } //默认20分钟
        /// <summary>
        /// 会话Cookie有效期（分钟）
        /// </summary>
        int SessionCookieExpires { get; set; }
        /// <summary>
        /// 是否采用数据库方式保存Cookie值，如是的话，则Cookie中只保存唯一ID
        /// 如果采用数据库方式保存，则必须要在主应用库增加Cookie相关操作表
        /// </summary>
        bool IsSqlSaveCookie { get; set; }
        /// <summary>
        /// 客户端是否可以访问Cookie
        /// </summary>
        bool HttpOnly { get; set; }
        /// <summary>
        /// Cookie是否非常重要
        /// </summary>
        bool IsEssential { get; set; }

        Func<string, IAppUser> ParseUser { get; }
    }
}
