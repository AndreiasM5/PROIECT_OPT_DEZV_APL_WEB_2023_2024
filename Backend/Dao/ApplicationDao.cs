using Backend.Entity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Dao;

public class ApplicationDao : DbContext
{
    public ApplicationDao(DbContextOptions<ApplicationDao> options)
        : base(options)
    {
    }  

    public DbSet<Product> Products { get; set; }
}