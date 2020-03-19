using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdminCoreWeb.Data.Redis;
using AdminCoreWeb.HostedService;
using AdminCoreWeb.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AdminCoreWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region csredis
            IConfigurationSection configurationSection = Configuration.GetSection("CsRedisConfig:DefaultConnectString");
            RedisHelper.Initialization(new CSRedis.CSRedisClient(configurationSection.Value));//初始化
            //services.AddScoped<IRedisClient, CustomerRedis>();
            #endregion

            #region 定时任务
            services.AddHostedService<RedisNumCallBackService>();
            //services.AddSingleton<IHostedService, RedisNumCallBackService>();
            #endregion
            services.AddSignalR();
            services.AddRazorPages();
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHubContext<ChatHub> chatHu, IHostApplicationLifetime appLifetime)
        {
            #region 控制程序生命周期
            // 发生在应用启动成功以后，也就是Startup.Configure()方法结束后。
            appLifetime.ApplicationStarted.Register(async () =>
            {
                
            });
            // 发生在程序正在完成正常退出的时候，所有请求都被处理完成。程序会在处理完这货的Action委托代码以后退出
            appLifetime.ApplicationStopped.Register(async () =>
            {
                //for (int i = 5; i > 0; i--)
                //{
                //    await chatHu.Clients.All.SendAsync("ReceiveMessage", "1", i + "s 后关闭程序");
                //    await Task.Delay(TimeSpan.FromSeconds(1));
                //}
            });
            // 发生在程序正在执行退出的过程中，此时还有请求正在被处理。应用程序也会等到这个事件完成后，再退出。
            //appLifetime.ApplicationStopping.Register(() =>
            //{

            //});
            #endregion
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<ChatHub>("/chathub");
                endpoints.MapRazorPages();
            });
        }
    }
}
