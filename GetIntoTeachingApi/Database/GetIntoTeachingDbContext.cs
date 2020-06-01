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
    }
}
