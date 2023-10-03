namespace WareWiz.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name field is required.")]
        [StringLength(maximumLength: 100, MinimumLength = 2)]
        public string Name { get; set; }


        [Required(ErrorMessage = "Email field is required.")]
        [StringLength(maximumLength: 100, MinimumLength = 2)]
        [EmailAddress]
        public string Email { get; set; }


        public string? Password { get; set; }


        [Required(ErrorMessage = "Usertype is required.")]
        public UserType UserType { get; set; }


        [StringLength(maximumLength: 15, MinimumLength = 10)]
        public string Phone { get; set; }


        [StringLength(maximumLength: 250)]
        public string? StudentNumber { get; set; }


        public DateTime CreatedDate { get; set; }


        public DateTime LastModifiedDate { get; set; }

    }
    public enum UserType
    {
        Manager,
        Borrower
    }
}   