using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClubProcessor.Migrations.EventDb
{
    /// <inheritdoc />
    public partial class AddsEligibleMemberRankProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventEligibleRidersRank",
                table: "Rides",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EventEligibleRoadBikeRidersRank",
                table: "Rides",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventEligibleRidersRank",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "EventEligibleRoadBikeRidersRank",
                table: "Rides");
        }
    }
}
