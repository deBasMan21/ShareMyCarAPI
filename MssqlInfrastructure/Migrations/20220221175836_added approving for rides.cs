using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MssqlInfrastructure.Migrations
{
    public partial class addedapprovingforrides : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Rides",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "NeedsApproval",
                table: "Cars",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "NeedsApproval",
                table: "Cars");
        }
    }
}
