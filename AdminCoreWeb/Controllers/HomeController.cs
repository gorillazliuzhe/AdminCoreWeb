using AdminCoreWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using AdminCoreWeb.Services;

namespace AdminCoreWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHostApplicationLifetime _appLifetime;
        public HomeController(ILogger<HomeController> logger, IHostApplicationLifetime appLifetime)
        {
            _logger = logger;
            _appLifetime = appLifetime;
        }

        public IActionResult Index()
        {
           // RedisDemo.InitHashData();

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
