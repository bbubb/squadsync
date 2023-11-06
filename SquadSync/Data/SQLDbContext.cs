using Microsoft.EntityFrameworkCore;
using SquadSync.Data.Models;

namespace SquadSync.Data
{
    public class SQLDbContext : DbContext
    {
        public SQLDbContext(DbContextOptions<SQLDbContext> options) : base(options) 
        {
        }
        //DbSets for tables
        public DbSet<User> Users { get; set; }
    }
}
