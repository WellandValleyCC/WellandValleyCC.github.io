using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClubProcessor.Migrations.CompetitorDb
{
    /// <inheritdoc />
    public partial class UseAgeGroupEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add AgeGroup column first
            migrationBuilder.AddColumn<int>(
                name: "AgeGroup",
                table: "Competitors",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0); // Adjust default if needed

            // Migrate data from flags to AgeGroup enum
            migrationBuilder.Sql(@"
                    UPDATE Competitors
                    SET AgeGroup =
                        CASE
                            WHEN IsJuvenile = 1 THEN 1
                            WHEN IsJunior = 1 THEN 2
                            WHEN IsSenior = 1 THEN 3
                            WHEN IsVeteran = 1 THEN 4
                            ELSE 0
                        END
                ");

            // Drop old boolean columns
            migrationBuilder.DropColumn(name: "IsJuvenile", table: "Competitors");
            migrationBuilder.DropColumn(name: "IsJunior", table: "Competitors");
            migrationBuilder.DropColumn(name: "IsSenior", table: "Competitors");
            migrationBuilder.DropColumn(name: "IsVeteran", table: "Competitors");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Re-add the four boolean columns
            migrationBuilder.AddColumn<bool>(
                name: "IsJuvenile",
                table: "Competitors",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsJunior",
                table: "Competitors",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSenior",
                table: "Competitors",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVeteran",
                table: "Competitors",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            // Restore flag values based on AgeGroup enum
            migrationBuilder.Sql(@"
                    UPDATE Competitors
                    SET IsJuvenile = CASE WHEN AgeGroup = 1 THEN 1 ELSE 0 END,
                        IsJunior   = CASE WHEN AgeGroup = 2 THEN 1 ELSE 0 END,
                        IsSenior   = CASE WHEN AgeGroup = 3 THEN 1 ELSE 0 END,
                        IsVeteran  = CASE WHEN AgeGroup = 4 THEN 1 ELSE 0 END
                ");

            // Drop the AgeGroup column
            migrationBuilder.DropColumn(name: "AgeGroup", table: "Competitors");
        }
    }
}
