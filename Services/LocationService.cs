namespace WareWiz.Services
{
    public class LocationService
    {
        private readonly ApplicationDBContext _dbContext;


        public LocationService(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddLocationAsync(Location location)
        {
            try
            {
                _dbContext.Locations.Add(location);
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
