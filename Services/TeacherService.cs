namespace WareWiz.Services
{
    public class TeacherService
    {
        private readonly List<Teacher> _teachers = new List<Teacher>();

        private readonly ApplicationDBContext _dbContext;
        private readonly ILogger<TeacherService> _logger;


        public TeacherService(ApplicationDBContext dbContext, ILogger<TeacherService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<bool> IsEmailAvailable(string email)
        {
            return !await _dbContext.Teachers.AnyAsync(u => u.EmailAddress == email);
        }
    }
}
