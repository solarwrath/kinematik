using Kinematik_Domain;
using Microsoft.EntityFrameworkCore;

namespace Kinematik_EntityFramework
{
    public class KinematikDbContext : DbContext
    {
        public KinematikDbContext(DbContextOptions<KinematikDbContext> options) : base(options)
        {
        }

        public DbSet<Film> Films { get; set; }
    }
}
