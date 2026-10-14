using Assignment3_API.Models;
using Assignment3_API.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Assignment3_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserClaimsPrincipalFactory<AppUser> _claimsPrincipalFactory;
        private readonly IRepository _repository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthenticationController> _logger; // Add this

        public AuthenticationController(
            UserManager<AppUser> userManager,
            IUserClaimsPrincipalFactory<AppUser> claimsPrincipalFactory,
            IRepository repository,
            IConfiguration configuration,
            ILogger<AuthenticationController> logger) // Add this parameter
        {
            _userManager = userManager;
            _claimsPrincipalFactory = claimsPrincipalFactory;
            _repository = repository;
            _configuration = configuration;
            _logger = logger; // Initialize the logger
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserViewModel uvm)
        {
            try
            {
                // Validate model
                if (!ModelState.IsValid)
                    return BadRequest("Invalid request body");

                var existingUser = await _userManager.FindByEmailAsync(uvm.emailaddress);
                if (existingUser != null)
                    return BadRequest("Email already exists");

                var newUser = new AppUser
                {
                    UserName = uvm.emailaddress,
                    Email = uvm.emailaddress,
                    EmailConfirmed = true // Optional: Remove if using email confirmation
                };

                var result = await _userManager.CreateAsync(newUser, uvm.password);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    return BadRequest(string.Join(", ", errors));
                }

                return Ok(new { Message = "Registration successful" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult> Login(UserViewModel uvm)
        {
            // Use FindByEmailAsync
            var user = await _userManager.FindByEmailAsync(uvm.emailaddress);

            if (user != null && await _userManager.CheckPasswordAsync(user, uvm.password))
            {
                try
                {
                    var principal = await _claimsPrincipalFactory.CreateAsync(user);
                    return GenerateJWTToken(user);
                }
                catch (Exception)
                {
                    return StatusCode(500, "Internal Server Error");
                }
            }
            else
            {
                return Unauthorized("Invalid credentials");
            }
        }


        private ActionResult GenerateJWTToken(AppUser user)
        {
            try
            {
                // 1. Validate configuration values
                var tokenKey = _configuration["Tokens:Key"];
                var issuer = _configuration["Tokens:Issuer"];
                var audience = _configuration["Tokens:Audience"];

                if (string.IsNullOrEmpty(tokenKey))
                    throw new ArgumentNullException("JWT Key is missing in configuration.");
                if (string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
                    throw new ArgumentNullException("Issuer/Audience not configured");

                // 2. Fix claims (critical change)
                var claims = new[]
                {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id), // Use user ID instead of email
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email), // Direct email claim
            new Claim(ClaimTypes.Name, user.UserName) // Standard name claim
        };

                // 3. Validate credentials
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
                if (key.KeySize < 128)
                    throw new ArgumentException("JWT key is too short. Minimum 128 bits required.");

                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                // 4. Token construction with validation
                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(3),
                    signingCredentials: credentials
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    user = user.UserName
                });
            }
            catch (Exception ex)
            {
                // 5. Detailed error logging
                _logger.LogError(ex, "JWT Generation Failed | User: {UserId} | Email: {Email}",
                    user.Id, user.Email);
                return StatusCode(500, $"Token generation failed: {ex.Message}");
            }
        }

    }
}
