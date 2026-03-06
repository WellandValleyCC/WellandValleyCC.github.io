using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClubProcessor.Migrations.EventDb
{
    /// <inheritdoc />
    public partial class AddRoundRobinSupportToRide : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RoundRobinClub",
                table: "Rides",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "RoundRobinPoints",
                table: "Rides",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoundRobinPosition",
                table: "Rides",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "RoundRobinWomenPoints",
                table: "Rides",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoundRobinWomenPosition",
                table: "Rides",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoundRobinClub",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "RoundRobinPoints",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "RoundRobinPosition",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "RoundRobinWomenPoints",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "RoundRobinWomenPosition",
                table: "Rides");
        }
    }
}
