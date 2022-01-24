using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain
{
    public class Car
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Plate { get; set; }
        public string Image { get; set; }

        public ICollection<Ride> Rides { get; set; }

        [JsonIgnore]
        public ICollection<User> Users { get; set; }
    }
}
