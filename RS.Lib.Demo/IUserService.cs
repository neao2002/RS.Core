using RS.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace RS.Lib.Demo
{
    public interface IUserService : IAppService
    {
        UserShopInfo GetMyShopInfo(string UserID);
    }
}
