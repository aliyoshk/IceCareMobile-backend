using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IceCareNigLtd.Migrations
{
    public partial class quickOne : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankInfo_Customers_CustomerId",
                table: "BankInfo");

            migrationBuilder.DropIndex(
                name: "IX_BankInfo_CustomerId",
                table: "BankInfo");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "BankInfo");

            migrationBuilder.CreateTable(
                name: "CustomerBankInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BankName = table.Column<string>(type: "TEXT", nullable: false),
                    AmountTransferred = table.Column<decimal>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerBankInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerBankInfo_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerBankInfo_CustomerId",
                table: "CustomerBankInfo",
                column: "CustomerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerBankInfo");

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "BankInfo",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BankInfo_CustomerId",
                table: "BankInfo",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_BankInfo_Customers_CustomerId",
                table: "BankInfo",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id");
        }
    }
}
