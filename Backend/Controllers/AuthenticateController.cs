using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Entity;
using Backend.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
namespace Backend.Controllers;
[ApiController, Route("api")]
public class AuthenticateController(UserManager<User> users, IConfiguration configuration) : ControllerBase {
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginModel model) {
        var user = await users.FindByEmailAsync(model.Email!);
        if (user is null || !await users.CheckPasswordAsync(user, model.Password!)) return Unauthorized(new {message = "Invalid email or password."});
        var roles = await users.GetRolesAsync(user);
        var claims = new List<Claim> {new(ClaimTypes.NameIdentifier, user.Id), new(ClaimTypes.Name, user.UserName!), new(JwtRegisteredClaimNames.Sub, user.Id), new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())};
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        var jwt = new JwtSecurityToken(configuration["JWT:ValidIssuer"], configuration["JWT:ValidAudience"], claims, expires: DateTime.UtcNow.AddHours(3),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]!)), SecurityAlgorithms.HmacSha256));
        var token = new JwtSecurityTokenHandler().WriteToken(jwt);
        Response.Headers["Jwt"] = token;
        return Ok(new {token, expiration = jwt.ValidTo, email = user.Email, roles});
    }
    [HttpPost("register")]
    public Task<IActionResult> Register(RegisterModel model) => Create(model, "User");
    [Authorize(Roles = "Admin"), HttpPost("register-admin")]
    public Task<IActionResult> RegisterAdmin(RegisterModel model) => Create(model, "Admin");
    private async Task<IActionResult> Create(RegisterModel model, string role) {
        if (await users.FindByEmailAsync(model.Email!) is not null) return Conflict(new {message = "An account with this email already exists."});
        var user = new User {Email = model.Email, UserName = model.Email};
        var result = await users.CreateAsync(user, model.Password!);
        if (!result.Succeeded) return BadRequest(new {message = string.Join("; ", result.Errors.Select(e => e.Description))});
        var roleResult = await users.AddToRoleAsync(user, role);
        if (!roleResult.Succeeded) {await users.DeleteAsync(user); return Problem("Account role could not be assigned.");}
        return StatusCode(201, new {message = "Account created. You can sign in."});
    }
}
