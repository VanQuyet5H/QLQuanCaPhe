using AutoMapper;
using ManageCoffee.Models;
using ManageCoffee.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;
using System.Security.Claims;

namespace ManageCoffee.Controllers
{
    public class OrderController : Controller
    {
        private readonly CoffeeShopContext _context;
        public const string CARTKEY = "cart";
        protected readonly IMapper _mapper;

        public OrderController(CoffeeShopContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            return View();
        }
        List<CartItem> GetCartItems()
        {
            var session = HttpContext.Session;
            string jsoncart = session.GetString(CARTKEY);
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
        public IActionResult Checkout(Order orders)
        {
            var cart = GetCartItems();           
            var cartItems = cart.ToList();

            foreach (var cartItem in cartItems)
            {
                var orderDetail = new Order
                {
                    CoffeeId = cartItem.Coffee.Id,
                    CustomerId = 1,
                    OrderDate = DateTime.Now,
                    Status = "Pending",
                };
                var orderCart = _mapper.Map<Order>(orderDetail);
                _context.Order.Add(orderCart);
            }

            _context.SaveChanges();
            ClearCart();

            return RedirectToAction("ThankYou", new { cartItems });
        }
        public ActionResult ThankYou(List<CartItem> cartItems)
        {
            return View(cartItems);
        }

    }
}
