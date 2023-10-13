using Microsoft.AspNetCore.Authorization;
using WareWiz.Services;
using WareWiz.ViewModels;

namespace WareWiz.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class WarehouseController : ControllerBase
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly ILogger<WarehouseController> _logger;
        private readonly WarehouseService _warehouseService;

        public WarehouseController(ApplicationDBContext dbContext, ILogger<WarehouseController> logger, WarehouseService warehouseService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _warehouseService = warehouseService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetWarehouseById([Required]int id)
        {
            var warehouse = await _dbContext.Warehouses.FindAsync(id);
            if (warehouse != null)
            {
                var warehouseViewModel = new WarehouseViewModel();
                warehouseViewModel.Id = warehouse.Id;
                warehouseViewModel.Name = warehouse.Name;
                warehouseViewModel.LocationId = warehouse.LocationId;
                warehouseViewModel.CreatedDate = warehouse.CreatedDate;
                warehouseViewModel.LastModifiedDate = warehouse.LastModifiedDate;

                return Ok(warehouseViewModel);
            }
            else
            {
                _logger.LogWarning($"No warehouse found with the given id: {id}");
                return NotFound("No warehouse found with the given id");
            }
        }

        [HttpGet]
        [Route("location/{id}")]
        public async Task<IActionResult> GetWarehousesByLocationId([Required] int id)
        {
            var warehousesToReturn = new List<WarehouseViewModel>();
            var warehouses = await _dbContext.Warehouses
                .Where(w => w.LocationId == id)
                .ToListAsync();

            foreach (var warehouse in warehouses)
            {
                var warehouseViewModel = new WarehouseViewModel { Id = warehouse.Id, Name = warehouse.Name, LocationId = warehouse.LocationId, CreatedDate = warehouse.CreatedDate, LastModifiedDate = warehouse.LastModifiedDate };
                warehousesToReturn.Add(warehouseViewModel);
            }
            return Ok(warehousesToReturn);
        }

        [HttpPost]
        public async Task<IActionResult> AddWarehouse([FromBody][Required] WarehouseViewModel givenWarehouse)
        {
            if (givenWarehouse != null)
            {
                var warehouse = new Warehouse();
                warehouse.Name= givenWarehouse.Name;
                warehouse.LocationId = givenWarehouse.LocationId;
                warehouse.CreatedDate = DateTime.UtcNow;
                warehouse.LastModifiedDate = DateTime.UtcNow;

                if (await _warehouseService.AddWarehouseAsync(warehouse))
                {
                    return Ok("Warehouse registered successfully");
                }
                else
                {
                    return StatusCode(500, "Something went wrong saving the warehouse!");
                }
            }
            return BadRequest("Invalid warehouse data");
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([Required] int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state during warehouse deletion");
                    return BadRequest(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                }

                var warehouse = await _dbContext.Warehouses.FindAsync(id);
                if (warehouse == null)
                {
                    _logger.LogWarning($"No warehouse found with the given id: {id}");
                    return NotFound("No warehouse found with the given id");
                }
                else
                {
                    _dbContext.Warehouses.Remove(warehouse);
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation($"Warehouse deleted successfully with ID: {id}");
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting warehouse with id {id}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
