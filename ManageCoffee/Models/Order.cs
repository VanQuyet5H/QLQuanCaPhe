namespace ManageCoffee.Models
{
    public class Order
    {
        public int Id { get; set; }
        public virtual Coffee Coffee { get; set; }
        public virtual Customer Customer { get; set; }
        public DateTime OrderDate { get; set; }
        public string QrCode { get; set; }
        public string Status { get; set; }

    }
}
