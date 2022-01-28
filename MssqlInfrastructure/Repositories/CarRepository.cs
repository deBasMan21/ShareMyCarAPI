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
    public class CarRepository : ICarRepository
    {
        private readonly SMCDbContext _context;

        public CarRepository(SMCDbContext context)
        {
            _context = context;
        }

        public async Task<Car> AddRideToCar(Car car, Ride ride, User user)
        {
            car.Rides.Add(ride);
            await _context.SaveChangesAsync();
            car.IsOwner = car.OwnerId == user.Id;
            return car;
        }

        public async Task<Car> Create(Car car, User user)
        {
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
            car.IsOwner = car.OwnerId == user.Id;
            return car;
        }

        public Car Delete(Car car, User user)
        {
            _context.Cars.Remove(car);
            _context.SaveChanges();
            car.IsOwner = car.OwnerId == user.Id;
            return car;
        }

        public List<Car> GetAll(User user)
        {
            List<Car> car = _context.Cars.ToList();
            car.ForEach(car => car.IsOwner = car.OwnerId == user.Id);
            return car;
        }

        public Car GetById(int id, User user)
        {
            Car car = _context.Cars
                .Include(c => c.Rides)
                    .ThenInclude(r => r.Destination)
                .Include(c => c.Users)
                .FirstOrDefault(c => c.Id == id);
            car.IsOwner = car.OwnerId == user.Id;
            return car;
        }

        public async Task<Car> RemoveRideFromCar(Car car, Ride ride, User user)
        {
            car.Rides.Remove(ride);
            await _context.SaveChangesAsync();
            car.IsOwner = car.OwnerId == user.Id;
            return car;
        }

        public async Task<Car> Update(Car car, User user)
        {
            Car old = GetById(car.Id, user);
            old.Name = car.Name;
            old.ShareCode = car.ShareCode;
            old.Plate = car.Plate;
            old.Image = car.Image;

            await _context.SaveChangesAsync();

            car.IsOwner = car.OwnerId == user.Id;
            return old;
        }
    }
}
