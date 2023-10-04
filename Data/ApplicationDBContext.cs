namespace WareWiz.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) { }

        public DbSet<Location> Locations { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<BorrowedItem> BorrowedItems { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Borrower> Borrowers { get; set; }
    }
}
