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
        public Car GetById(int id);
        public List<Car> GetAll();
        public Task<Car> Create(Car car);
        public Task<Car> Update(Car car);
        public Car Delete(Car car);
    }
}
