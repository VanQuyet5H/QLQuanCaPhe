using AspNetCoreHero.ToastNotification.Abstractions;
using ClosedXML.Excel;
using ManageCoffee.Models;
using ManageCoffee.Other;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

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
            ViewBag.SessionUser = HttpContext.Session.GetString("SessionUser");
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
            else
            {
                _notyfService.Warning("Không tìm thấy sản phẩm coffee");
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
        public async Task<IActionResult> ThemCoffee([Bind("Name,Description,Type,Image,Price,Quantity,InStock")] Coffee coffee, IFormFile formFile)
        {
            ViewBag.SessionUser = HttpContext.Session.GetString("SessionUser");
            if (ModelState.IsValid)
            {

                string filename = formFile.FileName;
                coffee.Image = filename.ToString(); // tên ảnh
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/img", filename);
                formFile.CopyTo(new FileStream(imagePath, FileMode.Create));
                _context.Add(coffee);
                await _context.SaveChangesAsync();
                _notyfService.Success("Bạn đã thêm thông tin của sản phẩm thành công.");
                return RedirectToAction(nameof(DanhSachCoffeeAdmin));
            }
            else
            {
                _notyfService.Error("Vui lòng nhập lại thông tin của sản phẩm");
            }
            return View(coffee);
        }
        public async Task<IActionResult> CapNhatCoffee(int? id)
        {
            ViewBag.SessionUser = HttpContext.Session.GetString("SessionUser");
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
        public async Task<IActionResult> CapNhatCoffee(int id, [Bind("Id,Name,Description,Type,Image,Price,Quantity,InStock")] Coffee coffee, IFormFile formFile)
        {
            ViewBag.SessionUser = HttpContext.Session.GetString("SessionUser");
            if (id != coffee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string filename = formFile.FileName;
                    coffee.Image = filename.ToString(); // tên ảnh
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/img", filename);
                    formFile.CopyTo(new FileStream(imagePath, FileMode.Create));
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
                return RedirectToAction(nameof(DanhSachCoffeeAdmin));
            }


            return View(coffee);
        }

        public ActionResult Delete(int id)
        {
            ViewBag.SessionUser = HttpContext.Session.GetString("SessionUser");
            // Lấy đối tượng account user từ view
            var coffee = _context.Coffee.Find(id);

            // Xóa đối tượng account user khỏi database
            _context.Coffee.Remove(coffee);

            // Lưu các thay đổi vào database
            _context.SaveChanges();
            _notyfService.Success("Bạn đã xóa thông tin sản phẩm thành công.");
            return RedirectToAction("DanhSachCoffeeAdmin");
        }
        private bool CoffeeExists(int id)
        {
            return _context.Coffee.Any(e => e.Id == id);
        }
        [HttpGet]
        public async Task<IActionResult> DanhSachCoffeeAdmin(string searchString, string currentFilter, int? pageNumber)
        {
            ViewBag.SessionUser = HttpContext.Session.GetString("SessionUser");
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
            ViewData["coffee"] = (from s in _context.Coffee
                                  select s.Id).Count();
            if (!String.IsNullOrEmpty(searchString))
            {
                sqlServerDbContext = sqlServerDbContext.Where(s => s.Name.Contains(searchString));
            }
            else
            {
                _notyfService.Warning("Không tìm thấy sản phẩm coffee");
            }

            int pageSize = 3;
            return View(await PaginatedList<Coffee>.CreateAsync(sqlServerDbContext.AsNoTracking(), pageNumber ?? 1, pageSize));

        }
        public async Task<IEnumerable<Coffee>> GetCoffeesAsync()
        {

            // Tạo một truy vấn để lấy dữ liệu
            var query = _context.Coffee;

            // Chạy truy vấn một cách bất đồng bộ
            var coffees = await query.ToListAsync();

            // Trả về danh sách các đối tượng `Coffee`
            return coffees;
        }
        [HttpPost]
        [Route("export-excel")]
        public FileResult XuatExcelCoffee()
        {
            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[8] { new DataColumn("STT"),
                                                     new DataColumn("Tên Coffee"),
                                                      new DataColumn("Hình Ảnh"),
                                                     new DataColumn("Giá"),
                                                     new DataColumn("Số Lượng"),
                                                     new DataColumn("Loại"),
                                                      new DataColumn("Trạng Thái"),
                                                     new DataColumn("Thành Tiền")

                                                    });

            var insuranceCertificate = from InsuranceCertificate in _context.Coffee select InsuranceCertificate;

           
            foreach (var insurance in insuranceCertificate)
            {
                dt.Rows.Add(insurance.Id, insurance.Name, insurance.Image, insurance.Price, insurance.Quantity,
                     insurance.Type,insurance.InStock ? "Còn hàng" : "Hết hàng", insurance.Quantity * insurance.Price);
            }

            using (XLWorkbook wb = new XLWorkbook()) //Install ClosedXml from Nuget for XLWorkbook  
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream()) //using System.IO;  
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ExcelFile.xlsx");
                }
            }

        }



    }
}
