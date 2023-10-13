using Microsoft.AspNetCore.Authorization;
using WareWiz.Services;
using WareWiz.ViewModels;

namespace WareWiz.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class LocationController : ControllerBase
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly ILogger<LocationController> _logger;
        private readonly LocationService _locationService;

        public LocationController(ApplicationDBContext dbContext, ILogger<LocationController> logger, LocationService locationService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _locationService = locationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLocations()
        {
            var locations = await _dbContext.Locations
                .Include(l => l.Warehouses)
                .Include(l => l.Address)
                .ToListAsync();

            if (locations.Count() == 0 || locations == null)
            {
                return NotFound("No locations found.");
            }

            return Ok(locations);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetLocationById([Required]int id)
        {
            var location = await _dbContext.Locations
                .Where(l => l.Id == id)
                .Include (l => l.Warehouses)
                .Include(l => l.Address)
                .FirstOrDefaultAsync();

            if (location != null)
            {
                var locationViewModel = new LocationViewModel();
                locationViewModel.Id = location.Id;
                locationViewModel.Name = location.Name;
                locationViewModel.Address = location.Address;
                locationViewModel.CreatedDate = location.CreatedDate;
                locationViewModel.LastModifiedDate = location.LastModifiedDate;

                return Ok(locationViewModel);
            }
            else
            {
                _logger.LogWarning($"No location found with the given id: {id}");
                return NotFound("No location found with the given id");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddLocation([FromBody][Required] LocationViewModel givenLocation)
        {
            if (givenLocation != null)
            {
                var location = new Location();
                location.Name= givenLocation.Name;
                location.Address = givenLocation.Address;
                location.CreatedDate = DateTime.UtcNow;
                location.LastModifiedDate = DateTime.UtcNow;

                if (await _locationService.AddLocationAsync(location))
                {
                    return Ok("Location registered successfully");
                }
                else
                {
                    return StatusCode(500, "Something went wrong saving the location!");
                }
            }
            return BadRequest("Invalid location data");
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([Required] int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state during location deletion");
                    return BadRequest(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                }

                var location = await _dbContext.Locations.FindAsync(id);
                if (location == null)
                {
                    _logger.LogWarning($"No location found with the given id: {id}");
                    return NotFound("No location found with the given id");
                }
                else
                {
                    _dbContext.Locations.Remove(location);
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation($"Locaiton deleted successfully with ID: {id}");
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting location with id {id}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
