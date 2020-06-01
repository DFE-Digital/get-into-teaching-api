using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GetIntoTeachingApi.Migrations
{
    public partial class CreateTeachingEventTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Longitude",
                table: "Locations",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "REAL",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Latitude",
                table: "Locations",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "REAL",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Postcode",
                table: "Locations",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

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
                    AddressPostcode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeachingEventBuildings", x => x.Id);
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
                    BuildingId = table.Column<Guid>(nullable: true)
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
                name: "TeachingEvents");

            migrationBuilder.DropTable(
                name: "TeachingEventBuildings");

            migrationBuilder.AlterColumn<float>(
                name: "Longitude",
                table: "Locations",
                type: "REAL",
                nullable: true,
                oldClrType: typeof(double),
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "Latitude",
                table: "Locations",
                type: "REAL",
                nullable: true,
                oldClrType: typeof(double),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Postcode",
                table: "Locations",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
