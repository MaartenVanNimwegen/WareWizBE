namespace WareWiz.Models
{
    public class Location
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name field is required.")]
        [StringLength(maximumLength: 100, MinimumLength = 2)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Address field is required.")]
        [StringLength(maximumLength: 100, MinimumLength = 5)]
        public string Address { get; set; }
    }
}
