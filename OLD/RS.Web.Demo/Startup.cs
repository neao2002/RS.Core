using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace RS.Web.Demo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            //根据本置进行初始化注册
            Configuration.InitRegistrar()
                .RegGlobalDbContext("db:MainConnStr", Core.Data.DbDriverType.SQLServer) //根据配置初始化数据连接
                .RegLogger("Logger:LogType")//根据配置初始化应用调试跟踪日志
                .RegAppSystemID("Apollo_SelfReport");//注册当前应用系统ID，以便在本地化处理时所用
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(this.Configuration)//注册当前配置对象为单例模式            
               .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
               .AddAppDbContext()
               .AddAppServices();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            
            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
