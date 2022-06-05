using Microsoft.EntityFrameworkCore.Migrations;

namespace Poker.Migrations
{
    public partial class WinnerChecking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BestHand",
                table: "UsersInGame",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsWinner",
                table: "UsersInGame",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BestHand",
                table: "UsersInGame");

            migrationBuilder.DropColumn(
                name: "IsWinner",
                table: "UsersInGame");
        }
    }
}
