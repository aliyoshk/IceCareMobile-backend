using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IceCareNigLtd.Migrations
{
    public partial class _29_08_241305 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalDollarAmount",
                table: "Customers");

            migrationBuilder.AddColumn<int>(
                name: "Channel",
                table: "Suppliers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Channel",
                table: "Suppliers");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalDollarAmount",
                table: "Customers",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
