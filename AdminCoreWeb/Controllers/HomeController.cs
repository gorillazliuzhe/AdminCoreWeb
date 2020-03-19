using AdminCoreWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;
using AdminCoreWeb.Hubs;
using AdminCoreWeb.Services;
using Microsoft.AspNetCore.SignalR;

namespace AdminCoreWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHubContext<ChatHub> _chatHub;
        private readonly IHostApplicationLifetime _appLifetime;
        public HomeController(ILogger<HomeController> logger, IHostApplicationLifetime appLifetime, IHubContext<ChatHub> chatHub)
        {
            _logger = logger;
            _chatHub = chatHub;
            _appLifetime = appLifetime;
        }

        public async Task<IActionResult> Index()
        {
            //RedisDemo.InitHashData();
            await _chatHub.Clients.All.SendAsync("ReceiveMessage", "0", "连接成功");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// 自杀开关
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult BlowMeUp()
        {
            _appLifetime.StopApplication();
            return new EmptyResult();
        }

    }
}
