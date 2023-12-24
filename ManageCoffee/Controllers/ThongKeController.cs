using ManageCoffee.Models;
using ManageCoffee.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ManageCoffee.Controllers
{
    public class ThongKeController : Controller
    {
        private readonly CoffeeShopContext _context;

        public ThongKeController(CoffeeShopContext context)
        {
            _context = context;

        }
        //Thông kê coffee đã bán theo khoảng thời gian
        public IActionResult ThongKe(Tg tg)
        {
            var ds = from a in _context.Order
                     join b in _context.OrderItem on a.Id equals b.Order.Id
                     join c in _context.Coffee on a.Coffee.Id equals c.Id
                     join d in _context.Customer on a.Customer.Id equals d.Id
                     where a.OrderDate<=tg.DenNgay && a.OrderDate>=tg.TuNgay  
                     select new
                     {
						
					 };




			return View();
        }
		public IActionResult XuatExcel()
		{
			return View();
		}



	}
}
