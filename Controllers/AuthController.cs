using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WareWiz.Services;
using WareWiz.ViewModels;

namespace WareWiz.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly ILogger<TeacherController> _logger;


        public AuthController(ApplicationDBContext dbContext, ILogger<TeacherController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            try
            {
                _logger.LogInformation($"Login attempt for email: {loginViewModel.EmailAddress}");
                var teacher = await _dbContext.Teachers.FirstOrDefaultAsync(u => u.EmailAddress == loginViewModel.EmailAddress);
                if (teacher != null)
                {
                    var keyBytes = new byte[32];
                    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                    {
                        rng.GetBytes(keyBytes);
                    }

                    bool passwordMatch = AuthenticateService.VerifyPassword(loginViewModel.Password, teacher.Password);

                    if (passwordMatch)
                    {
                        var claims = new[]
                        {
                            new Claim(ClaimTypes.Name, teacher.Name),
                            new Claim(ClaimTypes.Email, teacher.EmailAddress),
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

                        _logger.LogInformation($"Teacher {loginViewModel.EmailAddress} successfully authenticated.");
                        return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                    }
                    else
                    {
                        _logger.LogWarning($"Password mismatch for teacher {loginViewModel.EmailAddress}.");
                    }
                }
                else
                {
                    _logger.LogWarning($"Teacher with email {loginViewModel.EmailAddress} not found.");
                }

                _logger.LogWarning($"Authentication failed for teacher {loginViewModel.EmailAddress}.");
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
