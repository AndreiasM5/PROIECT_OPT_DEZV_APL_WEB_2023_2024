using Backend.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace Backend.Dao;
public class ApplicationDao(DbContextOptions<ApplicationDao> options) : IdentityDbContext<User>(options) {
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Order>().HasMany(o => o.Products).WithMany();
        modelBuilder.Entity<Order>().HasOne(o => o.User).WithMany().HasForeignKey(o => o.UserId).IsRequired();
        modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(12, 2);
        modelBuilder.Entity<Order>().Property(o => o.TotalAmount).HasPrecision(12, 2);
    }
}
