using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WareWiz.Models;
using WareWiz.Services;

namespace WareWiz.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly ILogger<UserController> _logger;
        private readonly UserService _userService;
        private readonly AuthenticateService _authenticateService;

        public UserController(ApplicationDBContext dbContext, ILogger<UserController> logger, UserService userService, AuthenticateService authenticateService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _userService = userService;
            _authenticateService = authenticateService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([Required] int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state during user retrieval");
                    return BadRequest(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                }

                var user = await _dbContext.Users.FindAsync(id);
                if (user == null)
                {
                    _logger.LogWarning($"No user found with the given id: {id}");
                    return NotFound("No user found with the given id");
                }
                else
                {
                    return Ok(user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving user with id {id}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("Invalid user data");
            }

            if (!await _userService.IsEmailAvailable(user.Email))
            {
                return BadRequest("Email is already in use");
            }

            if (await _authenticateService.RegisterUser(user))
            {
                return Ok("User registered successfully");
            }

            return StatusCode(500, "An error occurred");
        }

        [HttpPut]
        public async Task<IActionResult> Put(int id, string name, string email)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state during user update");
                    return BadRequest(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                }

                var user = await _dbContext.Users.FindAsync(id);
                if (user == null)
                {
                    _logger.LogWarning($"No user found with the given id: {id}");
                    return NotFound("No user found with the given id");
                }

                user.Name = name;
                user.Email = email;

                try
                {
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation($"User updated successfully with ID: {id}");
                    return Ok(user);
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError($"Error saving changes: {ex.Message}");
                    return BadRequest("Error saving changes. Please try again or contact support.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating user with id {id}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([Required] int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state during user deletion");
                    return BadRequest(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                }

                var user = await _dbContext.Users.FindAsync(id);
                if (user == null)
                {
                    _logger.LogWarning($"No user found with the given id: {id}");
                    return NotFound("No user found with the given id");
                }
                else
                {
                    _dbContext.Users.Remove(user);
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation($"User deleted successfully with ID: {id}");
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting user with id {id}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
