using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClubProcessor.Migrations.EventDb
{
    /// <inheritdoc />
    public partial class RideModelConsolidation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualTime",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "HCAdjustedTime",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "HandicapGenerated",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "HandicapUsed",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "StandardTime",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "TimeDelta",
                table: "Rides");

            migrationBuilder.RenameColumn(
                name: "Position",
                table: "Rides",
                newName: "WomenPosition");

            migrationBuilder.RenameColumn(
                name: "PointsAwarded",
                table: "Rides",
                newName: "WomenPoints");

            migrationBuilder.RenameColumn(
                name: "HCRank",
                table: "Rides",
                newName: "VeteransPosition");

            migrationBuilder.AddColumn<int>(
                name: "EventPosition",
                table: "Rides",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EventRoadBikePosition",
                table: "Rides",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "JuniorsPoints",
                table: "Rides",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "JuniorsPosition",
                table: "Rides",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "JuvenilesPoints",
                table: "Rides",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "JuvenilesPosition",
                table: "Rides",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "League1Points",
                table: "Rides",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "League1Position",
                table: "Rides",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "League2Points",
                table: "Rides",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "League2Position",
                table: "Rides",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "League3Points",
                table: "Rides",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "League3Position",
                table: "Rides",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "League4Points",
                table: "Rides",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "League4Position",
                table: "Rides",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NevBrooksPoints",
                table: "Rides",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NevBrooksPosition",
                table: "Rides",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NevBrooksSecondsAdjustedTime",
                table: "Rides",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NevBrooksSecondsApplied",
                table: "Rides",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NevBrooksSecondsGenerated",
                table: "Rides",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PremPoints",
                table: "Rides",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PremPosition",
                table: "Rides",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoadBikeMenPoints",
                table: "Rides",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoadBikeMenPosition",
                table: "Rides",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoadBikeWomenPoints",
                table: "Rides",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoadBikeWomenPosition",
                table: "Rides",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SeniorsPoints",
                table: "Rides",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VeteransPoints",
                table: "Rides",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventPosition",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "EventRoadBikePosition",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "JuniorsPoints",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "JuniorsPosition",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "JuvenilesPoints",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "JuvenilesPosition",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "League1Points",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "League1Position",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "League2Points",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "League2Position",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "League3Points",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "League3Position",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "League4Points",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "League4Position",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "NevBrooksPoints",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "NevBrooksPosition",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "NevBrooksSecondsAdjustedTime",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "NevBrooksSecondsApplied",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "NevBrooksSecondsGenerated",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "PremPoints",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "PremPosition",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "RoadBikeMenPoints",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "RoadBikeMenPosition",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "RoadBikeWomenPoints",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "RoadBikeWomenPosition",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "SeniorsPoints",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "VeteransPoints",
                table: "Rides");

            migrationBuilder.RenameColumn(
                name: "WomenPosition",
                table: "Rides",
                newName: "Position");

            migrationBuilder.RenameColumn(
                name: "WomenPoints",
                table: "Rides",
                newName: "PointsAwarded");

            migrationBuilder.RenameColumn(
                name: "VeteransPosition",
                table: "Rides",
                newName: "HCRank");

            migrationBuilder.AddColumn<string>(
                name: "ActualTime",
                table: "Rides",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HCAdjustedTime",
                table: "Rides",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HandicapGenerated",
                table: "Rides",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HandicapUsed",
                table: "Rides",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StandardTime",
                table: "Rides",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TimeDelta",
                table: "Rides",
                type: "TEXT",
                nullable: true);
        }
    }
}
