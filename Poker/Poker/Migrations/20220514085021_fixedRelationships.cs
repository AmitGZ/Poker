using Microsoft.EntityFrameworkCore.Migrations;

namespace Poker.Migrations
{
    public partial class fixedRelationships : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    _id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    _type = table.Column<int>(type: "int", nullable: false),
                    _number = table.Column<int>(type: "int", nullable: false),
                    _roomId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x._id);
                });

            migrationBuilder.CreateTable(
                name: "Pots",
                columns: table => new
                {
                    _id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    _money = table.Column<double>(type: "float", nullable: false),
                    _roomId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pots", x => x._id);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    _id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    _name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    _talkingPosition = table.Column<int>(type: "int", nullable: true),
                    _dealerPosition = table.Column<int>(type: "int", nullable: true),
                    _pot = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x._id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    userName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    _password = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    userMoney = table.Column<double>(type: "float", nullable: false),
                    _connectionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    _moneyInTable = table.Column<int>(type: "int", nullable: true),
                    _isActive = table.Column<bool>(type: "bit", nullable: true),
                    _position = table.Column<short>(type: "smallint", nullable: true),
                    _roomId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.userName);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropTable(
                name: "Pots");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
