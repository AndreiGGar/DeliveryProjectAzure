using DeliveryProjectAzure.Models;
using Microsoft.EntityFrameworkCore;

namespace DeliveryProjectAzure.Context
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryRestaurant> CategoriesRestaurants { get; set; }
        public DbSet<OpeningRestaurant> OpeningRestaurants { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<CategoryProduct> CategoriesProducts { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchasedProduct> PurchasedProducts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Wishlist> Wishlist { get; set; }
    }
}
