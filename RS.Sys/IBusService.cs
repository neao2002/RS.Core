using System;
using System.Collections.Generic;
using System.Text;
using RS.Core;
using RS.Data;

namespace RS.Sys
{
    public interface IBusService:IAppService
    {
        /// <summary>
        /// 获取所有店铺信息
        /// </summary>
        /// <returns></returns>
        List<DynamicObj> GetShopInfos();
    }

    public class BusService:IBusService
    {
        IDbContext db;
        public BusService(IDbContext dbContext)
        {
            db = dbContext;
        }

        public List<DynamicObj> GetShopInfos()
        {
            return db.GetDynamicObjs($"select top 100 ShopCode,ShopName from TWD_ShopInfo with(nolock) order by ShopCode");
        }
    }
}
