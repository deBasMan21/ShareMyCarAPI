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

        public List<Location> GetAll(int userId)
        {
            return _context.Locations.Where(l => l.CreatorId == userId).ToList();
        }

        public Location GetById(int id, int userId)
        {
            return _context.Locations.FirstOrDefault(l => l.Id == id && l.CreatorId == userId);
        }

        public Location GetByName(string name, int userId)
        {
            return _context.Locations.FirstOrDefault(l => l.Name == name && l.CreatorId == userId);
        }

        public async Task<Location> UpdateLocation(Location location)
        {
            Location old = _context.Locations.FirstOrDefault(l => l.Id == location.Id);
            if (old != null) { return null; }
            old.City = location.City;
            old.Name = location.Name;
            old.Address = location.Address;
            old.ZipCode = location.ZipCode;

            await _context.SaveChangesAsync();
            return old;
        }
    }
}
