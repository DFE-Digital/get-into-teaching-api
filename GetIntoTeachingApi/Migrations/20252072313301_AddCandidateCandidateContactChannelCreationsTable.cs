using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GetIntoTeachingApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCandidateCandidateContactChannelCreationsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");


            migrationBuilder.CreateTable(
                name: "CandidateContactChannelCreations",
                columns: table => new
                {
                    CandidateId = table.Column<Guid>(type: "uuid", nullable: false),
                    SerialisedContactCreationChannels = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateContactChannelCreations", x => x.CandidateId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CandidateContactChannelCreations");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:postgis", ",,");
        }
    }
}
