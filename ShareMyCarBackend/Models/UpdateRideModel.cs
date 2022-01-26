namespace ShareMyCarBackend.Models
{
    public class UpdateRideModel
    {
        public string Name { get; set; }
        public DateTime BeginDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public int LocationId { get; set; }
    }
}
