﻿// <auto-generated />
using System;
using GetIntoTeachingApi.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace GetIntoTeachingApi.Migrations
{
    [DbContext(typeof(GetIntoTeachingDbContext))]
    [Migration("20200601090855_CreateTeachingEventTables")]
    partial class CreateTeachingEventTables
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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

                    b.Property<double?>("Latitude")
                        .HasColumnType("double precision");

                    b.Property<double?>("Longitude")
                        .HasColumnType("double precision");

                    b.HasKey("Postcode");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("GetIntoTeachingApi.Models.TeachingEvent", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("BuildingId")
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<DateTime>("EndAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<DateTime>("StartAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("TypeId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("BuildingId");

                    b.ToTable("TeachingEvents");
                });

            modelBuilder.Entity("GetIntoTeachingApi.Models.TeachingEventBuilding", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
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

                    b.Property<string>("AddressState")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("TeachingEventBuildings");
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
