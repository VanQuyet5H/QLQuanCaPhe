using ManageCoffee.Models;

namespace ManageCoffee.ViewModels
{
    public class OrderInfor
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
        public DateTime OrderDate { get; set; }
        public string QrCode { get; set; } = string.Empty;
        public string Status { get; set; }

        public List<OrderItem> OrderItems { get; set; }
    }
}
