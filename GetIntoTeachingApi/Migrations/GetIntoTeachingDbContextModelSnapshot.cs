﻿// <auto-generated />
using System;
using GetIntoTeachingApi.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace GetIntoTeachingApi.Migrations
{
    [DbContext(typeof(GetIntoTeachingDbContext))]
    partial class GetIntoTeachingDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("GetIntoTeachingApi.Models.Location", b =>
                {
                    b.Property<string>("Postcode")
                        .HasColumnType("text");

                    b.Property<Point>("Coordinate")
                        .HasColumnType("geography");

                    b.HasKey("Postcode");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("GetIntoTeachingApi.Models.PrivacyPolicy", b =>
                {
                    b.Property<Guid?>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Text")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("PrivacyPolicies");
                });

            modelBuilder.Entity("GetIntoTeachingApi.Models.TeachingEvent", b =>
                {
                    b.Property<Guid?>("Id")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("BuildingId")
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<DateTime>("EndAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("ExternalName")
                        .HasColumnType("text");

                    b.Property<bool>("IsOnline")
                        .HasColumnType("boolean");

                    b.Property<string>("Message")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("ProviderContactEmail")
                        .HasColumnType("text");

                    b.Property<string>("ProviderOrganiser")
                        .HasColumnType("text");

                    b.Property<string>("ProviderTargetAudience")
                        .HasColumnType("text");

                    b.Property<string>("ProviderWebsiteUrl")
                        .HasColumnType("text");

                    b.Property<string>("ReadableId")
                        .HasColumnType("text");

                    b.Property<DateTime>("StartAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("StatusId")
                        .HasColumnType("integer");

                    b.Property<string>("Summary")
                        .HasColumnType("text");

                    b.Property<int>("TypeId")
                        .HasColumnType("integer");

                    b.Property<string>("VideoUrl")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("BuildingId");

                    b.ToTable("TeachingEvents");
                });

            modelBuilder.Entity("GetIntoTeachingApi.Models.TeachingEventBuilding", b =>
                {
                    b.Property<Guid?>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("AddressCity")
                        .HasColumnType("text");

                    b.Property<string>("AddressLine1")
                        .HasColumnType("text");

                    b.Property<string>("AddressLine2")
                        .HasColumnType("text");

                    b.Property<string>("AddressLine3")
                        .HasColumnType("text");

                    b.Property<string>("AddressPostcode")
                        .HasColumnType("text");

                    b.Property<Point>("Coordinate")
                        .HasColumnType("geography");

                    b.Property<string>("Venue")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("TeachingEventBuildings");
                });

            modelBuilder.Entity("GetIntoTeachingApi.Models.TypeEntity", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("EntityName")
                        .HasColumnType("text");

                    b.Property<string>("AttributeName")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("Id", "EntityName", "AttributeName");

                    b.ToTable("TypeEntities");
                });

            modelBuilder.Entity("GetIntoTeachingApi.Models.TeachingEvent", b =>
                {
                    b.HasOne("GetIntoTeachingApi.Models.TeachingEventBuilding", "Building")
                        .WithMany()
                        .HasForeignKey("BuildingId");
                });
#pragma warning restore 612, 618
        }
    }
}
