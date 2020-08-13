using Microsoft.EntityFrameworkCore.Migrations;

namespace GetIntoTeachingApi.Migrations
{
    public partial class AddWebFeedIdToTeachingEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WebFeedId",
                table: "TeachingEvents",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WebFeedId",
                table: "TeachingEvents");
        }
    }
}
