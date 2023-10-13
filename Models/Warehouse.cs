using Microsoft.Extensions.Hosting;

namespace WareWiz.Models
{
    public class Warehouse
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "Name field is required.")]
        [StringLength(maximumLength: 100, MinimumLength = 2)]
        public string Name { get; set; }


        public int LocationId { get; set; }

        
        public List<Item> Items { get; set; }


        public DateTime CreatedDate { get; set; }


        public DateTime LastModifiedDate { get; set; }
        
    }
}