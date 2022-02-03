using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfilePicture { get; set; }
        public bool ShowEventsInCalendar { get; set; }
        public ICollection<Car> Cars { get; set; }

        [JsonIgnore]
        public bool SendNotifications { get; set; }
        [JsonIgnore]
        public string FBToken { get; set; }
    }
}