using Microsoft.EntityFrameworkCore.Migrations;

namespace Poker.Migrations
{
    public partial class AddPot : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConnectionId",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DealerPosition",
                table: "Room",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TalkingPosition",
                table: "Room",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RoomId",
                table: "Cards",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Pots",
                columns: table => new
                {
                    PotId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Money = table.Column<double>(type: "float", nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pots", x => x.PotId);
                    table.ForeignKey(
                        name: "FK_Pots_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cards_RoomId",
                table: "Cards",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Pots_RoomId",
                table: "Pots",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_Room_RoomId",
                table: "Cards",
                column: "RoomId",
                principalTable: "Room",
                principalColumn: "RoomId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_Room_RoomId",
                table: "Cards");

            migrationBuilder.DropTable(
                name: "Pots");

            migrationBuilder.DropIndex(
                name: "IX_Cards_RoomId",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "ConnectionId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DealerPosition",
                table: "Room");

            migrationBuilder.DropColumn(
                name: "TalkingPosition",
                table: "Room");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "Cards");
        }
    }
}
