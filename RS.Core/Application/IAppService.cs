using RS.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace RS
{
    /// <summary>
    /// 应用服务标准接口，为所有对外公开应用服务接口
    /// 以便服务供应能采用依赖注入方式构建应用服务对象实例
    /// </summary>
    public interface IAppService
    {
    }
    /// <summary>
    /// 具有生命周期的应用服务，在运行周期内(一般情况下就是同一线程内)服务供应者只创建一个对象实体
    /// </summary>
    public class AppServiceBase
    {
        public IDbContext DbContext { get; set; }
        public AppServiceBase(IDbContext dbContext) 
        {
            DbContext = dbContext;
        }
    }
}
