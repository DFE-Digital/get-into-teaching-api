using System;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using Microsoft.EntityFrameworkCore;

namespace GetIntoTeachingApi.Database
{
    public class GetIntoTeachingDbContext : DbContext
    {
        public DbSet<Location> Locations { get; set; }
        public DbSet<TeachingEvent> TeachingEvents { get; set; }
        public DbSet<TeachingEventBuilding> TeachingEventBuildings { get; set; }
        public DbSet<PrivacyPolicy> PrivacyPolicies { get; set; }
        public DbSet<LookupItem> LookupItems { get; set; }
        public DbSet<PickListItem> PickListItems { get; set; }

        public GetIntoTeachingDbContext(DbContextOptions<GetIntoTeachingDbContext> options)
            : base(options)
        {
            ConfigureNpgsql();
        }

        public static void ConfigureNpgsql()
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TeachingEvent>().HasOne(c => c.Building).WithMany(b => b.TeachingEvents)
                .HasForeignKey(e => e.BuildingId).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<LookupItem>().HasKey(t => new { t.Id, t.EntityName });
            modelBuilder.Entity<PickListItem>().HasKey(t => new { t.Id, t.EntityName, t.AttributeName });
        }
    }
}
