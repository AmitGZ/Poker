using Microsoft.EntityFrameworkCore.Migrations;

namespace Poker.Migrations
{
    public partial class addedCardNumberEnum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Number",
                table: "Cards",
                newName: "Value");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "Cards",
                newName: "Number");
        }
    }
}
