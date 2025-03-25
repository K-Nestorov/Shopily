using Microsoft.EntityFrameworkCore;
using Shopily.Domain.Entity;

namespace Shopily.Data
{
    public class Context : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<NewsArticle>NewsArticles { get; set; }
     //   public DbSet<NewsSite> NewsSites { get; set; }

        public Context(DbContextOptions<Context> options) : base(options)
        {
            Users = Set<User>();
            Products = Set<Product>();
            Carts = Set<Cart>();
            Orders = Set<Order>();
            NewsArticles = Set<NewsArticle>();
         //   NewsSites = Set<NewsSite>();
        }

        public Context()
        {
        }
    }
}
