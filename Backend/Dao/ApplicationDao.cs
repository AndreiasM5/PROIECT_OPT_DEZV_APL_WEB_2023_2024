using Backend.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend.Dao;

public class ApplicationDao : IdentityDbContext<IdentityUser>
{
    public ApplicationDao(DbContextOptions<ApplicationDao> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {   
        //Identity config
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>()
        .HasMany(o => o.Products)
        .WithMany();

        modelBuilder.Entity<Order>()
        .HasOne(o => o.Customer)
        .WithMany()
        .HasForeignKey(o => o.CustomerId)
        .IsRequired();

        SeedData(modelBuilder);
    }
      

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Add initial data to be seeded
        modelBuilder.Entity<Product>().HasData(
            new Product{ProductId = 1, Name = "Water", Price = 2},
            new Product{ProductId = 2, Name = "Jack Daniels", Price = 10},
            new Product{ProductId = 3, Name = "Pepsi", Price = 4}
        );
    }

    public DbSet<Product> Products { get; set; }
}