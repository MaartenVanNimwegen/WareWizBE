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


        [Required(ErrorMessage = "Password field is required.")]
        public string Password { get; set; }


        public DateTime CreatedDate { get; set; }


        public DateTime LastModifiedDate { get; set; }

    }
}   