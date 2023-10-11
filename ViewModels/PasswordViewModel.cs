namespace WareWiz.ViewModels
{
    public class PasswordViewModel
    {
        [Required]
        public string Password { get; set; }

        [Required]
        public string ConfirmPassword { get; set; }
    }
}