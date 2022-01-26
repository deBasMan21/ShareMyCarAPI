using Domain;

namespace ShareMyCarBackend.Models
{
    public class NewRideModel
    {
        public string Name { get; set; }
        public DateTime BeginDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public int LocationId { get; set; }
        public int CarId { get; set; }
    }
}
