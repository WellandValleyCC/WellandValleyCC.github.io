using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClubProcessor.Migrations.EventDb
{
    /// <inheritdoc />
    public partial class AddRoundRobinClubFromYear : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Add the new column
            migrationBuilder.AddColumn<int>(
                name: "FromYear",
                table: "RoundRobinClubs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 2025); // default for existing clubs

            // 2. Update existing clubs to 2025
            migrationBuilder.Sql("""
                UPDATE RoundRobinClubs
                SET FromYear = 2025
            """);

            // 3. Insert the new club (KCC)
            migrationBuilder.InsertData(
                table: "RoundRobinClubs",
                columns: new[] { "ShortName", "FullName", "WebsiteUrl", "FromYear" },
                values: new object[]
                {
                    "KCC",
                    "Kettering Cycling Club",
                    "https://ketteringcyclingclub.co.uk/",
                    2026
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove KCC
            migrationBuilder.DeleteData(
                table: "RoundRobinClubs",
                keyColumn: "ShortName",
                keyValue: "KCC");

            // Remove the column
            migrationBuilder.DropColumn(
                name: "FromYear",
                table: "RoundRobinClubs");
        }
    }
}
