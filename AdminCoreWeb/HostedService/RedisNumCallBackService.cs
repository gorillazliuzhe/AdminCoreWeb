using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

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

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine($"{DateTime.Now:mm:ss} working...");
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                Stopwatch sp = new Stopwatch();
                sp.Start();
                var dc = await RedisHelper.HGetAllAsync("lzclickcache");
                sp.Stop();
                Console.WriteLine(sp.ElapsedMilliseconds);
            }
        }
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
            //for (int i = 5; i > 0; i--)
            //{
            //    Console.WriteLine(i + "s 后关闭");
            //    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            //}
        }
    }
}
