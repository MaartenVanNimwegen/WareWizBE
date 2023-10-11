namespace WareWiz.Services
{
    public class TeacherService
    {
        private readonly ApplicationDBContext _dbContext;

        public TeacherService(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> IsEmailAvailable(string email)
        {
            return !await _dbContext.Teachers.AnyAsync(u => u.EmailAddress == email);
        }
    }
}
