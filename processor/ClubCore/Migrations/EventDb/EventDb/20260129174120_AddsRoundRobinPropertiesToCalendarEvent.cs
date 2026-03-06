using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClubProcessor.Migrations.EventDb
{
    /// <inheritdoc />
    public partial class AddsRoundRobinPropertiesToCalendarEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRoundRobinEvent",
                table: "CalendarEvents",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RoundRobinClub",
                table: "CalendarEvents",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRoundRobinEvent",
                table: "CalendarEvents");

            migrationBuilder.DropColumn(
                name: "RoundRobinClub",
                table: "CalendarEvents");
        }
    }
}
