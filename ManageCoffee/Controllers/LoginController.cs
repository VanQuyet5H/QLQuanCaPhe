using ManageCoffee.Models;
using ManageCoffee.Other;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ManageCoffee.Controllers
{
	public class LoginController : Controller
	{
		private readonly CoffeeShopContext _context;
       
        public LoginController(CoffeeShopContext context)
		{
			_context = context;

		}
		[HttpGet]
		public IActionResult DangNhap()
		{
			return View();
		}
		[HttpPost]
		public IActionResult DangNhap(string username, string password)
		{
			var user = _context.User.FirstOrDefault(u => u.Username == username);


			// If the user is found and the password matches
			if (user != null && user.Password == password)
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
				TempData["Message"] = "Đăng nhập thành công";
				// Redirect to the home page
				return RedirectToAction("Index", "Home");
			}
			return RedirectToAction("Login");
		}
		public IActionResult Logout()
		{
			// Clear the user from the session
			HttpContext.Session.Remove("User");

			// Redirect to the login page
			return RedirectToAction("Login","DangNhap");
		}
		[HttpGet]
		public IActionResult DangKy()
		{
			return View();
		}
		[HttpPost]
		public IActionResult DangKy([FromBody]User user)
		{

			// Validate user data
			if (ModelState.IsValid)
			{
				//kiểm tra username đã tồn tại
				if (_context.User.Any(u => u.Username == user.Username))
				{
					// Username already exists
					ModelState.AddModelError("Username", "Username đã tồn tại");
					return View("DangKy", user);
				}
				// Create new user
				_context.User.Add(user);
				_context.SaveChanges();

				// Login user after registration
				HttpContext.Session.SetString("User", user.Username);
                //Thong bao
                TempData["Message"] = "Đăng ký thành công";
                return RedirectToAction("Login", "Login");
			}
            TempData["Message"] = "Đăng ký thất bại";
            // Return error if validation fails
            return View("DangKy", user);
		}
		public async Task<IActionResult> DanhSachAccount(string searchString, string currentFilter, int? pageNumber)
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
            var sqlServerDbContext = from s in _context.User
                                     select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                sqlServerDbContext = sqlServerDbContext.Where(s => s.Username.Contains(searchString)
                                       || s.Email.Contains(searchString));
            }
            int pageSize = 3;
            return View(await PaginatedList<User>.CreateAsync(sqlServerDbContext.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
		[HttpPost]
		public IActionResult CapNhatAccount(int? id,[FromBody]User user)
		{
			//Kiem tra id
			var accountUser = _context.User.Find(id);
            accountUser.Username = user.Username;
            accountUser.Email = user.Email;
			accountUser.Password = user.Password;
			accountUser.Role = user.Role;
            // Lưu các thay đổi vào database
            _context.SaveChanges();
            return View();
		}
        public ActionResult Delete(int id)
        {
            // Lấy đối tượng account user từ view
            var accountUser = _context.User.Find(id);

            // Xóa đối tượng account user khỏi database
            _context.User.Remove(accountUser);

            // Lưu các thay đổi vào database
            _context.SaveChanges();

            return RedirectToAction("DanhSachAccount");
        }
    }
}
