using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SignalR.Hubs;
using SignalR.Models;
using SignalR.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _siginInManager;
        private readonly IHubContext<ChatHub> _hubContext;

        public HomeController(ILogger<HomeController> logger,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> siginInManager,
            IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
            _logger = logger;
            _userManager = userManager;
            _siginInManager = siginInManager;
        }

        public IActionResult Index()
        {
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
        public IActionResult Chat()
        {
            return View();
        }
        public async Task<IActionResult> CreateUser()
        {
            var user1 = new AppUser { FullName = "Kamran", UserName = "_Kamran" };
            var user2 = new AppUser { FullName = "Gunel", UserName = "_Gunel" };
            var user3 = new AppUser { FullName = "Murad", UserName = "_Murad" };

            await _userManager.CreateAsync(user1, "12345@Ki");
            await _userManager.CreateAsync(user2, "12345Gt");
            await _userManager.CreateAsync(user3, "12345Mu");

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Login()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            AppUser appUser = await _userManager.FindByNameAsync(loginVM.UserName);

            var result = await _siginInManager.PasswordSignInAsync(appUser, loginVM.Password, true, true);

            if (appUser ==null)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Chat));
        }
        public async Task ShowUserAlert(string id) 
        {
            AppUser appUser = await _userManager.FindByIdAsync(id);
            await _hubContext.Clients.Client(appUser.ConnectionId).SendAsync("ShowAlert", appUser.Id);
        }
    }
}
