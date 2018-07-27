using System;
using System.Collections.Generic;
using System.Text;

namespace RS
{
    public interface IRSServiceCollection
    {
        /// <summary>
        /// 以生命周期为Scoped方式注册应用服务
        /// </summary>
        /// <param name="serviceType">应用服务类型</param>
        /// <param name="implementationType"></param>
        /// <returns></returns>
        IRSServiceCollection AddScoped(Type serviceType, Type implementationType);
        /// <summary>
        /// 以生命周期为Transient方式注册应用服务
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationType"></param>
        /// <returns></returns>
        IRSServiceCollection AddTransient(Type serviceType, Type implementationType);

        IRSServiceCollection AddSingletion<TService>(TService implementationInstance) where TService : class;

        IRSServiceCollection AddScoped<TService>(Func<IServiceProvider, TService> implementationFactory) where TService : class;
    }

    
}
