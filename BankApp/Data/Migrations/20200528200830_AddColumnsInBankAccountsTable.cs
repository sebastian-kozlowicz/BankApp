using Microsoft.EntityFrameworkCore.Migrations;

namespace BankApp.Data.Migrations
{
    public partial class AddColumnsInBankAccountsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Number",
                table: "BankAccounts");

            migrationBuilder.AddColumn<long>(
                name: "AccountNumber",
                table: "BankAccounts",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "BranchCode",
                table: "BankAccounts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CheckNumber",
                table: "BankAccounts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                table: "BankAccounts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Iban",
                table: "BankAccounts",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NationalBankCode",
                table: "BankAccounts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NationalCheckDigit",
                table: "BankAccounts",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountNumber",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "BranchCode",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "CheckNumber",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "CountryCode",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "Iban",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "NationalBankCode",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "NationalCheckDigit",
                table: "BankAccounts");

            migrationBuilder.AddColumn<string>(
                name: "Number",
                table: "BankAccounts",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
