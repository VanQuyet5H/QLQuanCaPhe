using ManageCoffee.Models;

namespace ManageCoffee.ViewModels
{
    public class CoffeeViewModel : ViewModelBase
    {
        public IList<Coffee> Coffees { get; set; }
        public string SearchTerm { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
    }
}
