using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClubProcessor.Migrations.EventDb
{
    /// <inheritdoc />
    public partial class AddPointsAllocations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PointsAllocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Position = table.Column<int>(type: "INTEGER", nullable: false),
                    Points = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointsAllocations", x => x.Id);
                });

            var allocations = Enumerable.Range(1, 100)
                .Select(pos => new
                {
                    Position = pos,
                    Points = pos switch
                    {
                        1 => 60,
                        2 => 55,
                        3 => 51,
                        4 => 48,
                        5 => 46,
                        6 => 44,
                        7 => 42,
                        8 => 40,
                        9 => 39,
                        10 => 38,
                        11 => 37,
                        12 => 36,
                        13 => 35,
                        14 => 34,
                        15 => 33,
                        16 => 32,
                        17 => 31,
                        18 => 30,
                        19 => 29,
                        20 => 28,
                        21 => 27,
                        22 => 26,
                        23 => 25,
                        24 => 24,
                        25 => 23,
                        26 => 22,
                        27 => 21,
                        28 => 20,
                        29 => 19,
                        30 => 18,
                        31 => 17,
                        32 => 16,
                        33 => 15,
                        34 => 14,
                        35 => 13,
                        36 => 12,
                        37 => 11,
                        38 => 10,
                        39 => 9,
                        40 => 8,
                        41 => 7,
                        42 => 6,
                        43 => 5,
                        44 => 4,
                        45 => 3,
                        46 => 2,
                        >= 47 and <= 100 => 1,
                        _ => 0
                    }
                });

            foreach (var alloc in allocations)
            {
                migrationBuilder.InsertData(
                    table: "PointsAllocations",
                    columns: new[] { "Position", "Points" },
                    values: new object[] { alloc.Position, alloc.Points });
            }

            migrationBuilder.CreateIndex(
                name: "IX_PointsAllocations_Position",
                table: "PointsAllocations",
                column: "Position",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PointsAllocations");
        }
    }
}
