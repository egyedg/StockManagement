using Microsoft.EntityFrameworkCore;
using StockManagement.Infrastructure.Models;

namespace StockManagement.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationships
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

            modelBuilder.Entity<StockMovement>()
                .HasOne(sm => sm.Product)
                .WithMany(p => p.StockMovements)
                .HasForeignKey(sm => sm.ProductId);

            modelBuilder.Entity<StockMovement>()
                .HasOne(sm => sm.Order)
                .WithMany(o => o.StockMovements)
                .HasForeignKey(sm => sm.OrderId)
                .IsRequired(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}