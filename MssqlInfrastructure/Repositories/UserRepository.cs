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
    public class UserRepository : IUserRepository
    {
        private readonly SMCDbContext _context;
        public UserRepository(SMCDbContext context)
        {
            _context = context;
        }

        public async Task<User> AddCar(User user, Car car)
        {
            user.Cars.Add(car);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> Create(User user)
        {
            _context.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public User Delete(User user)
        {
            _context.Remove(user);
            _context.SaveChanges();
            return user;
        }

        public List<User> GetAll()
        {
            return _context.Users.Include(u => u.Cars).ToList();
        }

        public User GetByEmail(string email)
        {
            return _context.Users.Include(u => u.Cars).FirstOrDefault(u => u.Email == email);
        }

        public User GetById(int id)
        {
            return _context.Users.Include(u => u.Cars).FirstOrDefault(u => u.Id == id);
        }
        public User GetByName(string name)
        {
            return _context.Users.Include(u => u.Cars).FirstOrDefault(l => l.Name == name);
        }

        public async Task<User> RemoveCar(User user, Car car)
        {
            user.Cars.Remove(car);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> Update(User user)
        {
            User old = _context.Users.FirstOrDefault(u => u.Id == user.Id);
            if (old == null) { return null; }

            old.Name = user.Name;
            old.Email = user.Email;
            old.Cars = user.Cars;
            old.FBToken = user.FBToken;
            old.PhoneNumber = user.PhoneNumber;

            await _context.SaveChangesAsync();
            return old;
        }
    }
}
