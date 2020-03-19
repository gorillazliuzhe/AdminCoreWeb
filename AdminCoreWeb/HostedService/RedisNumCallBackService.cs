using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AdminCoreWeb.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Linq;

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
                await DataTime(stoppingToken);
            }
        }


        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
            await DataTime(cancellationToken);
        }
        private async Task DataTime(CancellationToken stoppingToken)
        {
            await _chatHub.Clients.All.SendAsync("ReceiveMessage", "1", "开始定时任务", cancellationToken: stoppingToken);

            if (RedisHelper.Exists("xxxooo"))
            {
                Stopwatch sp = new Stopwatch();
                sp.Start();

                #region list

                var listlength = await RedisHelper.LLenAsync("xxxooo");
                var strlist = await RedisHelper.LRangeAsync<int>("xxxooo", 0, listlength);
                var pipe = RedisHelper.StartPipe();
                for (int i = 0; i < strlist.Length; i++)
                {
                    pipe.LPop<int>("xxxooo");
                }

                pipe.EndPipe();

                #endregion

                Dictionary<int, int> fc = strlist.ToList().GroupBy(s => s).ToDictionary(item => item.Key, item => item.Count());
                sp.Stop();
                await _chatHub.Clients.All.SendAsync("ReceiveMessage", "1", "删除集合完成.耗时:" + sp.ElapsedMilliseconds,
                    cancellationToken: stoppingToken);
            }
            await Task.Delay(5000, stoppingToken);
        }
    }
}
