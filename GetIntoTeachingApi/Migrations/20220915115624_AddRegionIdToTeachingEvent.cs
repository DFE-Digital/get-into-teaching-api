using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GetIntoTeachingApi.Migrations
{
    public partial class AddRegionIdToTeachingEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RegionId",
                table: "TeachingEvents",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegionId",
                table: "TeachingEvents");
        }
    }
}
