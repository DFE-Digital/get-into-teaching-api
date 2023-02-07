﻿// <auto-generated />
using System;
using GetIntoTeachingApi.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GetIntoTeachingApi.Migrations
{
    [DbContext(typeof(GetIntoTeachingDbContext))]
    [Migration("20230203122811_AddCountryTeachingSubjectTables")]
    partial class AddCountryTeachingSubjectTables
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "postgis");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("GetIntoTeachingApi.Models.Country", b =>
                {
                    b.Property<Guid?>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("IsoCode")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Countries");
                });

            modelBuilder.Entity("GetIntoTeachingApi.Models.Crm.PrivacyPolicy", b =>
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

            modelBuilder.Entity("GetIntoTeachingApi.Models.Crm.TeachingEvent", b =>
                {
                    b.Property<Guid?>("Id")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("BuildingId")
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<DateTime>("EndAt")
                        .HasColumnType("timestamp without time zone");

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

                    b.Property<string>("ProvidersList")
                        .HasColumnType("text");

                    b.Property<string>("ReadableId")
                        .HasColumnType("text");

                    b.Property<int?>("RegionId")
                        .HasColumnType("integer");

                    b.Property<string>("ScribbleId")
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

                    b.Property<string>("WebFeedId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("BuildingId");

                    b.ToTable("TeachingEvents");
                });

            modelBuilder.Entity("GetIntoTeachingApi.Models.Crm.TeachingEventBuilding", b =>
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

                    b.Property<string>("ImageUrl")
                        .HasColumnType("text");

                    b.Property<string>("Venue")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("TeachingEventBuildings");
                });

            modelBuilder.Entity("GetIntoTeachingApi.Models.Location", b =>
                {
                    b.Property<string>("Postcode")
                        .HasColumnType("text");

                    b.Property<Point>("Coordinate")
                        .HasColumnType("geography");

                    b.Property<int>("Source")
                        .HasColumnType("integer");

                    b.HasKey("Postcode");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("GetIntoTeachingApi.Models.LookupItem", b =>
                {
                    b.Property<Guid?>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("EntityName")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("Id", "EntityName");

                    b.ToTable("LookupItems");
                });

            modelBuilder.Entity("GetIntoTeachingApi.Models.PickListItem", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.Property<string>("EntityName")
                        .HasColumnType("text");

                    b.Property<string>("AttributeName")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("Id", "EntityName", "AttributeName");

                    b.ToTable("PickListItems");
                });

            modelBuilder.Entity("GetIntoTeachingApi.Models.TeachingSubject", b =>
                {
                    b.Property<Guid?>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("TeachingSubjects");
                });

            modelBuilder.Entity("GetIntoTeachingApi.Models.Crm.TeachingEvent", b =>
                {
                    b.HasOne("GetIntoTeachingApi.Models.Crm.TeachingEventBuilding", "Building")
                        .WithMany("TeachingEvents")
                        .HasForeignKey("BuildingId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Building");
                });

            modelBuilder.Entity("GetIntoTeachingApi.Models.Crm.TeachingEventBuilding", b =>
                {
                    b.Navigation("TeachingEvents");
                });
#pragma warning restore 612, 618
        }
    }
}
