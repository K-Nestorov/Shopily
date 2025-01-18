using Microsoft.EntityFrameworkCore;
using Shopily.Entity;

namespace Shopily.Repositories
{
    public class Context : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }

        // Constructor that accepts DbContextOptions<Context> and passes it to the base constructor
        public Context(DbContextOptions<Context> options) : base(options)
        {
            this.Users = this.Set<User>();
            this.Products = this.Set<Product>();
            this.Carts = this.Set<Cart>();
            this.Orders = this.Set<Order>();
        }

       
    }
}
