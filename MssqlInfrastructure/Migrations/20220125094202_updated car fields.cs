using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MssqlInfrastructure.Migrations
{
    public partial class updatedcarfields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rides_Users_userId",
                table: "Rides");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "Rides",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Rides_userId",
                table: "Rides",
                newName: "IX_Rides_UserId");

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Cars",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ShareCode",
                table: "Cars",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Rides_Users_UserId",
                table: "Rides",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rides_Users_UserId",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "ShareCode",
                table: "Cars");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Rides",
                newName: "userId");

            migrationBuilder.RenameIndex(
                name: "IX_Rides_UserId",
                table: "Rides",
                newName: "IX_Rides_userId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rides_Users_userId",
                table: "Rides",
                column: "userId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
