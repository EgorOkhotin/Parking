using Microsoft.EntityFrameworkCore.Migrations;

namespace Parking.Data.Migrations
{
    public partial class AutoId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AutoId",
                table: "Keys",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoId",
                table: "Keys");
        }
    }
}
