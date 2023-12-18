using ManageCoffee.Models;
using Microsoft.AspNetCore.Mvc;

namespace ManageCoffee.Controllers
{
    public class OrderController : Controller
    {
        private readonly CoffeeShopContext _context;

        public OrderController(CoffeeShopContext context)
        {
            _context = context;

        }
        public IActionResult Index()
        {
            return View();
        }
        //Tạo order
        //Danh sách order
        //Hóa đơn
        //Thanh Toán
        //Lịch sử order

    }
}
