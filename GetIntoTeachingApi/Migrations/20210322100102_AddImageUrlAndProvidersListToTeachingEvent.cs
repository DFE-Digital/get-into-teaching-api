using Microsoft.EntityFrameworkCore.Migrations;

namespace GetIntoTeachingApi.Migrations
{
    public partial class AddImageUrlAndProvidersListToTeachingEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProvidersList",
                table: "TeachingEvents",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "TeachingEventBuildings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProvidersList",
                table: "TeachingEvents");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "TeachingEventBuildings");
        }
    }
}
