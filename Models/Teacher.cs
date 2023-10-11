namespace WareWiz.Models
{
    public class Teacher : UserBase
    {
        [Required(ErrorMessage = "Password field is required.")]
        public string Password { get; set; }

    }
}   