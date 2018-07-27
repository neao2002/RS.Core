using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RS.Data;

namespace RS.Lib.Demo
{
    internal class ShopStore :DALStoreBase
    {
        public ShopStore(IDbContext dbContext):base(dbContext)
        {
        }


        public List<DynamicObj> GetShopInfos()
        {
            return db.GetDynamicObjs($"select top 100 ShopCode,ShopName from TWD_ShopInfo with(nolock) order by ShopCode");
        }

        public async Task<DynamicObj> GetShopInfo(string ShopCode)
        {
            return db.GetDynamicObj($"select * from TWD_ShopInfo where ShopCode='{ShopCode}'");
        }
    }
}
