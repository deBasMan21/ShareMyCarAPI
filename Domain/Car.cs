using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        [JsonIgnore]
        public string ShareCode { get; set; }
        [JsonIgnore]
        public int OwnerId { get; set; }
        [NotMapped]
        public bool IsOwner { get; set; } = false;

        public ICollection<Ride> Rides { get; set; }

        [JsonIgnore]
        public ICollection<User> Users { get; set; }
    }
}
