namespace WareWiz.ViewModels
{
    public class LocationViewModel
    {
        public int Id { get; set; }


        public string Name { get; set; }


        public Address Address { get; set; }


        public List<Warehouse> Warehouses { get; set; }


        public DateTime CreatedDate { get; set; }


        public DateTime LastModifiedDate { get; set; }
    }
}
