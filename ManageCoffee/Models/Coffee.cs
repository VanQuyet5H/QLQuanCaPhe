using System.ComponentModel.DataAnnotations;

namespace ManageCoffee.Models
{
    public class Coffee
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Tên sản phẩm coffee không được bỏ trống"), StringLength(50)]
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
        [DataType(DataType.Upload)]
        public string? Image { get; set; }
        [Required(ErrorMessage = "Giá không được bỏ trống"), Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Số lượng không được bỏ trống"), Range(0, int.MaxValue)]
        public int Quantity { get; set; }
        public bool InStock { get; set; }
    }
}
