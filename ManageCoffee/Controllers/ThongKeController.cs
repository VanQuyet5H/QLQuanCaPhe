using ManageCoffee.Models;
using ManageCoffee.ViewModels;
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
		public IActionResult BestSellingCoffees()
		{
            ViewBag.SessionUser = HttpContext.Session.GetString("SessionUser");
            DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
			DateTime endDate = startDate.AddMonths(1).AddDays(-1);

			var topSellingProducts = _context.OrderItem
				.Where(a => a.Order.OrderDate >= startDate && a.Order.OrderDate <= endDate)
				.GroupBy(a => a.Coffee.Name)
				.Select(g => new
				{
					CoffeeName = g.Key,
					TotalQuantity = g.Sum(a => a.Quantity)
				})
				.OrderByDescending(result => result.TotalQuantity)
				.Take(10)
				.ToList();

			var productDetails = _context.OrderItem
				.Where(a => a.Order.OrderDate >= startDate && a.Order.OrderDate <= endDate)
				.Select(a => new
				{
					CoffeeName = a.Coffee.Name,
					Quantity = a.Quantity,
					OrderDate = a.Order.OrderDate
				})
				.ToList();
			// Truyền dữ liệu vào ViewBag để sử dụng trong View
			//ViewBag.TopSellingProducts = topSellingProducts;
			ViewBag.ProductDetails = productDetails;
			var chartData = new SalesChartViewModel
			{
				ChartLabels = topSellingProducts.Select(item => item.CoffeeName.ToString()).ToList(),
				ChartData = topSellingProducts.Select(item => item.TotalQuantity).ToList()
			};
			//ViewBag.TopSellingProducts = chartData;
			return View(chartData);

		}
	}
}
