using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FirstApi.Data;
using FirstApi.Models;

namespace FirstApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    // 🔐 LOGIN
    [HttpPost("login")]
    public IActionResult Login(LoginModel model)
    {
        // 🔍 Check user from DB
        var user = _context.Users
            .FirstOrDefault(x =>
                x.Username == model.Username &&
                x.Password == model.Password);

        if (user == null)
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        // 🔐 SECRET KEY
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes("THIS_IS_MY_SECRET_KEY_12345")
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // 🎯 CLAIMS (VERY IMPORTANT)
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),   // 🔥 ROLE
            new Claim("UserId", user.Id.ToString())
        };

        // 🪪 TOKEN
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new
        {
            token = tokenString,
            username = user.Username,
            role = user.Role
        });
    }
}