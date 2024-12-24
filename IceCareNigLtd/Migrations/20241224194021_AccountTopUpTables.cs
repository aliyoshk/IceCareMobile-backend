using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace IceCareNigLtd.Migrations
{
    public partial class AccountTopUpTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Balance",
                table: "Transfers",
                newName: "BalanceNaira");

            migrationBuilder.RenameColumn(
                name: "Balance",
                table: "ThirdPartyPayments",
                newName: "BalanceNaira");

            migrationBuilder.RenameColumn(
                name: "Balance",
                table: "AccountPayments",
                newName: "BalanceNaira");

            migrationBuilder.AddColumn<decimal>(
                name: "BalanceDollar",
                table: "Transfers",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Currency",
                table: "Transfers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "BalanceDollar",
                table: "ThirdPartyPayments",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BalanceDollar",
                table: "AccountPayments",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Currency",
                table: "AccountPayments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AccountTopUps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BalanceNaira = table.Column<decimal>(type: "numeric", nullable: false),
                    BalanceDollar = table.Column<decimal>(type: "numeric", nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    Currency = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    AccountNo = table.Column<string>(type: "text", nullable: false),
                    Reference = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    Approver = table.Column<string>(type: "text", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountTopUps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TopUpEvidences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Receipts = table.Column<string>(type: "text", nullable: false),
                    AccountTopUpId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopUpEvidences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TopUpEvidences_AccountTopUps_AccountTopUpId",
                        column: x => x.AccountTopUpId,
                        principalTable: "AccountTopUps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransferDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BankName = table.Column<string>(type: "text", nullable: false),
                    TransferredAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    AccountName = table.Column<string>(type: "text", nullable: false),
                    AccountNumber = table.Column<string>(type: "text", nullable: false),
                    AccountTopUpId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransferDetails_AccountTopUps_AccountTopUpId",
                        column: x => x.AccountTopUpId,
                        principalTable: "AccountTopUps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TopUpEvidences_AccountTopUpId",
                table: "TopUpEvidences",
                column: "AccountTopUpId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferDetails_AccountTopUpId",
                table: "TransferDetails",
                column: "AccountTopUpId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TopUpEvidences");

            migrationBuilder.DropTable(
                name: "TransferDetails");

            migrationBuilder.DropTable(
                name: "AccountTopUps");

            migrationBuilder.DropColumn(
                name: "BalanceDollar",
                table: "Transfers");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Transfers");

            migrationBuilder.DropColumn(
                name: "BalanceDollar",
                table: "ThirdPartyPayments");

            migrationBuilder.DropColumn(
                name: "BalanceDollar",
                table: "AccountPayments");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "AccountPayments");

            migrationBuilder.RenameColumn(
                name: "BalanceNaira",
                table: "Transfers",
                newName: "Balance");

            migrationBuilder.RenameColumn(
                name: "BalanceNaira",
                table: "ThirdPartyPayments",
                newName: "Balance");

            migrationBuilder.RenameColumn(
                name: "BalanceNaira",
                table: "AccountPayments",
                newName: "Balance");
        }
    }
}
