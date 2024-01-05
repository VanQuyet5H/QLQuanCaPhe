namespace ManageCoffee.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public DateTime OrderDate { get; set; }
        public string QrCode { get; set; } = string.Empty;
        public string Status { get; set; }
        public List<OrderItem> OrderItem { get; set; }
    }
}
