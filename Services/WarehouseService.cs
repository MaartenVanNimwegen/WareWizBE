namespace WareWiz.Services
{
    public class WarehouseService
    {
        private readonly ApplicationDBContext _dbContext;


        public WarehouseService(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddWarehouseAsync(Warehouse warehouse)
        {
            try
            {
                _dbContext.Warehouses.Add(warehouse);
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
