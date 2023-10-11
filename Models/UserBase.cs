namespace WareWiz.Models
{
    [Index(nameof(EmailAddress), IsUnique = true)]
    public class UserBase
    {
        public int Id { get; set; }

        
        [Required(ErrorMessage = "Name field is required.")]
        [StringLength(maximumLength: 100, MinimumLength = 2)]
        public string Name { get; set; }


        [Required(ErrorMessage = "Emailaddress field is required.")]
        [StringLength(maximumLength: 100, MinimumLength = 2)]
        [EmailAddress]
        public string EmailAddress { get; set; }


        [Required(ErrorMessage = "Phone field is required.")]
        [StringLength(maximumLength: 15, MinimumLength = 10)]
        public string Phone { get; set; }


        public DateTime CreatedDate { get; set; }


        public DateTime LastModifiedDate { get; set; }

    }
}
