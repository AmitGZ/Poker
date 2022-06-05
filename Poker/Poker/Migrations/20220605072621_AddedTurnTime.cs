using Microsoft.EntityFrameworkCore.Migrations;

namespace Poker.Migrations
{
    public partial class AddedTurnTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TurnTime",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TurnTime",
                table: "Rooms");
        }
    }
}
