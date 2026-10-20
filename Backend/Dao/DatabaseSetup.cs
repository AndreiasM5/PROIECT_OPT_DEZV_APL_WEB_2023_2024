using Backend.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace Backend.Dao;
public static class DatabaseSetup {
    public static async Task InitializeAsync(IServiceProvider services) {
        var db = services.GetRequiredService<ApplicationDao>();
        await db.Database.EnsureCreatedAsync();
        var roles = services.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var role in new[] {"Admin", "User"})
            if (!await roles.RoleExistsAsync(role)) Ensure(await roles.CreateAsync(new IdentityRole(role)));
        var users = services.GetRequiredService<UserManager<User>>();
        foreach (var entry in new[] {("admin@example.com", "DemoAdmin123!", "Admin"), ("customer@example.com", "DemoCustomer123!", "User")}) {
            var user = await users.FindByEmailAsync(entry.Item1);
            if (user is null) {
                user = new User {UserName = entry.Item1, Email = entry.Item1, EmailConfirmed = true};
                Ensure(await users.CreateAsync(user, entry.Item2));
            }
            if (!await users.IsInRoleAsync(user, entry.Item3)) Ensure(await users.AddToRoleAsync(user, entry.Item3));
        }
        if (!await db.Products.AnyAsync()) {
            db.Products.AddRange(new Product {Name = "Water", Price = 2}, new Product {Name = "Jack Daniels", Price = 10}, new Product {Name = "Pepsi", Price = 4});
            await db.SaveChangesAsync();
        }
    }
    private static void Ensure(IdentityResult result) {
        if (!result.Succeeded) throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));
    }
}
