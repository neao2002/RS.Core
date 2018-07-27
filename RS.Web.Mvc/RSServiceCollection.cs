using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace RS.Web.Mvc
{
    public class RSServiceCollection : IRSServiceCollection
    {
        private IServiceCollection services;
        private RSServiceCollection(IServiceCollection _services)
        {
            services = _services;
        }
        public IRSServiceCollection AddScoped(Type serviceType, Type implementationType)
        {
            services.AddScoped(serviceType, implementationType);
            return this;
        }

        public IRSServiceCollection AddScoped<TService>(Func<IServiceProvider, TService> implementationFactory) where TService : class
        {
            services.AddScoped<TService>(implementationFactory);
            return this;
        }

        public IRSServiceCollection AddSingletion<TService>(TService implementationInstance) where TService : class
        {
            services.AddSingleton<TService>(implementationInstance);
            return this;
        }

        public IRSServiceCollection AddTransient(Type serviceType, Type implementationType)
        {
            services.AddTransient(serviceType, implementationType);
            return this;
        }

        public static IRSServiceCollection Services(IServiceCollection services)
        {
            return new RSServiceCollection(services);
        }



    }
}
