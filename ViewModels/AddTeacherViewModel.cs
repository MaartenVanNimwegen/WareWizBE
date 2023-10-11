namespace WareWiz.ViewModels
{
    public class AddTeacherViewModel : UserBase
    {
        [Required]
        public string Password { get; set; }

        [Required]
        public string ConfirmPassword { get; set; }
    }
}