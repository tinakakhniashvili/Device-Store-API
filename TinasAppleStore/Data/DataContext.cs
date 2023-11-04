using Microsoft.EntityFrameworkCore;
using TinasAppleStore.Models;

namespace TinasAppleStore.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {    }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasKey(P => P.productId);
        }
    }
}
