using ManageCoffee.Models;

namespace ManageCoffee.ViewModels
{
    public class CartItem
    {
        public int Quantity { set; get; }
        public Coffee Coffee { set; get; }
    }
}
