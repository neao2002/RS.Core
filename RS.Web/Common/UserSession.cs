using System;
using System.Collections.Generic;
using System.Text;

namespace RS.Web
{
    public interface IUserSession<T>:IAppOperatorUser<T> where T:IAppUserProvider 
    {
        string CacheKey { get; set; }
        Guid SessionID { get; set; }
    }
    class UserSession<T> : AppOperator,IUserSession<T> where T : IAppUserProvider
    {
        public UserSession(string Id,Guid sessionID)
        {
            CacheKey = Id;
            SessionID = sessionID;
        }
        public string CacheKey { get; set; }
        public Guid SessionID { get; set; }

    }
}
