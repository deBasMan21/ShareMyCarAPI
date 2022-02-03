using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MssqlInfrastructure.Migrations
{
    public partial class updateduser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfilePicture",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "SendNotifications",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowEventsInCalendar",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SendNotifications",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ShowEventsInCalendar",
                table: "Users");
        }
    }
}
