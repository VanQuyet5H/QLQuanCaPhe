using ManageCoffee.Models;
using ManageCoffee.Other;
using ManageCoffee.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Globalization;
using System.Linq;

namespace ManageCoffee.Controllers
{
    public class CoffeeController : Controller
    {
        private readonly CoffeeShopContext _context;

        public CoffeeController(CoffeeShopContext context)
        {
            _context = context;

        }

        [HttpGet]
        public async Task<IActionResult> DanhSachCoffee(string searchString, string currentFilter, int? pageNumber)
        {
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;
            var sqlServerDbContext = from s in _context.Coffee
                                     select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                sqlServerDbContext = sqlServerDbContext.Where(s => s.Name.Contains(searchString));
            }

            int pageSize = 3;
            return View(await PaginatedList<Coffee>.CreateAsync(sqlServerDbContext.AsNoTracking(), pageNumber ?? 1, pageSize));

        }
        [HttpGet]
        public IActionResult ThemCoffee()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ThemCoffee([FromBody] Coffee coffee)
        {
            if (ModelState.IsValid)
            {
                _context.Coffee.Add(coffee);
                _context.SaveChanges();
            }
            return RedirectToAction("DanhSachCoffee", "Coffee");
        }
       
    }
}
