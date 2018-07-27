using System;
using System.Collections.Generic;
using System.Text;
using RS.Data;
using RS;
using System.Threading.Tasks;

namespace RS.Lib.Demo
{
    public interface IShopService : IAppService
    {
        /// <summary>
        /// 获取所有店铺信息
        /// </summary>
        /// <returns></returns>
        List<DynamicObj> GetShopInfos();
        Task<DynamicObj> GetShopInfo(string ShopCode);
    }
    
    
}
