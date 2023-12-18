using ManageCoffee.Models;
using Microsoft.AspNetCore.Mvc;

namespace ManageCoffee.Controllers
{
    public class ThongKeController : Controller
    {
        private readonly CoffeeShopContext _context;

        public ThongKeController(CoffeeShopContext context)
        {
            _context = context;

        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
