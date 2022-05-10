using Microsoft.EntityFrameworkCore.Migrations;

namespace Poker.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Room",
                columns: table => new
                {
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    RoomName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Pot = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Room", x => x.RoomId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    userName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    userMoney = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.userName);
                });

            migrationBuilder.CreateTable(
                name: "UserInRoom",
                columns: table => new
                {
                    userName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    MoneyInTable = table.Column<int>(type: "int", nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: true),
                    position = table.Column<short>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK__UserInRoo__RoomI__412EB0B6",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__UserInRoo__userN__403A8C7D",
                        column: x => x.userName,
                        principalTable: "Users",
                        principalColumn: "userName",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserInRoom_RoomId",
                table: "UserInRoom",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInRoom_userName",
                table: "UserInRoom",
                column: "userName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserInRoom");

            migrationBuilder.DropTable(
                name: "Room");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
