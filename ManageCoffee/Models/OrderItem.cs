namespace ManageCoffee.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int CoffeeId { get; set; }
        public Order Order { get; set; }
        public Coffee Coffee { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Unit { get; set; } = string.Empty;
    }
}
