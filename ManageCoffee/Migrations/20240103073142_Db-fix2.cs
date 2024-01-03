using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageCoffee.Migrations
{
    public partial class Dbfix2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdUser",
                table: "Customer",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdUser",
                table: "Customer");
        }
    }
}
