using Microsoft.EntityFrameworkCore.Migrations;

namespace Poker.Migrations
{
    public partial class bigBlind : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BigBlind",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BigBlind",
                table: "Rooms");
        }
    }
}
