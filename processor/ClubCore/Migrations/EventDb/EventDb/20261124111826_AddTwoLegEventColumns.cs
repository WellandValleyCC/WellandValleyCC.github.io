using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClubProcessor.Migrations.EventDb
{
    /// <inheritdoc />
    public partial class AddTwoLegEventColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTwoLegEvent",
                table: "CalendarEvents",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Leg1Name",
                table: "CalendarEvents",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Leg2Name",
                table: "CalendarEvents",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTwoLegEvent",
                table: "CalendarEvents");

            migrationBuilder.DropColumn(
                name: "Leg1Name",
                table: "CalendarEvents");

            migrationBuilder.DropColumn(
                name: "Leg2Name",
                table: "CalendarEvents");
        }
    }
}
