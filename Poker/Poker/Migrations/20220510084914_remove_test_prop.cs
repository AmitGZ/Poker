using Microsoft.EntityFrameworkCore.Migrations;

namespace Poker.Migrations
{
    public partial class remove_test_prop : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TestProp",
                table: "Cards");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TestProp",
                table: "Cards",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
