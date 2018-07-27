using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyModel;
using RS;
using RS.Data;
using RS.Web.Mvc;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtension
    {
        public static IAppRegister GlobalRegistrar(this IConfiguration config)
        {
            return Global.GlobalRegistrar(RSConfiguration.ToRSConfiguration(config),()=>LoadCompileAssemblies());
        }
        public static IServiceCollection AddAppDbContext(this IServiceCollection services)
        {
            services.AddSingleton<IDbContextFactory>(Global.DbContextFactory);//以Globals.DbContextFactory作为全局数据库上下文构建工厂
            //注册会话所用数据库上下文，即在一个请求会话时，使用同一数据数上下文，这里必须保存在会话中使用同一个数据库上下文对象，实际应用中始需单单独创建，则自行控制
            services.AddScoped<IDbContext>(a =>
            {
                return a.GetService<IDbContextFactory>().CreateContext();
            });
            return services;
        }
        /// <summary>
        /// 注册当前所有应用服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            RSServiceCollection.Services(services).RegisterAppServices();
            return services;
        }

        public static IServiceProvider RegServiceProvider(this IServiceProvider provider)
        {
            Global.RegServiceProvider(provider);
            return provider;
        }


        public static List<Assembly> LoadCompileAssemblies()
        {
            List<CompilationLibrary> libs = DependencyContext.Default.CompileLibraries.Where(lib => !lib.Serviceable && lib.Type != "package").ToList();

            List<Assembly> ret = new List<Assembly>();
            foreach (var lib in libs)
            {
                Assembly assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name));
                ret.Add(assembly);
            }

            return ret;
        }
    }
}

