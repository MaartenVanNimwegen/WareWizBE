namespace WareWiz.Models
{
    public class BorrowdItem
    {
        public int Id { get; set; }

        public int ItemId { get; set; }

        public int UserId { get; set; }

        public DateTime BorrowedDate { get; set; }

        public DateTime ReturnDate { get; set; }

        public Status Status{ get; set; }

    }

    public enum Status
    {
        Borrowed,
        Returned,
        Overdue
    }
}
