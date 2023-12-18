using Microsoft.EntityFrameworkCore;

namespace ManageCoffee.Models
{
    public class CoffeeShopContext : DbContext
    {
        public CoffeeShopContext(DbContextOptions options) : base(options) { }
        public DbSet<Coffee> Coffee { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderItem> OrderItem { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Customer>Customer{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


        }
    }
}
