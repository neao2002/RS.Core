using RS.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RS.Lib.Demo
{
    internal class ShopService:AppServiceBase<ShopStore>,IShopService
    {
        public ShopService(IDbContext dbContext) : base(dbContext)
        { }

        public List<DynamicObj> GetShopInfos()
        {
            return store.GetShopInfos();
        }

        public async Task<DynamicObj> GetShopInfo(string ShopCode)
        {
            return await store.GetShopInfo(ShopCode);
        }
    }
}
