using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IceCareNigLtd.Migrations
{
    public partial class AddPhoneNumberThirdParty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "ThirdPartyPayments",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "ThirdPartyPayments");
        }
    }
}
