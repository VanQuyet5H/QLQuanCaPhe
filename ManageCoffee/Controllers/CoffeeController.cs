using AspNetCoreHero.ToastNotification.Abstractions;
using ManageCoffee.Models;
using ManageCoffee.Other;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace ManageCoffee.Controllers
{
    public class CoffeeController : Controller
    {
        private readonly CoffeeShopContext _context;
        private readonly INotyfService _notyfService;
        public CoffeeController(INotyfService notyfService, CoffeeShopContext context)
        {
            _context = context;
            _notyfService = notyfService;
        }
        //Hiển thị danh sách coffee
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
        public async Task<IActionResult> ThemCoffee([Bind("Id","Name", "Description", "Type", "Image", "Price", "Quantity", "InStock")]Coffee coffee, IFormFile formFile)
        {
            if (ModelState.IsValid)
            {
                string filename = formFile.FileName;
                coffee.Image = filename.ToString(); // tên ảnh
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/img", filename);
                formFile.CopyTo(new FileStream(imagePath, FileMode.Create));
                _context.Add(coffee);
                await _context.SaveChangesAsync();
                _notyfService.Success("Bạn đã thêm thông tin của sản phẩm thành công.");
                return RedirectToAction(nameof(DanhSachCoffee));
            }
            else
            {
                _notyfService.Error("Vui lòng nhập lại thông tin của sản phẩm");
            }
            return View(coffee);
        }
        public async Task<IActionResult> CapNhatCoffee(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coffee = await _context.Coffee.FindAsync(id);
            if (coffee == null)
            {
                return NotFound();
            }
            return View(coffee);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatCoffee(int id, [Bind("Id,Name,Description,Type,Image,Price,Quantity,InStock")]Coffee coffee)
        {
            if (id != coffee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(coffee);
                    await _context.SaveChangesAsync();
                    _notyfService.Success("Bạn đã cập nhật thông tin coffee thành công.");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CoffeeExists(coffee.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(DanhSachCoffee));
            }

            
            return View(coffee);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coffee = await _context.Coffee
                .FirstOrDefaultAsync(m => m.Id == id);
            if (coffee == null)
            {
                return NotFound();
            }

            return View(coffee);
        }

        // POST: NhanVien/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            

            var coffee = await _context.Coffee.FindAsync(id);
            _context.Coffee.Remove(coffee);
            await _context.SaveChangesAsync();
            _notyfService.Success("Bạn đã xóa thông tin của nhân viên thành công.");
            return RedirectToAction(nameof(DanhSachCoffee));
        }
        private bool CoffeeExists(int id)
        {
            return _context.Coffee.Any(e => e.Id == id);
        }
    }
}
