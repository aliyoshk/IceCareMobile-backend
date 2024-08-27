using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IceCareNigLtd.Migrations
{
    public partial class AddAccountNumberToRegistration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountNumber",
                table: "Registrations",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountNumber",
                table: "Registrations");
        }
    }
}
