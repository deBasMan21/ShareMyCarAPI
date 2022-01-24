using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MssqlInfrastructure.Context
{
    public class SMCDbContext : DbContext
    {
        public DbSet<Car> Cars => Set<Car>();
        public DbSet<Ride> Rides => Set<Ride>();
        public DbSet<Location> Locations => Set<Location>();
        public DbSet<User> Users => Set<User>();
        public DbSet<FriendShip> Friends => Set<FriendShip>();

        public SMCDbContext(DbContextOptions<SMCDbContext> contextOptions) : base(contextOptions)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Car>().HasMany<User>(c => c.Users).WithMany(u => u.Cars);

            builder.Entity<User>().HasIndex(p => p.Email).IsUnique();
        }
    }
}
