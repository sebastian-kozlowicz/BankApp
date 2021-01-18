using Microsoft.EntityFrameworkCore.Migrations;

namespace BankApp.Data.Migrations
{
    public partial class RenameColumnInBankAccountsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CheckNumber",
                table: "BankAccounts",
                newName: "CheckDigits");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CheckDigits",
                table: "BankAccounts",
                newName: "CheckNumber");
        }
    }
}
