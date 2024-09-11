using Abocar.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Abocar.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Message> Messages { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Wishlist> Wishlist { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<Review> Review { get; set; }
        public DbSet<DeliveryOption> DeliveryOption { get; set; }
        public DbSet<PaymentOption> PaymentOption { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<LocalPickUp> LocalPickUp { get; set; }
        public DbSet<Brand> Brand { get; set; }

    }
}
