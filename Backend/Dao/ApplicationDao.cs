using Backend.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend.Dao;

public class ApplicationDao : IdentityDbContext<User>
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
        .HasOne(o => o.User)
        .WithMany()
        .HasForeignKey(o => o.UserId)
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

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = "1",
                UserName = "admin@example.com",
                NormalizedUserName = "ADMIN@EXAMPLE.COM",
                Email = "admin@example.com",
                NormalizedEmail = "ADMIN@EXAMPLE.COM",
                EmailConfirmed = true,
                FirstName = "Admin",
                LastName = "User",
                Street = "123 Main St",
                City = "City1",
                Phone = "555-1234",
                SecurityStamp = Guid.NewGuid().ToString(),
                PasswordHash = GetPasswordHash("customerPassword123") // Set password hash directly
            },
            new User
            {
                Id = "2",
                UserName = "customer@example.com",
                NormalizedUserName = "CUSTOMER@EXAMPLE.COM",
                Email = "customer@example.com",
                NormalizedEmail = "CUSTOMER@EXAMPLE.COM",
                EmailConfirmed = true,
                FirstName = "Customer",
                LastName = "User",
                Street = "456 Oak St",
                City = "City2",
                Phone = "555-5678",
                SecurityStamp = Guid.NewGuid().ToString(),
                PasswordHash = GetPasswordHash("customerPassword123") // Set password hash directly
            }
        );

        // // Assign roles
        // modelBuilder.Entity<IdentityUserRole<string>>().HasData(
        //     new IdentityUserRole<string> { UserId = "1", RoleId = "1" },
        //     new IdentityUserRole<string> { UserId = "2", RoleId = "2" } 
        // );

        modelBuilder.Entity<Order>().HasData(
            new Order
            {
                OrderId = 1,
                TotalAmount = 20.5m, 
                UserId = "1"
            },
            new Order
            {
                OrderId = 2,
                TotalAmount = 15.75m,
                UserId = "1"
            },
            new Order
            {
                OrderId = 3,
                TotalAmount = 30.0m,
                UserId = "2"
            }
        );
        // Many-to-many relationship: Add products to orders
        modelBuilder.Entity("OrderProduct").HasData(
            new { OrderId = 1, ProductsProductId = 1 },
            new { OrderId = 1, ProductsProductId = 2 },
            new { OrderId = 2, ProductsProductId = 2 },
            new { OrderId = 3, ProductsProductId = 3 }
        );
    }

    private string GetPasswordHash(string password)
    {
        var passwordHasher = new PasswordHasher<User>();
        return passwordHasher.HashPassword(null, password);
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
}