using GetIntoTeachingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GetIntoTeachingApi.Database
{
    public class GetIntoTeachingDbContext : DbContext
    {
        public DbSet<Location> Locations { get; set; }
        public DbSet<TeachingEvent> TeachingEvents { get; set; }
        public DbSet<TeachingEventBuilding> TeachingEventBuildings { get; set; }
        public DbSet<PrivacyPolicy> PrivacyPolicies { get; set; }
        public DbSet<TypeEntity> TypeEntities { get; set; }

        public GetIntoTeachingDbContext(DbContextOptions<GetIntoTeachingDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TeachingEvent>().HasOne(c => c.Building).WithMany(b => b.TeachingEvents)
                .HasForeignKey(e => e.BuildingId).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<TypeEntity>().HasKey(t => new { t.Id, t.EntityName, t.AttributeName });
        }
    }
}
