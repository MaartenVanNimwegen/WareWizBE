namespace WareWiz.Models
{
    public class BorrowedItem
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "ItemId field is required.")]
        public int ItemId { get; set; }


        [Required(ErrorMessage = "UserId field is required.")]
        public int UserId { get; set; }


        [Required(ErrorMessage = "BorrowedDate field is required.")]
        public DateTime BorrowedDate { get; set; }


        [Required(ErrorMessage = "ReturnDate field is required.")]
        public DateTime ReturnDate { get; set; }


        [Required(ErrorMessage = "Status field is required.")]
        public Status Status { get; set; }

    }

    public enum Status
    {
        Borrowed,
        Returned,
        Overdue
    }
}
