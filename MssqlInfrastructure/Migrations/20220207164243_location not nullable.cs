using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MssqlInfrastructure.Migrations
{
    public partial class locationnotnullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rides_Locations_DestinationId",
                table: "Rides");

            migrationBuilder.AlterColumn<int>(
                name: "DestinationId",
                table: "Rides",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Rides_Locations_DestinationId",
                table: "Rides",
                column: "DestinationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rides_Locations_DestinationId",
                table: "Rides");

            migrationBuilder.AlterColumn<int>(
                name: "DestinationId",
                table: "Rides",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Rides_Locations_DestinationId",
                table: "Rides",
                column: "DestinationId",
                principalTable: "Locations",
                principalColumn: "Id");
        }
    }
}
