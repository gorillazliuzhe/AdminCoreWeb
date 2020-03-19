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
            RedisHelper.Initialization(new CSRedis.CSRedisClient(configurationSection.Value));//��ʼ��
            //services.AddScoped<IRedisClient, CustomerRedis>();
            #endregion

            #region ��ʱ����
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
            #region ���Ƴ�����������
            // ������Ӧ�������ɹ��Ժ�Ҳ����Startup.Configure()����������
            appLifetime.ApplicationStarted.Register(async () =>
            {
                
            });
            // �����ڳ���������������˳���ʱ���������󶼱�������ɡ�������ڴ����������Actionί�д����Ժ��˳�
            appLifetime.ApplicationStopped.Register(async () =>
            {
                //for (int i = 5; i > 0; i--)
                //{
                //    await chatHu.Clients.All.SendAsync("ReceiveMessage", "1", i + "s ��رճ���");
                //    await Task.Delay(TimeSpan.FromSeconds(1));
                //}
            });
            // �����ڳ�������ִ���˳��Ĺ����У���ʱ�����������ڱ�����Ӧ�ó���Ҳ��ȵ�����¼���ɺ����˳���
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
