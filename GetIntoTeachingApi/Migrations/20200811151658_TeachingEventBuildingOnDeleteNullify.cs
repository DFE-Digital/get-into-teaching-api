using Microsoft.EntityFrameworkCore.Migrations;

namespace GetIntoTeachingApi.Migrations
{
    public partial class TeachingEventBuildingOnDeleteNullify : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeachingEvents_TeachingEventBuildings_BuildingId",
                table: "TeachingEvents");

            migrationBuilder.DropIndex(
                name: "IX_TeachingEvents_BuildingId",
                table: "TeachingEvents");

            migrationBuilder.CreateIndex(
                name: "IX_TeachingEvents_BuildingId",
                table: "TeachingEvents",
                column: "BuildingId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TeachingEvents_TeachingEventBuildings_BuildingId",
                table: "TeachingEvents",
                column: "BuildingId",
                principalTable: "TeachingEventBuildings",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeachingEvents_TeachingEventBuildings_BuildingId",
                table: "TeachingEvents");

            migrationBuilder.DropIndex(
                name: "IX_TeachingEvents_BuildingId",
                table: "TeachingEvents");

            migrationBuilder.CreateIndex(
                name: "IX_TeachingEvents_BuildingId",
                table: "TeachingEvents",
                column: "BuildingId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeachingEvents_TeachingEventBuildings_BuildingId",
                table: "TeachingEvents",
                column: "BuildingId",
                principalTable: "TeachingEventBuildings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
