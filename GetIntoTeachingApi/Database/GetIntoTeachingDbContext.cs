using GetIntoTeachingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GetIntoTeachingApi.Database
{
    public class GetIntoTeachingDbContext : DbContext
    {
        public DbSet<Location> Locations { get; set; }
        public DbSet<TeachingEvent> TeachingEvents { get; set; }
        public DbSet<TeachingEventBuilding> TeachingEventBuildings { get; set; }

        public GetIntoTeachingDbContext(DbContextOptions<GetIntoTeachingDbContext> options) 
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Location>().Property(m => m.Coordinate).HasSrid(DbConfiguration.Wgs84Srid);
            modelBuilder.Entity<TeachingEventBuilding>().Property(m => m.Coordinate).HasSrid(DbConfiguration.Wgs84Srid);
            modelBuilder.Entity<TeachingEvent>().HasOne(c => c.Building);
        }
    }
}
