using Microsoft.EntityFrameworkCore.Migrations;

namespace Poker.Migrations
{
    public partial class changedRoundToStage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Round",
                table: "Rooms",
                newName: "Stage");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Stage",
                table: "Rooms",
                newName: "Round");
        }
    }
}
