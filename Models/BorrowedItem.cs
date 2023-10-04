namespace WareWiz.Models
{
    public class BorrowedItem
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "ItemId field is required.")]
        public int ItemId { get; set; }


        [Required(ErrorMessage = "BorrowerId field is required.")]
        public int BorrowerId { get; set; }


        [Required(ErrorMessage = "BorrowedDate field is required.")]
        public DateTime BorrowedDate { get; set; }


        [Required(ErrorMessage = "ReturnDate field is required.")]
        public DateTime ReturnDate { get; set; }


        [Required(ErrorMessage = "Status field is required.")]
        public BorrowedItemStatus Status { get; set; }

        public DateTime CreatedDate { get; set; }


        public DateTime LastModifiedDate { get; set; }

    }

    public enum BorrowedItemStatus
    {
        Borrowed,
        Returned
    }
}
