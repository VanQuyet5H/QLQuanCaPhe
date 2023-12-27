using ManageCoffee.Models;
using ManageCoffee.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ManageCoffee.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CoffeeShopContext _context;
        public HomeController(ILogger<HomeController> logger, CoffeeShopContext context)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewBag.SessionUser = HttpContext.Session.GetString("SessionUser");
            var skh = from o in _context.Order
                      select o.Customer.Id;
            var sod = from o in _context.Order
                      select o.Id;
            var dt = from o in _context.Order
                     join b in _context.OrderItem on o.Id equals b.Order.Id
                     select b.Price * b.Quantity;
            var ssp = from o in _context.OrderItem
                      select o.Quantity;
            ViewData["CountCustomer"] = skh.Count();
            ViewData["CountOrder"] = sod.Count();
            ViewData["DoanhThu"] = dt.Sum();
            ViewData["SoSp"] = ssp.Count();
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