using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClubProcessor.Migrations.EventDb
{
    /// <inheritdoc />
    public partial class AddCalendarEventsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CalendarEvents",
                columns: table => new
                {
                    EventID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EventDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    EventName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Miles = table.Column<double>(type: "REAL", nullable: false),
                    Location = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    IsClubChampionship = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsNonStandard10 = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsEvening10 = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsHardRideSeries = table.Column<bool>(type: "INTEGER", nullable: false),
                    SheetName = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    IsCancelled = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarEvents", x => x.EventID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalendarEvents");
        }
    }
}
