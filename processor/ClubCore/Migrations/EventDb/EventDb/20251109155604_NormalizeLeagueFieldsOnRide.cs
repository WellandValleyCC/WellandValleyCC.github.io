using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClubProcessor.Migrations.EventDb
{
    /// <inheritdoc />
    public partial class NormalizeLeagueFieldsOnRide : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "League1Points",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "League1Position",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "League2Points",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "League2Position",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "League3Points",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "League3Position",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "League4Points",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "League4Position",
                table: "Rides");

            migrationBuilder.RenameColumn(
                name: "PremPosition",
                table: "Rides",
                newName: "LeaguePosition");

            migrationBuilder.RenameColumn(
                name: "PremPoints",
                table: "Rides",
                newName: "LeaguePoints");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LeaguePosition",
                table: "Rides",
                newName: "PremPosition");

            migrationBuilder.RenameColumn(
                name: "LeaguePoints",
                table: "Rides",
                newName: "PremPoints");

            migrationBuilder.AddColumn<double>(
                name: "League1Points",
                table: "Rides",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "League1Position",
                table: "Rides",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "League2Points",
                table: "Rides",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "League2Position",
                table: "Rides",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "League3Points",
                table: "Rides",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "League3Position",
                table: "Rides",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "League4Points",
                table: "Rides",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "League4Position",
                table: "Rides",
                type: "INTEGER",
                nullable: true);
        }
    }
}
