using Microsoft.EntityFrameworkCore.Migrations;

namespace GetIntoTeachingApi.Migrations
{
    public partial class UpdateEventSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressState",
                table: "TeachingEventBuildings");

            migrationBuilder.AddColumn<string>(
                name: "ExternalName",
                table: "TeachingEvents",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOnline",
                table: "TeachingEvents",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "TeachingEvents",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProviderContactEmail",
                table: "TeachingEvents",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProviderOrganiser",
                table: "TeachingEvents",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProviderTargetAudience",
                table: "TeachingEvents",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProviderWebsiteUrl",
                table: "TeachingEvents",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "TeachingEvents",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "TeachingEvents",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoUrl",
                table: "TeachingEvents",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Venue",
                table: "TeachingEventBuildings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalName",
                table: "TeachingEvents");

            migrationBuilder.DropColumn(
                name: "IsOnline",
                table: "TeachingEvents");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "TeachingEvents");

            migrationBuilder.DropColumn(
                name: "ProviderContactEmail",
                table: "TeachingEvents");

            migrationBuilder.DropColumn(
                name: "ProviderOrganiser",
                table: "TeachingEvents");

            migrationBuilder.DropColumn(
                name: "ProviderTargetAudience",
                table: "TeachingEvents");

            migrationBuilder.DropColumn(
                name: "ProviderWebsiteUrl",
                table: "TeachingEvents");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "TeachingEvents");

            migrationBuilder.DropColumn(
                name: "Summary",
                table: "TeachingEvents");

            migrationBuilder.DropColumn(
                name: "VideoUrl",
                table: "TeachingEvents");

            migrationBuilder.DropColumn(
                name: "Venue",
                table: "TeachingEventBuildings");

            migrationBuilder.AddColumn<string>(
                name: "AddressState",
                table: "TeachingEventBuildings",
                type: "text",
                nullable: true);
        }
    }
}
