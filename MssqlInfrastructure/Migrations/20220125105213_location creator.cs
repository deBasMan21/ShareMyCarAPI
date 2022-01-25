using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MssqlInfrastructure.Migrations
{
    public partial class locationcreator : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatorId",
                table: "Locations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Locations");
        }
    }
}
