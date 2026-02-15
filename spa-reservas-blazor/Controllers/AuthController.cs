using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using spa_reservas_blazor.Application.Interfaces;
using spa_reservas_blazor.Shared.DTOs;
using spa_reservas_blazor.Shared.Entities;

namespace spa_reservas_blazor.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthController(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        if (await _userRepository.ExistsAsync(request.Email))
        {
            return BadRequest("Email already registered.");
        }

        // Hash password (Simple BCrypt or similar recommended, doing simple hash for demo/speed if needed, but lets use BCrypt if possible, or simple sha256 for now to avoid external deps if not installed)
        // For simplicity in this context without adding NuGet packages interactively, I'll use a simple hashing method. 
        // IDEALLY: Use BCrypt.Net-Next
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = passwordHash,
            Role = "Client" 
        };

        // ADMIN SEED CHECK: If it's the specific admin email (or first user), make them admin
        if (request.Email.ToLower() == "admin@relaxspa.com") 
        {
            user.Role = "Admin";
        }

        await _userRepository.CreateAsync(user);

        return Ok(GenerateToken(user));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
        {
            return BadRequest("User not found.");
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return BadRequest("Wrong password.");
        }

        return Ok(GenerateToken(user));
    }

    private AuthResponse GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return new AuthResponse
        {
            Token = jwt,
            UserName = user.Name,
            Role = user.Role
        };
    }
}
