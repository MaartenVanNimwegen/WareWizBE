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

        [Required(ErrorMessage = "Phone field is required.")]
        [StringLength(maximumLength: 15, MinimumLength = 10)]
        public string Phone { get; set; }

        [StringLength(maximumLength: 250)]
        public string? StudentNumber { get; set; }

        [Required(ErrorMessage = "Usertype is required.")]
        public UserType userType { get; set; }

        [Required(ErrorMessage = "Password field is required.")]
        public string? Password { get; set; }

    }
    public enum UserType
    {
        Teacher,
        Student
    }
}   