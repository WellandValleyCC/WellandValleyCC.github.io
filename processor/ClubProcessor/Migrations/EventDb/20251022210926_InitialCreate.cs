using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClubProcessor.Migrations.EventDb
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rides",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EventNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    ClubNumber = table.Column<int>(type: "INTEGER", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    IsRoadBike = table.Column<bool>(type: "INTEGER", nullable: false),
                    ActualTime = table.Column<string>(type: "TEXT", nullable: false),
                    TotalSeconds = table.Column<double>(type: "REAL", nullable: false),
                    AvgSpeed = table.Column<double>(type: "REAL", nullable: true),
                    Position = table.Column<int>(type: "INTEGER", nullable: true),
                    PointsAwarded = table.Column<int>(type: "INTEGER", nullable: true),
                    StandardTime = table.Column<string>(type: "TEXT", nullable: true),
                    TimeDelta = table.Column<string>(type: "TEXT", nullable: true),
                    HandicapGenerated = table.Column<string>(type: "TEXT", nullable: true),
                    HandicapUsed = table.Column<string>(type: "TEXT", nullable: true),
                    HCAdjustedTime = table.Column<string>(type: "TEXT", nullable: true),
                    HCRank = table.Column<int>(type: "INTEGER", nullable: true),
                    Eligibility = table.Column<string>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rides", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rides");
        }
    }
}
