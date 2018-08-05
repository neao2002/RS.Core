using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RS.Data;

namespace RS.Lib.Demo
{
    internal class UserStore:DALStoreBase
    {
        public UserStore(IDbContext dbContext) : base(dbContext)
        {
            
        }

        public UserShopInfo GetMyShopInfo(string UserID,IShopService shopSvr)
        {
            UserShopInfo user = db.GetEntity<UserShopInfo>(dr => DBHelper.CreateObject<UserShopInfo>(dr), $@"select a.UserID,a.UserName,b.ShopCode
from TWD_UserInfo a with(nolock) inner join
     TWD_ShopInfo b with(nolock) on a.UserID = b.UserID
 where a.UserID = '{UserID}'");
            if (user != null)
            {
                //这里加上事务，检测是否在同一数据上下文创建
                db.RunTransaction(() => {
                    //先取店编号，在这里就会锁定这个表
                    string ShopCode = db.GetField<string>($"select ShopCode from TWD_ShopInfo where UserID='{UserID}'");

                    //如果是同一个数据上下文对象，下面代码是不可能执行的，因为已经锁在里面了
                    Task<DynamicObj> v = shopSvr.GetShopInfo(ShopCode);
                    user.Shop = v.Result;
                });
            }
            return user;
        }
    }
}
