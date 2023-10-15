namespace WareWiz.Services
{
    public class BorrowerService
    {
        private readonly ApplicationDBContext _dbContext;

        public BorrowerService(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<int> AddBorrowerAsync(Borrower borrower)
        {
            _dbContext.Borrowers.Add(borrower);
            await _dbContext.SaveChangesAsync();

            return borrower.Id;
        }

        public async Task<Borrower> GetBorrowerByStudentNumberOrEmailAsync(string studentNumber, string email)
        {
            return await _dbContext.Borrowers
                .Where(b => b.StudentNumber == studentNumber || b.EmailAddress == email)
                .FirstOrDefaultAsync();
        }
    }
}
