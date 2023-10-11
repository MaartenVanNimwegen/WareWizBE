namespace WareWiz.Models
{
    [Index(nameof(StudentNumber), IsUnique = true)]
    public class Borrower : UserBase
    {
        [Required(ErrorMessage = "Studentnumber field is required.")]
        [StringLength(maximumLength: 250)]
        public string StudentNumber { get; set; }


        public DateTime CreatedDate { get; set; }


        public DateTime LastModifiedDate { get; set; }

    }
}   