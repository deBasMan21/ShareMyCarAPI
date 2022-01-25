using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Ride
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BeginDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public DateTime ReservationDateTime { get; set; }
        public DateTime LastChangeDateTime { get; set; }

        public Location Destination { get; set; }

        public User User { get; set; }

        public Car Car { get; set; }
    }
}
