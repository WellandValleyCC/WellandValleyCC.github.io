using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClubProcessor.Migrations.EventDb
{
    /// <inheritdoc />
    public partial class AddRoundRobinClubs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoundRobinClubs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ShortName = table.Column<string>(type: "TEXT", nullable: false),
                    FullName = table.Column<string>(type: "TEXT", nullable: false),
                    WebsiteUrl = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoundRobinClubs", x => x.Id);
                });

            var clubs = new[]
            {
                new { ShortName = "WVCC", FullName = "Welland Valley Cycling Club", WebsiteUrl = "https://wellandvalleycc.co.uk/" },
                new { ShortName = "HCRC", FullName = "Hinckley Cycle Racing Club", WebsiteUrl = "https://hinckleycrc.org/" },
                new { ShortName = "RFW",  FullName = "Rockingham Forest Wheelers", WebsiteUrl = "https://www.rockinghamforestwheelers.org/" },
                new { ShortName = "Ratae", FullName = "Ratae Road Club", WebsiteUrl = "https://www.rataerc.org.uk/" },
                new { ShortName = "LFCC", FullName = "Leicester Forest Cycling Club", WebsiteUrl = "https://www.leicesterforest.com/" }
            };

            foreach (var club in clubs)
            {
                migrationBuilder.InsertData(
                    table: "RoundRobinClubs",
                    columns: new[] { "ShortName", "FullName", "WebsiteUrl" },
                    values: new object[] { club.ShortName, club.FullName, club.WebsiteUrl });
            }

            migrationBuilder.CreateIndex(
                name: "IX_RoundRobinClubs_ShortName",
                table: "RoundRobinClubs",
                column: "ShortName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoundRobinClubs");
        }
    }
}
