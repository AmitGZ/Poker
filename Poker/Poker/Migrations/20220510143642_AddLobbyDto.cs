using Microsoft.EntityFrameworkCore.Migrations;

namespace Poker.Migrations
{
    public partial class AddLobbyDto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_Room_RoomId",
                table: "Cards");

            migrationBuilder.DropForeignKey(
                name: "FK_Pots_Room_RoomId",
                table: "Pots");

            migrationBuilder.DropForeignKey(
                name: "FK__UserInRoo__RoomI__412EB0B6",
                table: "UserInRoom");

            migrationBuilder.DropIndex(
                name: "IX_UserInRoom_userName",
                table: "UserInRoom");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Room",
                table: "Room");

            migrationBuilder.RenameTable(
                name: "UserInRoom",
                newName: "UsersInRoom");

            migrationBuilder.RenameTable(
                name: "Room",
                newName: "Rooms");

            migrationBuilder.RenameIndex(
                name: "IX_UserInRoom_RoomId",
                table: "UsersInRoom",
                newName: "IX_UsersInRoom_RoomId");

            migrationBuilder.AlterColumn<double>(
                name: "userMoney",
                table: "Users",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsersInRoom",
                table: "UsersInRoom",
                column: "userName");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rooms",
                table: "Rooms",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_Rooms_RoomId",
                table: "Cards",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "RoomId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pots_Rooms_RoomId",
                table: "Pots",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "RoomId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersInRoom_Rooms_RoomId",
                table: "UsersInRoom",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "RoomId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_Rooms_RoomId",
                table: "Cards");

            migrationBuilder.DropForeignKey(
                name: "FK_Pots_Rooms_RoomId",
                table: "Pots");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersInRoom_Rooms_RoomId",
                table: "UsersInRoom");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsersInRoom",
                table: "UsersInRoom");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rooms",
                table: "Rooms");

            migrationBuilder.RenameTable(
                name: "UsersInRoom",
                newName: "UserInRoom");

            migrationBuilder.RenameTable(
                name: "Rooms",
                newName: "Room");

            migrationBuilder.RenameIndex(
                name: "IX_UsersInRoom_RoomId",
                table: "UserInRoom",
                newName: "IX_UserInRoom_RoomId");

            migrationBuilder.AlterColumn<int>(
                name: "userMoney",
                table: "Users",
                type: "int",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Room",
                table: "Room",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInRoom_userName",
                table: "UserInRoom",
                column: "userName");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_Room_RoomId",
                table: "Cards",
                column: "RoomId",
                principalTable: "Room",
                principalColumn: "RoomId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pots_Room_RoomId",
                table: "Pots",
                column: "RoomId",
                principalTable: "Room",
                principalColumn: "RoomId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK__UserInRoo__RoomI__412EB0B6",
                table: "UserInRoom",
                column: "RoomId",
                principalTable: "Room",
                principalColumn: "RoomId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
