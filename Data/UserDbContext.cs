using UserServiceAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace UserServiceAPI.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
    }
}
