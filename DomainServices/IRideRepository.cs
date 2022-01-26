using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainServices
{
    public interface IRideRepository
    {
        public Ride GetById(int id);
        public List<Ride> GetAll(User user);
        public Task<Ride> Create(Ride ride);
        public Task<Ride> Update(Ride ride);
        public Task<Ride> Delete(Ride ride);

    }
}
