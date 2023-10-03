using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WareWiz.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly ILogger<UserController> _logger;

        public UserController(ApplicationDBContext dbContext, ILogger<UserController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
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
        public async Task<IActionResult> Post([FromBody] User formUser)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state during user creation");
                    return BadRequest(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                }

                var user = new User
                {
                    Name = formUser.Name,
                    Email = formUser.Email,
                    Phone = formUser.Phone,
                    StudentNumber = formUser.StudentNumber,
                    userType = formUser.userType,
                    Password = formUser.Password
                };

                _dbContext.Users.Add(user);

                try
                {
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation($"User added successfully with ID: {user.Id}");
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
                _logger.LogError($"Error creating user: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut]
        public async Task<IActionResult> Put(int id, [FromBody] User formUser)
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

                user.Name = formUser.Name;
                user.Email = formUser.Email;
                user.Phone = formUser.Phone;
                user.StudentNumber = formUser.StudentNumber;
                user.userType = formUser.userType;
                user.Password = formUser.Password;

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
        public IActionResult Delete([Required] int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state during user deletion");
                    return BadRequest(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                }

                var user = _dbContext.Users.Find(id);
                if (user == null)
                {
                    _logger.LogWarning($"No user found with the given id: {id}");
                    return NotFound("No user found with the given id");
                }
                else
                {
                    _dbContext.Users.Remove(user);
                    _dbContext.SaveChanges();
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
