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
    public class CarRepository : ICarRepository
    {
        private readonly SMCDbContext _context;

        public CarRepository(SMCDbContext context)
        {
            _context = context;
        }

        public Task<Car> Create(Car car)
        {
            throw new NotImplementedException();
        }

        public Car Delete(Car car)
        {
            throw new NotImplementedException();
        }

        public List<Car> GetAll()
        {
            return _context.Cars.ToList();
        }

        public Car GetById(int id)
        {
            return _context.Cars.FirstOrDefault(c => c.Id == id);
        }

        public Task<Car> Update(Car car)
        {
            throw new NotImplementedException();
        }
    }
}
