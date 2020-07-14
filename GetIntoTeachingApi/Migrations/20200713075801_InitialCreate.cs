using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

namespace GetIntoTeachingApi.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Postcode = table.Column<string>(nullable: false),
                    Coordinate = table.Column<Point>(type: "geography", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Postcode);
                });

            migrationBuilder.CreateTable(
                name: "PrivacyPolicies",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivacyPolicies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TeachingEventBuildings",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AddressLine1 = table.Column<string>(nullable: true),
                    AddressLine2 = table.Column<string>(nullable: true),
                    AddressLine3 = table.Column<string>(nullable: true),
                    AddressCity = table.Column<string>(nullable: true),
                    AddressState = table.Column<string>(nullable: true),
                    AddressPostcode = table.Column<string>(nullable: true),
                    Coordinate = table.Column<Point>(type: "geography", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeachingEventBuildings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TypeEntities",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    EntityName = table.Column<string>(nullable: false),
                    AttributeName = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeEntities", x => new { x.Id, x.EntityName, x.AttributeName });
                });

            migrationBuilder.CreateTable(
                name: "TeachingEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TypeId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    StartAt = table.Column<DateTime>(nullable: false),
                    EndAt = table.Column<DateTime>(nullable: false),
                    BuildingId = table.Column<Guid>(nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeachingEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeachingEvents_TeachingEventBuildings_BuildingId",
                        column: x => x.BuildingId,
                        principalTable: "TeachingEventBuildings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeachingEvents_BuildingId",
                table: "TeachingEvents",
                column: "BuildingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "PrivacyPolicies");

            migrationBuilder.DropTable(
                name: "TeachingEvents");

            migrationBuilder.DropTable(
                name: "TypeEntities");

            migrationBuilder.DropTable(
                name: "TeachingEventBuildings");
        }
    }
}
