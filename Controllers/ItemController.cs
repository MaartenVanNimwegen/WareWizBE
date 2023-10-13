using Microsoft.AspNetCore.Authorization;
using WareWiz.Services;
using WareWiz.ViewModels;

namespace WareWiz.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class ItemController : ControllerBase
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly ILogger<ItemController> _logger;
        private readonly ItemService _itemService;

        public ItemController(ApplicationDBContext dbContext, ILogger<ItemController> logger, ItemService itemService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _itemService = itemService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetItemById([Required]int id)
        {
            var item = await _dbContext.Items.FindAsync(id);
            if (item != null)
            {
                var itemViewModel = new ItemViewModel();
                itemViewModel.Id = item.Id;
                itemViewModel.Name = item.Name;
                itemViewModel.Description = item.Description;
                itemViewModel.PhotoLocation = item.PhotoLocation;
                itemViewModel.WarehouseId = item.WarehouseId;
                itemViewModel.CreatedDate = item.CreatedDate;
                itemViewModel.LastModifiedDate = item.LastModifiedDate;
                itemViewModel.Status = item.Status;

                return Ok(itemViewModel);
            }
            else
            {
                _logger.LogWarning($"No item found with the given id: {id}");
                return NotFound("No item found with the given id");
            }
        }

        [HttpGet]
        [Route("warehouse/{id}")]
        public async Task<IActionResult> GetItemsByWarehouseId([Required] int id)
        {
            var itemsToReturn = new List<ItemViewModel>();
            var items = await _dbContext.Items
                .Where(i => i.WarehouseId == id)
                .ToListAsync();

            foreach (var item in items)
            {
                var itemViewModel = new ItemViewModel { Id = item.Id, Name = item.Name, Description = item.Description, PhotoLocation = item.PhotoLocation, WarehouseId = item.WarehouseId, Status = item.Status, CreatedDate = item.CreatedDate, LastModifiedDate = item.LastModifiedDate };
                itemsToReturn.Add(itemViewModel);
            }
            return Ok(itemsToReturn);
        }

        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody][Required] ItemViewModel givenItem)
        {
            if (givenItem != null)
            {
                var item = new Item();
                item.Name = givenItem.Name;
                item.Description = givenItem.Description;
                item.PhotoLocation = givenItem.PhotoLocation;
                item.WarehouseId = givenItem.WarehouseId;
                item.Status = givenItem.Status;
                item.CreatedDate = DateTime.UtcNow;
                item.LastModifiedDate = DateTime.UtcNow;

                if (await _itemService.AddItemAsync(item))
                {
                    return Ok("Item added successfully");
                }
                else
                {
                    return StatusCode(500, "Something went wrong saving the item!");
                }
            }
            return BadRequest("Invalid item data");
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([Required] int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state during item deletion");
                    return BadRequest(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                }

                var item = await _dbContext.Items.FindAsync(id);
                if (item == null)
                {
                    _logger.LogWarning($"No item found with the given id: {id}");
                    return NotFound("No item found with the given id");
                }
                else
                {
                    _dbContext.Items.Remove(item);
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation($"Item deleted successfully with ID: {id}");
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting item with id {id}: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}