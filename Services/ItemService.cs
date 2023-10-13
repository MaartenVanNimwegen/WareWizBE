namespace WareWiz.Services
{
    public class ItemService
    {
        private readonly ApplicationDBContext _dbContext;


        public ItemService(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
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
    }
}
