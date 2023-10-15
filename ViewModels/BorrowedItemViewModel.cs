namespace WareWiz.Models
{
    public class BorrowedItemViewModel
    {
        public int Id { get; set; }

        public int ItemId { get; set; }

        public int BorrowerId { get; set; }

        public DateTime BorrowedDate { get; set; }

        public DateTime ReturnDate { get; set; }

        public BorrowedItemStatus Status { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime LastModifiedDate { get; set; }

    }
}
