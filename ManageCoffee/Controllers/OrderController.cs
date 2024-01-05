using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using ManageCoffee.Models;
using ManageCoffee.Other;
using ManageCoffee.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ManageCoffee.Controllers
{
    public class OrderController : Controller
    {
        private readonly CoffeeShopContext _context;
        public const string CARTKEY = "cart";
        protected readonly IMapper _mapper;
        private readonly INotyfService _notyfService;

        public OrderController(INotyfService notyfService, CoffeeShopContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _notyfService = notyfService;
        }
        public IActionResult Index()
        {
            return View();
        }
        List<CartItem> GetCartItems()
        {
            var session = HttpContext.Session;
            string? jsoncart = session.GetString(CARTKEY);
            if (jsoncart != null)
            {
                return JsonConvert.DeserializeObject<List<CartItem>>(jsoncart);
            }
            return new List<CartItem>();
        }

        void ClearCart()
        {
            var session = HttpContext.Session;
            session.Remove(CARTKEY);
        }

        void SaveCartSession(List<CartItem> ls)
        {
            var session = HttpContext.Session;
            string jsoncart = JsonConvert.SerializeObject(ls);
            session.SetString(CARTKEY, jsoncart);
        }

        [Route("addcart/{productid:int}", Name = "addcart")]
        public IActionResult AddToCart([FromRoute] int productid)
        {

            var product = _context.Coffee
                .Where(p => p.Id == productid)
                .FirstOrDefault();
            if (product == null)
                return NotFound("Không có sản phẩm");

            var cart = GetCartItems();
            var cartitem = cart.Find(p => p.Coffee.Id == productid);
            if (cartitem != null)
            {
                cartitem.Quantity++;
            }
            else
            {
                cart.Add(new CartItem() { Quantity = 1, Coffee = product });
            }

            SaveCartSession(cart);
            return RedirectToAction(nameof(Cart));
        }

        [Route("/cart", Name = "cart")]
        public IActionResult Cart()
        {
            return View(GetCartItems());
        }

        [Route("/updatecart", Name = "updatecart")]
        [HttpPost]
        public IActionResult UpdateCart([FromForm] int productid, [FromForm] int quantity)
        {
            var cart = GetCartItems();
            var cartitem = cart.Find(p => p.Coffee.Id == productid);
            if (cartitem != null)
            {
                cartitem.Quantity = quantity;
            }
            SaveCartSession(cart);
            return Ok();
        }

        [Route("/removecart/{productid:int}", Name = "removecart")]
        public IActionResult RemoveCart([FromRoute] int productid)
        {
            var cart = GetCartItems();
            var cartitem = cart.Find(p => p.Coffee.Id == productid);
            if (cartitem != null)
            {
                cart.Remove(cartitem);
            }

            SaveCartSession(cart);
            return RedirectToAction(nameof(Cart));
        }

        [Route("/checkout")]
        [Authorize]
        public IActionResult Checkout()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "IDcustomer");
            if (userIdClaim != null)
            {
                int IDcustomer = Convert.ToInt32(userIdClaim.Value);

                var cart = GetCartItems();
                var cartItems = cart.ToList();

                var orderDetail = new Order
                {
                    CustomerId = IDcustomer,
                    OrderDate = DateTime.Now,
                    Status = "Pending",
                };
                _context.Order.Add(orderDetail);
                _context.SaveChanges();

                foreach (var cartItem in cartItems)
                {
                    var orderitem = new OrderItem
                    {
                        OrderId = orderDetail.Id,
                        CoffeeId = cartItem.Coffee.Id,
                        Quantity = cartItem.Quantity,
                        Price = cartItem.Quantity * cartItem.Coffee.Price
                    };
                    _context.OrderItem.Add(orderitem);
                }
                _context.SaveChanges();
                ClearCart();

                return RedirectToAction("ThankYou", new { cartItems });
            }
            else
            {
                return RedirectToAction("DangNhap", "Account");
            }
        }

        public IActionResult ThankYou(List<CartItem> cartItems)
        {
            return View(cartItems);
        }

        [HttpGet]
        public async Task<IActionResult> ListOrderAdmin(string searchString, string currentFilter, int? pageNumber)
        {
            try
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

                var query = _context.Order
                    .Select(order => new OrderInfor
                    {
                        Id = order.Id,
                        CustomerId = order.CustomerId,
                        CustomerName = _context.Customer
                            .Where(c => c.IdUser == order.CustomerId)
                            .Select(c => c.Name)
                            .FirstOrDefault()!,
                        OrderDate = order.OrderDate,
                        Status = order.Status
                    });

                if (!String.IsNullOrEmpty(searchString))
                {
                    query = query.Where(s => s.Status.Contains(searchString));
                }

                int pageSize = 5;
                return View(await PaginatedList<OrderInfor>.CreateAsync(query.AsNoTracking(), pageNumber ?? 1, pageSize));
            }
            catch (Exception)
            {
                return View();
            }

        }

        public bool Delete(int id)
        {
            try
            {
                var order = _context.Order.Find(id);
                if (order == null)
                {
                    return false;
                }
                _context.Order.Remove(order);
                _context.SaveChanges();
                return true;

            }
            catch (Exception ex)
            {
                throw new Exception("Đã có lỗi xảy ra, vui lòng thử lại sau", ex);
            }
        }
        public IActionResult DeleteOrder(int id)
        {
            try
            {
                bool success = Delete(id);
                if (success)
                {
                    TempData["DeleteUserMessage"] = "Xoá thành công";
                }
                else
                {
                    TempData["DeleteUserMessage"] = "Xoá không thành công";
                }
                return RedirectToAction(nameof(ListOrderAdmin));
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Đã có lỗi xảy ra, vui lòng thử lại sau!");
                return View("Index");
            }
        }

        [HttpGet]
        public IActionResult DetailOrder(int id)
        {
            var orderDetail = _context.Order.Find(id);
            orderDetail = _context.Order
            .Include(o => o.OrderItem)
            .FirstOrDefault(o => o.Id == id);

            if (orderDetail != null)
            {
                var customerId = orderDetail.CustomerId;
                var customerName = GetCustomerNameById(customerId);

                ViewData["CustomerName"] = customerName;

                return View(orderDetail);
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult DetailOrder(int id, OrderInfor model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var orderDetail = _context.Order.Find(id);
            orderDetail = _context.Order
            .Include(o => o.OrderItem)
            .FirstOrDefault(o => o.Id == id);

            if (orderDetail != null)
            {
                orderDetail.Status = model.Status;

                _context.SaveChanges();

                return RedirectToAction("DetailOrder", new { id = orderDetail.Id });
            }

            return NotFound();
        }


        private string GetCustomerNameById(int? customerId)
        {
            if (customerId.HasValue)
            {
                var customerName = _context.Customer
                    .Where(c => c.IdUser == customerId)
                    .Select(c => c.Name)
                    .FirstOrDefault();

                return customerName ?? "N/A";
            }

            return "N/A";
        }

    }
}
