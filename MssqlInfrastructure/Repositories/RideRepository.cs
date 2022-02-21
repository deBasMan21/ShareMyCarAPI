using Domain;
using DomainServices;
using Microsoft.EntityFrameworkCore;
using MssqlInfrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MssqlInfrastructure.Repositories
{
    public class RideRepository : IRideRepository
    {
        private readonly SMCDbContext _context;

        public RideRepository(SMCDbContext context)
        {
            _context = context;
        }

        public async Task<Ride> Create(Ride ride)
        {
            ride.ReservationDateTime = DateTime.Now;
            ride.LastChangeDateTime = DateTime.Now;
            _context.Add(ride);
            await _context.SaveChangesAsync();

            Ride updated = GetById(ride.Id);
            return updated;
        }

        public async Task<Ride> Delete(Ride ride)
        {
            ride.Car = null;
            ride.User = null;
            ride.Destination = null;
            await _context.SaveChangesAsync();

            _context.Remove(ride);
            await _context.SaveChangesAsync();
            return ride;
        }

        public List<Ride> GetAll(User user)
        {
            return _context.Rides
                .Include(r => r.Destination)
                .Include(r => r.User)
                .Include(r => r.Car)
                .Where(r => r.User == user && r.BeginDateTime > DateTime.Now.AddDays(-30) && r.Status == StatusType.APPROVED)
                .OrderBy(r => r.BeginDateTime)
                .AsNoTracking()
                .ToList();
        }

        public List<Ride> GetDenied(User user)
        {
            return _context.Rides
                .Include(r => r.Destination)
                .Include(r => r.User)
                .Include(r => r.Car)
                .Where(r => r.User == user && r.BeginDateTime > DateTime.Now && r.Status == StatusType.DENIED)
                .OrderBy(r => r.BeginDateTime)
                .AsNoTracking()
                .ToList();
        }

        public List<Ride> GetRequested(User user)
        {
            return _context.Rides
                .Include(r => r.Destination)
                .Include(r => r.User)
                .Include(r => r.Car)
                .Where(r => r.Car.OwnerId == user.Id && r.Status == StatusType.REQUESTED && r.BeginDateTime > DateTime.Now)
                .OrderBy(r => r.BeginDateTime)
                .AsNoTracking()
                .ToList();
        }

        public Ride GetById(int id)
        {
            return _context.Rides
                .Include(r => r.Destination)
                .Include(r => r.User)
                .Include(r => r.Car)
                .Where(r => r.Id == id)
                .AsNoTracking()
                .FirstOrDefault();
        }

        public async Task<Ride> Update(Ride ride)
        {
            Ride old = _context.Rides.FirstOrDefault(r => r.Id == ride.Id);

            old.Destination = ride.Destination;
            old.BeginDateTime = ride.BeginDateTime;
            old.EndDateTime = ride.EndDateTime;
            old.Name = ride.Name;
            old.LastChangeDateTime = DateTime.Now;
            old.Status = ride.Status;

            await _context.SaveChangesAsync();

            Ride updated = GetById(ride.Id);

            return updated;
        }
    }
}
