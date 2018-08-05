using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RS.Data;
using RS.Lib.Demo;

namespace RS.Web.MvcDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //模架基础组件初始化(以系统操作员角色作为整个Web平台的默认用户角色,对于只有一种用户角色的Web应用，则可不用参数)
            Configuration.GlobalRegistrar<ISysUserProvider,SysUserProvider>(new SysUserProvider())
                .RegRequestService(() => WebHelper.Current.RequestServices)
                .RegGlobalDbContext("db:MainConnStr", DbDriverType.SQLServer) //根据配置初始化数据连接
                .RegLogger("Logger:LogType")//根据配置初始化应用调试跟踪日志
                .RegAppSystemID("Test");//注册当前应用系统ID，以便在本地化处理时所用;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //注册服务对象和应用对象
            services.AddSingleton(this.Configuration)//注册当前配置对象为单例模式  
                    .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                    //注册添加用户角色供应者（单例模式）
                    .AddMoreUserProvider<IStudentUserProvider,StudentUserProvider>()
                    .AddMoreUserProvider<ITeacherUserProvider, TeacherUserProvider>()
                    //.AddDistributedMemoryCache()
                    .AddDistributedSqlServerCache(o =>
                    {
                        o.ConnectionString = "Data Source=.;Initial Catalog=RSDB;Integrated Security=false;User ID=sa;Password=sql;Min Pool Size=0;Max Pool Size=100;";
                        o.SchemaName = "dbo";
                        o.TableName = "ASPNET5SessionState";
                    })
                    .AddSession(options =>
                     {
                         // 设置10秒钟Session过期来测试
                         options.IdleTimeout = TimeSpan.FromSeconds(20);
                         options.Cookie.HttpOnly = true;
                     }) //增加对Session服务的支持                    
                    .AddAppDbContext()
                    .AddAppServices();


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //全局配置对象初始注册，以便为会话请求提供对象支持
            app.ApplicationServices.RegServiceProvider();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseSession();

            app.Use((context, next) =>
            {
                //return context.Response.WriteAsync(new Guid("fdsafdsafdsa").ToString());

                return next();
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.Use((context, next) => {
                return next();
            });
        }
    }
}
