using AspNetCoreHero.ToastNotification.Abstractions;
using DocumentFormat.OpenXml.Spreadsheet;
using ManageCoffee.Models;
using ManageCoffee.Other;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ManageCoffee.Controllers
{
    public class LoginController : Controller
    {
        private readonly CoffeeShopContext _context;
        private readonly INotyfService _notyfService;
        public LoginController(CoffeeShopContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
        [HttpGet]
        public IActionResult DangNhap()
        {
            return View();
        }
        [HttpPost]
        public IActionResult DangNhap(string username, string password)
        {
            if (ModelState.IsValid)
            {
                var f_password = GetMD5(password);
                var user = _context.User.FirstOrDefault(u => u.Username == username);
                //var data = _context.User.Where(s => s.Username.Equals(username) && s.Password.Equals(f_password)).ToList();
                if (user != null && user.Password == f_password)
                {
                    // Set the user as logged in
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name,user.Username),
                        new Claim(ClaimTypes.Role, user.Role),
                    };
                    HttpContext.Session.SetString("SessionUser", user?.Username ?? "");
                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity)
                      );
                    //Thông báo
                    _notyfService.Success("Đăng nhập thành công");
                    // Redirect to the home page
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    _notyfService.Error("Vui lòng kiểm tra lại thông tin");
                }
            }
            return RedirectToAction("DangNhap");
        }
        public IActionResult Logout()
        {
            // Clear the user from the session
            HttpContext.Session.Remove("User");

            // Redirect to the login page
            return RedirectToAction("DangNhap", "Login");
        }
        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DangKy([Bind("Username,Password,Email,Role")] User user)
        {

            // Validate user data
            if (ModelState.IsValid)
            {
                var check = _context.User.FirstOrDefault(s => s.Username == user.Username);
                //kiểm tra username đã tồn tại
                if (check == null)
                {
                    user.Password = GetMD5(user.Password);
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    HttpContext.Session.SetString("User", user.Username);
                    //Thong bao
                    _notyfService.Success("Đăng ký thành công");
                    return RedirectToAction("DangNhap", "Login");
                }
                else
                {
                    _notyfService.Error("Đăng ký thất bại");
                }


            }
            return View("DangKy", user);
        }
        public async Task<IActionResult> DanhSachAccount(string searchString, string currentFilter, int? pageNumber)
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
            var sqlServerDbContext = from s in _context.User
                                     select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                sqlServerDbContext = sqlServerDbContext.Where(s => s.Username.Contains(searchString)
                                       || s.Email.Contains(searchString));
            }
            else
            {
                _notyfService.Warning("Không tìm thấy tài khoản");
            }
            int pageSize = 3;
            return View(await PaginatedList<User>.CreateAsync(sqlServerDbContext.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
        [HttpGet]
        public async Task<IActionResult> CapNhatAccount(int? id)
        {
            ViewBag.SessionUser = HttpContext.Session.GetString("SessionUser");
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> CapNhatAccount(int? id, [Bind("Id", "Username,Password,Email,Role")] User user)
        {
            ViewBag.SessionUser = HttpContext.Session.GetString("SessionUser");
            if (id != user.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    _notyfService.Success("Bạn đã cập nhật thông tin account user thành công.");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(DanhSachAccount));
            }
            return View(user);
        }
        private bool UserExists(int id)
        {

            return _context.User.Any(e => e.Id == id);
        }
        public ActionResult Delete(int id)
        {
            ViewBag.SessionUser = HttpContext.Session.GetString("SessionUser");
            // Lấy đối tượng account user từ view
            var accountUser = _context.User.Find(id);

            // Xóa đối tượng account user khỏi database
            _context.User.Remove(accountUser);

            // Lưu các thay đổi vào database
            _context.SaveChanges();
            _notyfService.Success("Bạn đã xóa thông tin account user thành công.");
            return RedirectToAction("DanhSachAccount");
        }
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }
    }
}
