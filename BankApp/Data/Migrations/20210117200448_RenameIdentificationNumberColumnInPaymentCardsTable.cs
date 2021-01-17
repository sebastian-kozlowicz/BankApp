using Microsoft.EntityFrameworkCore.Migrations;

namespace BankApp.Data.Migrations
{
    public partial class RenameIdentificationNumberColumnInPaymentCardsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IssuerIdentificationNumber",
                table: "PaymentCards",
                newName: "BankIdentificationNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BankIdentificationNumber",
                table: "PaymentCards",
                newName: "IssuerIdentificationNumber");
        }
    }
}
