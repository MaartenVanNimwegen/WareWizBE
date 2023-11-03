using Microsoft.AspNetCore.Authorization;
using WareWiz.Services;
using WareWiz.ViewModels;

namespace WareWiz.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class PhotoController : ControllerBase
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly ILogger<WarehouseController> _logger;

        public PhotoController(ApplicationDBContext dbContext, ILogger<WarehouseController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Invalid file");

            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { FilePath = uniqueFileName });
        }

        [HttpGet("getByPath")]
        public IActionResult GetByPath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return BadRequest("Invalid file path");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            var fullPath = Path.Combine(uploadsFolder, filePath);

            if (!System.IO.File.Exists(fullPath))
                return NotFound("File not found");

            var fileBytes = System.IO.File.ReadAllBytes(fullPath);
            return File(fileBytes, "image/jpeg");
        }
    }
}
