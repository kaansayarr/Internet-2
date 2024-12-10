using Internet_1.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Internet_1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
           
            var userId = HttpContext.Session.GetString("UserId");
            var userRole = HttpContext.Session.GetString("Rol"); 

            Console.WriteLine("userId: " + userId); 
            Console.WriteLine("userRole: " + userRole); 

            ViewBag.IsUserLoggedIn = !string.IsNullOrEmpty(userId);
            ViewBag.IsUserAdmin = userRole == "Admin";

            return View();
        }




        public IActionResult TestWithLayout()
        {
            return View();
        }

        public IActionResult TestWithOutLayout()
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
    }
}