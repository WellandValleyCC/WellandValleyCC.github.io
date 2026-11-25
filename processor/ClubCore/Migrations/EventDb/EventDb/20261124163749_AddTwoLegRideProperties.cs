using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClubProcessor.Migrations.EventDb
{
    /// <inheritdoc />
    public partial class AddTwoLegRideProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Leg1AvgSpeed",
                table: "Rides",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Leg1TotalSeconds",
                table: "Rides",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Leg2AvgSpeed",
                table: "Rides",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Leg2TotalSeconds",
                table: "Rides",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.Sql("""
                UPDATE Rides
                SET
                    Leg1TotalSeconds = TotalSeconds,
                    Leg2TotalSeconds = 0
                """);

            migrationBuilder.DropColumn(
                name: "TotalSeconds",
                table: "Rides");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "TotalSeconds",
                table: "Rides",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.Sql("""
                UPDATE Rides
                SET TotalSeconds = Leg1TotalSeconds + Leg2TotalSeconds
                """);

            migrationBuilder.DropColumn(
                name: "Leg1AvgSpeed",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "Leg1TotalSeconds",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "Leg2AvgSpeed",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "Leg2TotalSeconds",
                table: "Rides");
        }
    }
}
