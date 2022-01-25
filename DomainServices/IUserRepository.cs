using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainServices
{
    public interface IUserRepository
    {
        public User GetById(int id);
        public List<User> GetAll();
        public User GetByName(string name);
        public User GetByEmail(string email);
        public Task<User> Create(User user);
        public Task<User> Update(User user);
        public User Delete(User user);
    }
}
