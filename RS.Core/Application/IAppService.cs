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
        protected IDbContext db;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        public AppServiceBase(IDbContext dbContext) 
        {
            db = dbContext;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AppServiceBase<T> where T:DALStoreBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected IDbContext db;
        /// <summary>
        /// 
        /// </summary>
        protected T store;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        public AppServiceBase(IDbContext dbContext)
        {
            db = dbContext;
            store =App.CreateDALStore<T>(dbContext);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class DALStoreBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected IDbContext db;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        public DALStoreBase(IDbContext dbContext)
        {
            db = dbContext;
        }
    }
    
}
