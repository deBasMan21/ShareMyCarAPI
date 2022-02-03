namespace ShareMyCarBackend.Models
{
    public class UpdateUserModel
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public bool SendNotifications { get; set; }
        public bool ShowEventsInCalendar { get; set; }
    }
}
