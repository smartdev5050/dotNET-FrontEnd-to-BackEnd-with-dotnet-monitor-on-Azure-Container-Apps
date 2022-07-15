using Banking.WebUI.Models;
using Banking.WebUI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Banking.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAccountBackendClient _accountBackendClient;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IAccountBackendClient accountBackendClient, ILogger<HomeController> logger)
        {
            _accountBackendClient = accountBackendClient;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var accounts = _accountBackendClient.GetAccounts().Result;

            return View(accounts);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}