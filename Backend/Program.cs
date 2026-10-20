using Backend.Dao;
using Backend.Entity;
using Backend.Service;
using Backend.ExceptionHandling;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var provider = builder.Configuration["DatabaseProvider"] ?? "Sqlite";
if (provider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
    Directory.CreateDirectory(Path.Combine(builder.Environment.ContentRootPath, "data"));
builder.Services.AddDbContext<ApplicationDao>(options => {
    var connection = builder.Configuration.GetConnectionString("DefaultConnection");
    if (provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase)) options.UseSqlServer(connection);
    else if (provider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase)) options.UseSqlite(connection);
    else throw new InvalidOperationException("DatabaseProvider must be Sqlite or SqlServer.");
});
builder.Services.AddTransient<ErrorHandlerMiddleware>();
builder.Services.AddScoped<ProductService, ProductServiceImpl>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddCors(options => options.AddPolicy("frontend", policy =>
    policy.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [])
        .AllowAnyHeader().AllowAnyMethod().WithExposedHeaders("Jwt")));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddIdentityCore<User>(options => options.User.RequireUniqueEmail = true)
    .AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDao>().AddDefaultTokenProviders();
var secret = builder.Configuration["JWT:Secret"];
if (string.IsNullOrWhiteSpace(secret)) {
    if (!builder.Environment.IsDevelopment()) throw new InvalidOperationException("Set JWT__Secret outside Development.");
    secret = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    builder.Configuration["JWT:Secret"] = secret;
}
if (Encoding.UTF8.GetByteCount(secret) < 32) throw new InvalidOperationException("JWT__Secret must contain at least 32 bytes.");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = true, ValidateAudience = true, ValidateLifetime = true, ValidateIssuerSigningKey = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"], ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)), ClockSkew = TimeSpan.FromSeconds(15)
    };
});
builder.Services.AddAuthorization();
var app = builder.Build();
if (app.Environment.IsDevelopment()) {
    using var scope = app.Services.CreateScope();
    await DatabaseSetup.InitializeAsync(scope.ServiceProvider);
    app.UseSwagger(); app.UseSwaggerUI();
}
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseCors("frontend");
app.UseAuthentication(); app.UseAuthorization();
app.MapControllers();
app.MapGet("/api/health", () => Results.Ok(new { status = "ok" })).AllowAnonymous();
app.Run();
