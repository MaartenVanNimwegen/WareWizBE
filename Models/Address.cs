namespace WareWiz.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public int Housenumber { get; set; }
        public string HousenumberPrefix { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        
    }
}
