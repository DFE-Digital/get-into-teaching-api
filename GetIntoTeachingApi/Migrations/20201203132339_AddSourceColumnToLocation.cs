using Microsoft.EntityFrameworkCore.Migrations;

namespace GetIntoTeachingApi.Migrations
{
    public partial class AddSourceColumnToLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Source",
                table: "Locations",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Source",
                table: "Locations");
        }
    }
}
