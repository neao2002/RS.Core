using RS.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace RS.Lib.Demo
{
    internal class UserService:AppServiceBase<UserStore>, IUserService
    {
        public UserService(IDbContext dbContext) : base(dbContext)
        { }

        public UserShopInfo GetMyShopInfo(string UserID)
        {
            return store.GetMyShopInfo(UserID, App.GetAppService<IShopService>());
        }
    }
}
