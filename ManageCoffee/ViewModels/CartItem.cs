using ManageCoffee.Models;

namespace ManageCoffee.ViewModels
{
    public class CartItem
    {
        public int Quantity { get; set; }
        public Coffee Coffee { get; set; }
        public int CoffeeId { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
    }
}
