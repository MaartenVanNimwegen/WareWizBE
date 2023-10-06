using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WareWiz.Services;

namespace WareWiz.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly ILogger<UserController> _logger;


        public AuthController(ApplicationDBContext dbContext, ILogger<UserController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Login(string email, string enteredPassword)
        {
            try
            {
                _logger.LogInformation($"Login attempt for email: {email}");
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user != null)
                {
                    var keyBytes = new byte[32];
                    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                    {
                        rng.GetBytes(keyBytes);
                    }

                    bool passwordMatch = AuthenticateService.VerifyPassword(enteredPassword, user.Password);

                    if (passwordMatch)
                    {
                        var claims = new[]
                        {
                            new Claim(ClaimTypes.Name, user.Name),
                            new Claim(ClaimTypes.Email, user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                        };
                        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("ThisIsMyKey!@12345"));   

                        var token = new JwtSecurityToken(
                            issuer: "WareWiz",
                            audience: "WareWiz",
                            expires: DateTime.Now.AddMinutes(30),
                            claims: claims,
                            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
                        );

                        _logger.LogInformation($"User {email} successfully authenticated.");
                        return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                    }
                    else
                    {
                        _logger.LogWarning($"Password mismatch for user {email}.");
                    }
                }
                else
                {
                    _logger.LogWarning($"User with email {email} not found.");
                }

                _logger.LogWarning($"Authentication failed for user {email}.");
                return Unauthorized();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred during login: {ex.Message}", ex);
                return StatusCode(500, "An error occurred during login.");
            }
        }
    }
}
