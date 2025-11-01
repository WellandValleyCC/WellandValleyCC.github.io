using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClubProcessor.Migrations.EventDb
{
    /// <inheritdoc />
    public partial class RenamePositionToRankInRide : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EventRoadBikePosition",
                table: "Rides",
                newName: "EventRoadBikeRank");

            migrationBuilder.RenameColumn(
                name: "EventPosition",
                table: "Rides",
                newName: "EventRank");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EventRoadBikeRank",
                table: "Rides",
                newName: "EventRoadBikePosition");

            migrationBuilder.RenameColumn(
                name: "EventRank",
                table: "Rides",
                newName: "EventPosition");
        }
    }
}
