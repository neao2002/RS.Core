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
using RS.Web;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtension
    {
        #region 全局对象初始注册
        /// <summary>
        /// 应用全局对象初始化注册
        /// </summary>
        /// <param name="config">系统全局配置对象</param>
        /// <returns>返回注册手柄，以便继续注册</returns>
        public static IAppRegister GlobalRegistrar<U>(this IConfiguration config) where U:DefaultUser
        {
            Global.RegDefaultUserProvider<IDefaultUserProvider,DefaultUserProvider<U>>(new DefaultUserProvider<U>());
            return Global.GlobalRegistrar(RSConfiguration.ToRSConfiguration(config), () => LoadCompileAssemblies());
        }
        public static IAppRegister GlobalRegistrar(this IConfiguration config) 
        {
            Global.RegDefaultUserProvider<IDefaultUserProvider, DefaultUserProvider<DefaultUser>>(new DefaultUserProvider<DefaultUser>());
            return Global.GlobalRegistrar(RSConfiguration.ToRSConfiguration(config), () => LoadCompileAssemblies());
        }
        /// <summary>
        /// 全局应用集供应者初始注册
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static IServiceProvider RegServiceProvider(this IServiceProvider provider)
        {
            Global.RegServiceProvider(provider);
            return provider;
        }

        public static IAppRegister GlobalRegistrar<T,E>(this IConfiguration config, E userProvider) where T:IAppUserProvider where E: T
        {
            Global.RegDefaultUserProvider<T,E>(userProvider);
            return Global.GlobalRegistrar(RSConfiguration.ToRSConfiguration(config), () => LoadCompileAssemblies());
        }
        #endregion

        #region 业务应用服务组件注册
        /// <summary>
        /// 进行数据库服务对象注册
        /// </summary>
        /// <param name="services">服务对象集</param>
        /// <returns>返回服务对象集，以便继续进行注册</returns>
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
        /// 注册当前所有业务应用服务
        /// </summary>
        /// <param name="services">服务对象集</param>
        /// <returns>返回服务对象集，以便继续进行注册</returns>
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            //默认用户角色服务注册
            if (Global.DefaultUserProvider != null)
            {
                Type type = Global.DefaultUserProviderType;// .DefaultUserProvider.GetType();
                services.AddSingleton(type, Global.DefaultUserProvider);
                services.AddScoped(Global.DefaultUserType,a => Global.DefaultUserProvider.GetAppOperator());
            }

            RSServiceCollection.Services(services).RegisterAppServices();
            return services;
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

        #endregion

        #region 用户会话注册

        public static IServiceCollection AddMoreUserProvider<TService, TImplementation>(this IServiceCollection services) where TService :class, IAppUserProvider
                                                                                                                           where TImplementation : class, TService
        {
            services.AddSingleton<TService,TImplementation>();

            services.AddScoped<IAppOperatorUser<TService>>(a =>
            {
                return (IAppOperatorUser<TService>)a.GetService<TService>().GetAppOperator();
            });

            return services;
        }
       

        #endregion

    }
}

