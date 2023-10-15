using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WareWiz.Models;


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
        private readonly BorrowerService _borrowerService;

        public ItemController(ApplicationDBContext dbContext, ILogger<ItemController> logger, ItemService itemService, BorrowerService borrowerService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _itemService = itemService;
            _borrowerService = borrowerService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetItemById([Required] int id)
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

        [HttpPost]
        [Route("borrow")]
        public async Task<IActionResult> BorrowItem(int itemId, DateTime returnDate, BorrowerViewModel givenBorrower)
        {
            var item = await _dbContext.Items.FirstOrDefaultAsync(i => i.Id == itemId);

            if (item == null)
            {
                return BadRequest("No item with the given id was found.");
            }

            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            var isAvailable = await _dbContext.Items.AnyAsync(i => i.Id == itemId && i.Status == 0);

            if (!isAvailable)
            {
                return BadRequest("This item is not available to borrow.");
            }

            var existingBorrower = await _borrowerService.GetBorrowerByStudentNumberOrEmailAsync(givenBorrower.StudentNumber, givenBorrower.EmailAddress);

            if (existingBorrower == null)
            {
                var borrower = new Borrower
                {
                    Name = givenBorrower.Name,
                    EmailAddress = givenBorrower.EmailAddress,
                    Phone = givenBorrower.Phone,
                    StudentNumber = givenBorrower.StudentNumber,
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedDate = DateTime.UtcNow
                };

                var borrowerId = await _borrowerService.AddBorrowerAsync(borrower);

                var borrowedItem = new BorrowedItem
                {
                    ItemId = itemId,
                    BorrowerId = borrowerId,
                    BorrowedDate = DateTime.UtcNow,
                    ReturnDate = returnDate,
                    Status = 0,
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedDate = DateTime.UtcNow,
                };

                _dbContext.BorrowedItems.Add(borrowedItem);
                item.Status = ItemStatus.Borrowed;
                await _dbContext.SaveChangesAsync();
                return Ok(borrowedItem);
            }
            else
            {
                var borrowedItem = new BorrowedItem
                {
                    ItemId = itemId,
                    BorrowerId = existingBorrower.Id,
                    BorrowedDate = DateTime.UtcNow,
                    ReturnDate = returnDate,
                    Status = 0,
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedDate = DateTime.UtcNow,
                };

                _dbContext.BorrowedItems.Add(borrowedItem);
                item.Status = ItemStatus.Borrowed;
                await _dbContext.SaveChangesAsync();
                return Ok(borrowedItem);
            }
        }
    }
}
