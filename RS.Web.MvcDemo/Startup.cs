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

namespace RS.Web.MvcDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //模架基础组件初始化
            Configuration.GlobalRegistrar()
                .RegRequestService(()=>WebHelper.Current.RequestServices)
                .RegGlobalDbContext("db:MainConnStr", DbDriverType.SQLServer) //根据配置初始化数据连接
                .RegLogger("Logger:LogType")//根据配置初始化应用调试跟踪日志
                .RegAppSystemID("Apollo_SelfReport");//注册当前应用系统ID，以便在本地化处理时所用;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //注册服务对象和应用对象
            services.AddSingleton(this.Configuration)//注册当前配置对象为单例模式  
                    .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
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

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
