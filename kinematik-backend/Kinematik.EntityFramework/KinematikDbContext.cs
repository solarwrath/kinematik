using System.Reflection;
using Kinematik.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kinematik.EntityFramework
{
    public class KinematikDbContext : DbContext
    {
        public DbSet<Film> Films { get; set; } = null!;
        public DbSet<Genre> Genres { get; set; } = null!;
        public DbSet<FilmToGenrePair> FilmToGenrePairs { get; set; } = null!;
        public DbSet<Hall> Halls { get; set; } = null!;
        public DbSet<Session> Sessions { get; set; } = null!;
        public DbSet<HallLayoutItem> HallLayoutItems { get; set; } = null!;
        public DbSet<Booking> Bookings { get; set; } = null!;
        public DbSet<BookedSeat> BookedSeats { get; set; } = null!;

        public KinematikDbContext(DbContextOptions<KinematikDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}