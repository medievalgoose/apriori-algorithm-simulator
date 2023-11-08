using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ShopApp.Models;

namespace ShopApp.Data
{
    public class ShopAppDbContext : DbContext
    {
        public ShopAppDbContext(DbContextOptions<ShopAppDbContext> options) : base (options)
        {
            
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Sales> Sales { get; set; }

        public DbSet<SalesDetail> SalesDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<SalesDetail>()
                .HasKey( sd => new { sd.SalesId, sd.ProductId });
        }
    }
}
