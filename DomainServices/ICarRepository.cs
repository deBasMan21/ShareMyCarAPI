using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainServices
{
    public interface ICarRepository
    {
        public Car GetById(int id, User user);
        public List<Car> GetAll(User user);
        public Task<Car> Create(Car car, User user);
        public Task<Car> Update(Car car, User user);
        public Car Delete(Car car, User user);
        public Task<Car> AddRideToCar(Car car, Ride ride, User user);
        public Task<Car> RemoveRideFromCar(Car car, Ride ride, User user);
    }
}
