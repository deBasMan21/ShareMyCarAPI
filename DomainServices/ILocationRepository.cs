using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainServices
{
    public interface ILocationRepository
    {
        public Location GetById(int id);
        public List<Location> GetAll();
        public Location GetByName(string name);
        public Task<Location> UpdateLocation(Location location);
        public Location DeleteLocation(Location location);
        public Task<Location> CreateLocation(Location location);
    }
}
