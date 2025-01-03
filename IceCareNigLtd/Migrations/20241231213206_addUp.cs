using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IceCareNigLtd.Migrations
{
    public partial class addUp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BalanceDollar",
                table: "ThirdPartyPayments");

            migrationBuilder.DropColumn(
                name: "BalanceDollar",
                table: "AccountTopUps");

            migrationBuilder.DropColumn(
                name: "BalanceNaira",
                table: "AccountTopUps");

            migrationBuilder.RenameColumn(
                name: "BalanceNaira",
                table: "ThirdPartyPayments",
                newName: "Email");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Email",
                table: "ThirdPartyPayments",
                newName: "BalanceNaira");

            migrationBuilder.AddColumn<decimal>(
                name: "BalanceDollar",
                table: "ThirdPartyPayments",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BalanceDollar",
                table: "AccountTopUps",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BalanceNaira",
                table: "AccountTopUps",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
