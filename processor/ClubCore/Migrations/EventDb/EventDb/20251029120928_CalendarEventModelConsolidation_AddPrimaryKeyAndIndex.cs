using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClubProcessor.Migrations.EventDb
{
    /// <inheritdoc />
    public partial class CalendarEventModelConsolidation_AddPrimaryKeyAndIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SheetName",
                table: "CalendarEvents");

            migrationBuilder.RenameColumn(
                name: "EventID",
                table: "CalendarEvents",
                newName: "Id");

            migrationBuilder.AddColumn<int>(
                name: "EventNumber",
                table: "CalendarEvents",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_EventNumber",
                table: "CalendarEvents",
                column: "EventNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CalendarEvents_EventNumber",
                table: "CalendarEvents");

            migrationBuilder.DropColumn(
                name: "EventNumber",
                table: "CalendarEvents");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "CalendarEvents",
                newName: "EventID");

            migrationBuilder.AddColumn<string>(
                name: "SheetName",
                table: "CalendarEvents",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }
    }
}
