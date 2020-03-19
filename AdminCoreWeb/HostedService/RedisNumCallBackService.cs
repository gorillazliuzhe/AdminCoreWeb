using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AdminCoreWeb.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace AdminCoreWeb.HostedService
{


    public class RedisNumCallBackService : BackgroundService
    {
        //protected override Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    Timer timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(3));
        //    return Task.CompletedTask;
        //}
        //private void DoWork(Object state)
        //{
        //    Console.WriteLine("DoWork");
        //}
        private readonly IHubContext<ChatHub> _chatHub;
        public RedisNumCallBackService(IHubContext<ChatHub> chatHub)
        {
            _chatHub = chatHub;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken).ContinueWith(async tsk =>
                {
                    await _chatHub.Clients.All.SendAsync("ReceiveMessage", "1", "开始定时任务", cancellationToken: stoppingToken);
                  
                    if (RedisHelper.Exists("lzclickcache"))
                    {
                        Stopwatch sp = new Stopwatch();
                        sp.Start();
                        await _chatHub.Clients.All.SendAsync("ReceiveMessage", "1", "获取hash集合开始", cancellationToken: stoppingToken);
                        var dc = await RedisHelper.HGetAllAsync("lzclickcache");
                        await _chatHub.Clients.All.SendAsync("ReceiveMessage", "1", "获取hash集合结束", cancellationToken: stoppingToken);
                        await _chatHub.Clients.All.SendAsync("ReceiveMessage", "1", "开始删除集合", cancellationToken: stoppingToken);
                        var pipe = RedisHelper.StartPipe();
                        foreach (var kv in dc)
                        {
                            pipe.HDel("lzclickcache", kv.Key);
                        }
                        pipe.EndPipe();
                        sp.Stop();
                        await _chatHub.Clients.All.SendAsync("ReceiveMessage", "1", "删除集合完成.耗时:"+ sp.ElapsedMilliseconds, cancellationToken: stoppingToken);

                    }
                    
                }, stoppingToken);
            }
        }
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
            for (int i = 5; i > 0; i--)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
