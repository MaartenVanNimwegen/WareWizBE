using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WareWiz.Models;
using WareWiz.Services;
using WareWiz.ViewModels;

namespace WareWiz.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class TeacherController : ControllerBase
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly ILogger<TeacherController> _logger;
        private readonly TeacherService _teacherService;
        private readonly AuthenticateService _authenticateService;

        public TeacherController(ApplicationDBContext dbContext, ILogger<TeacherController> logger, TeacherService teacherService, AuthenticateService authenticateService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _teacherService = teacherService;
            _authenticateService = authenticateService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTeacherById([Required]int id)
        {
            var teacher = await _dbContext.Teachers.FindAsync(id);
            if (teacher != null)
            {
                var teacherViewModel = new TeacherViewModel();
                teacherViewModel.Id = teacher.Id;
                teacherViewModel.Name = teacher.Name;
                teacherViewModel.EmailAddress = teacher.EmailAddress;
                teacherViewModel.Phone = teacher.Phone;
                teacherViewModel.CreatedDate = teacher.CreatedDate;
                teacherViewModel.LastModifiedDate = teacher.LastModifiedDate;
                return Ok(teacherViewModel);
            }
            else
            {
                _logger.LogWarning($"No teacher found with the given id: {id}");
                return NotFound("No teacher found with the given id");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddTeacher([FromBody][Required] AddTeacherViewModel givenTeacher)
        {
            if (givenTeacher != null)
            {
                if (await _teacherService.IsEmailAvailable(givenTeacher.EmailAddress))
                {
                    if (givenTeacher.Password == givenTeacher.ConfirmPassword)
                    {
                        var teacher = new Teacher();
                        teacher.Name= givenTeacher.Name;
                        teacher.EmailAddress = givenTeacher.EmailAddress;
                        teacher.Phone = givenTeacher.Phone;
                        teacher.Password = givenTeacher.Password;
                        teacher.CreatedDate = DateTime.UtcNow;
                        teacher.LastModifiedDate = DateTime.UtcNow;

                        if (await _authenticateService.RegisterTeacher(teacher))
                        {
                            return Ok("Teacher registered successfully");
                        }
                        else
                        {
                            return StatusCode(500, "Something went wrong saving the teacher!");
                        }
                    }
                    return BadRequest("The chosen passwords are not the same!");
                }
                return BadRequest("Email is already in use");
            }

            return BadRequest("Invalid teacher data");
        }

        [HttpPut]
        [Route("{id:int}/changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody][Required] ChangePasswordViewModel passwords, int id)
        {
            var teacher = await _dbContext.Teachers.FindAsync(id);
            if (teacher != null)
            {
                if (passwords.NewPassword == passwords.ConfirmNewPassword)
                {
                    if (AuthenticateService.VerifyPassword(passwords.OldPassword, teacher.Password))
                    {
                        teacher.Password = passwords.NewPassword;
                        teacher.LastModifiedDate = DateTime.UtcNow;
                        await _authenticateService.ChangePassword(teacher);
                        return Ok();
                    }
                    return BadRequest("Incorrect password!");
                }
                return BadRequest("The given passwords are not the same!");
            }
            return BadRequest("No teacher found with the given id!");
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([Required] int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state during teacher deletion");
                    return BadRequest(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                }

                var teacher = await _dbContext.Teachers.FindAsync(id);
                if (teacher == null)
                {
                    _logger.LogWarning($"No teacher found with the given id: {id}");
                    return NotFound("No teacher found with the given id");
                }
                else
                {
                    _dbContext.Teachers.Remove(teacher);
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation($"teacher deleted successfully with ID: {id}");
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting teacher with id {id}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
