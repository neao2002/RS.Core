using System;
using System.Collections.Generic;
using System.Text;
using RS.Data;

namespace RS.Web
{
    internal class SessionStore : DALStoreBase
    {
        public SessionStore(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}
