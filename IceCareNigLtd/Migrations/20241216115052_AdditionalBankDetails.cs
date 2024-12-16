using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IceCareNigLtd.Migrations
{
    public partial class AdditionalBankDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountName",
                table: "TransferBanks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AccountNumber",
                table: "TransferBanks",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountName",
                table: "TransferBanks");

            migrationBuilder.DropColumn(
                name: "AccountNumber",
                table: "TransferBanks");
        }
    }
}
