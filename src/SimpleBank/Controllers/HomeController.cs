using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleBank.DataAccess.Models;
using SimpleBank.DataAccess.Models;
using System.Diagnostics;

namespace SimpleBank.Controllers
{
    [Authorize]

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["FirstName"] = user?.FirstName ?? "User";
            return View();
           
        }

        public IActionResult Privacy()
        {
            return View();
        }
        // GET: /Home/About
        public ActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [AllowAnonymous]
        public IActionResult NotFound()
        {
            return View();
        }

        [Authorize(Roles ="Admin")]
        public IActionResult VeryImportant()
        {
            return View();
        }
    }
}
