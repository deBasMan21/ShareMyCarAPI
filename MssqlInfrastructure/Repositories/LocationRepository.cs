using Domain;
using DomainServices;
using MssqlInfrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MssqlInfrastructure.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly SMCDbContext _context;

        public LocationRepository(SMCDbContext context)
        {
            _context = context;
        }
        public async Task<Location> CreateLocation(Location location)
        {
            _context.Locations.Add(location);
            await _context.SaveChangesAsync();
            return location;
        }

        public Location DeleteLocation(Location location)
        {
            _context.Remove(location);
            _context.SaveChanges();
            return location;
        }

        public List<Location> GetAll()
        {
            return _context.Locations.ToList();
        }

        public Location GetById(int id)
        {
            return _context.Locations.FirstOrDefault(l => l.Id == id);
        }

        public Location GetByName(string name)
        {
            return _context.Locations.FirstOrDefault(l => l.Name == name);
        }

        public async Task<Location> UpdateLocation(Location location)
        {
            Location old = _context.Locations.FirstOrDefault(l => l.Id == location.Id);
            if (old != null) { return null; }
            //old.City = location.City;
            //old.Name = location.Name;
            //old.Address = location.Address;
            //old.ZipCode = location.ZipCode;

            old = location;

            await _context.SaveChangesAsync();
            return old;
        }
    }
}
