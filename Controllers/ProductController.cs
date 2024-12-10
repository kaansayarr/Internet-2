using Internet_1.Models;
using Internet_1.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Internet_1.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductRepository _productRepository;

        public ProductController(ProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public IActionResult Index()


        {
            var userId = HttpContext.Session.GetString("UserId");
            var userRole = HttpContext.Session.GetString("Rol");

            Console.WriteLine("userId: " + userId);
            Console.WriteLine("userRole: " + userRole);

            ViewBag.IsUserLoggedIn = !string.IsNullOrEmpty(userId);
            ViewBag.IsUserAdmin = userRole == "Admin";


            var products = _productRepository.GetList();
            return View(products);
        }
        public IActionResult Add()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var userRole = HttpContext.Session.GetString("Rol");

            Console.WriteLine("userId: " + userId);
            Console.WriteLine("userRole: " + userRole);

            ViewBag.IsUserLoggedIn = !string.IsNullOrEmpty(userId);
            ViewBag.IsUserAdmin = userRole == "Admin";
            return View();
        }

        [HttpPost]
        public IActionResult Add(Product model)
        {

            var userId = HttpContext.Session.GetString("UserId");
            var userRole = HttpContext.Session.GetString("Rol");

            Console.WriteLine("userId: " + userId);
            Console.WriteLine("userRole: " + userRole);

            ViewBag.IsUserLoggedIn = !string.IsNullOrEmpty(userId);
            ViewBag.IsUserAdmin = userRole == "Admin";
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            _productRepository.Add(model);
            return RedirectToAction("Index");
        }
        public IActionResult Update(int id)
        {

            var userId = HttpContext.Session.GetString("UserId");
            var userRole = HttpContext.Session.GetString("Rol");

            Console.WriteLine("userId: " + userId);
            Console.WriteLine("userRole: " + userRole);

            ViewBag.IsUserLoggedIn = !string.IsNullOrEmpty(userId);
            ViewBag.IsUserAdmin = userRole == "Admin";
            var product = _productRepository.GetById(id);
            return View(product);
        }

        [HttpPost]
        public IActionResult Update(Product model)
        {

            var userId = HttpContext.Session.GetString("UserId");
            var userRole = HttpContext.Session.GetString("Rol");

            Console.WriteLine("userId: " + userId);
            Console.WriteLine("userRole: " + userRole);

            ViewBag.IsUserLoggedIn = !string.IsNullOrEmpty(userId);
            ViewBag.IsUserAdmin = userRole == "Admin";
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            _productRepository.Update(model);
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {

            var userId = HttpContext.Session.GetString("UserId");
            var userRole = HttpContext.Session.GetString("Rol");

            Console.WriteLine("userId: " + userId);
            Console.WriteLine("userRole: " + userRole);

            ViewBag.IsUserLoggedIn = !string.IsNullOrEmpty(userId);
            ViewBag.IsUserAdmin = userRole == "Admin";
            var product = _productRepository.GetById(id);
            return View(product);
        }

        [HttpPost]
        public IActionResult Delete(Product model)
        {

            var userId = HttpContext.Session.GetString("UserId");
            var userRole = HttpContext.Session.GetString("Rol");

            Console.WriteLine("userId: " + userId);
            Console.WriteLine("userRole: " + userRole);

            ViewBag.IsUserLoggedIn = !string.IsNullOrEmpty(userId);
            ViewBag.IsUserAdmin = userRole == "Admin";

            _productRepository.Delete(model.Id);
            return RedirectToAction("Index");
        }
    }
}
