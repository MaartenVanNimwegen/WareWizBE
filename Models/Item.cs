namespace WareWiz.Models
{
    public class Item
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "Name field is required.")]
        [StringLength(maximumLength: 100, MinimumLength = 2)]
        public string Name { get; set; }


        [Required(ErrorMessage = "Description field is required.")]
        [StringLength(maximumLength: 250, MinimumLength = 10)]
        public string Description { get; set; }


        public string? PhotoLocation { get; set; }


        [Required(ErrorMessage = "WarehouseId field is required.")]
        public int WarehouseId { get; set; }


        public DateTime CreatedDate { get; set; }


        public DateTime LastModifiedDate { get; set; }

    }
}