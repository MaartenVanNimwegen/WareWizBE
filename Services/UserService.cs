namespace WareWiz.Services
{
    public class UserService
    {
        private readonly List<User> _users = new List<User>();

        private readonly ApplicationDBContext _dbContext;
        private readonly ILogger<UserService> _logger;


        public UserService(ApplicationDBContext dbContext, ILogger<UserService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<bool> IsEmailAvailable(string email)
        {
            return !await _dbContext.Users.AnyAsync(u => u.Email == email);
        }
    }
}
