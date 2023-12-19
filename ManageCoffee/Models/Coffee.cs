namespace ManageCoffee.Models
{
    public class Coffee
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
        public string? Image { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public bool InStock { get; set; }
    }
}
