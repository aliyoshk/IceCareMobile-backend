using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IceCareNigLtd.Migrations
{
    public partial class ImplContd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NairaAmount",
                table: "AccountPayments",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "BalanceNaira",
                table: "AccountPayments",
                newName: "ReferenceNo");

            migrationBuilder.RenameColumn(
                name: "BalanceDollar",
                table: "AccountPayments",
                newName: "DollarRate");

            migrationBuilder.AddColumn<string>(
                name: "Approver",
                table: "ThirdPartyPayments",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "ThirdPartyPayments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceNo",
                table: "ThirdPartyPayments",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "AccountPayments",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "AccountPayments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Approver",
                table: "ThirdPartyPayments");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "ThirdPartyPayments");

            migrationBuilder.DropColumn(
                name: "ReferenceNo",
                table: "ThirdPartyPayments");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "AccountPayments");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "AccountPayments");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "AccountPayments",
                newName: "NairaAmount");

            migrationBuilder.RenameColumn(
                name: "ReferenceNo",
                table: "AccountPayments",
                newName: "BalanceNaira");

            migrationBuilder.RenameColumn(
                name: "DollarRate",
                table: "AccountPayments",
                newName: "BalanceDollar");
        }
    }
}
