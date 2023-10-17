using Microsoft.EntityFrameworkCore;

namespace WareWiz.Services
{
    public class ItemService
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly BorrowerService _borrowerService;


        public ItemService(ApplicationDBContext dbContext, BorrowerService borrowerService)
        {
            _dbContext = dbContext;
            _borrowerService = borrowerService;
        }

        public async Task<bool> AddItemAsync(Item item)
        {
            try
            {
                _dbContext.Items.Add(item);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public async Task<BorrowedItem> BorrowItemAsync(int itemId, DateTime returnDate, BorrowerViewModel givenBorrower)
        {
            var existingBorrower = await _borrowerService.GetBorrowerByStudentNumberOrEmailAsync(givenBorrower.StudentNumber, givenBorrower.EmailAddress);

            if (existingBorrower == null)
            {
                var borrower = CreateBorrowerFromViewModel(givenBorrower);
                var borrowerId = await _borrowerService.AddBorrowerAsync(borrower);
                return await CreateBorrowedItemAsync(itemId, borrowerId, returnDate);
            }
            else
            {
                return await CreateBorrowedItemAsync(itemId, existingBorrower.Id, returnDate);
            }
        }

        private Borrower CreateBorrowerFromViewModel(BorrowerViewModel givenBorrower)
        {
            return new Borrower
            {
                Name = givenBorrower.Name,
                EmailAddress = givenBorrower.EmailAddress,
                Phone = givenBorrower.Phone,
                StudentNumber = givenBorrower.StudentNumber,
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow
            };
        }

        private async Task<BorrowedItem> CreateBorrowedItemAsync(int itemId, int borrowerId, DateTime returnDate)
        {
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
            var item = await _dbContext.Items.FirstOrDefaultAsync(i => i.Id == itemId);
            item.Status = ItemStatus.Borrowed;
            await _dbContext.SaveChangesAsync();

            return borrowedItem;
        }

        public async Task<bool> ReturnItemAsync(int itemId)
        {
            var updatedItemStatus = await UpdateItemStatusAsync(itemId, ItemStatus.Avalible);
            var updatedBorrowedItemStatus = await UpdateBorrowedItemStatus(itemId, BorrowedItemStatus.Returned);

            if (updatedItemStatus == true && updatedBorrowedItemStatus == true)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateItemStatusAsync(int itemId, ItemStatus newStatus)
        {
            var item = await _dbContext.Items.FindAsync(itemId);

            if (item == null)
            {
                return false;
            }

            item.Status = newStatus;
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateBorrowedItemStatus(int itemId, BorrowedItemStatus newStatus)
        {
            var borrowedItem = await _dbContext.BorrowedItems
                .Where(i => i.ItemId == itemId)
                .FirstOrDefaultAsync();

            if (borrowedItem == null)
            {
                return false;
            }

            borrowedItem.Status = newStatus;
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
