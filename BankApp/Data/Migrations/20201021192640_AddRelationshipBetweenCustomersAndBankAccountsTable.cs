using Microsoft.EntityFrameworkCore.Migrations;

namespace BankApp.Data.Migrations
{
    public partial class AddRelationshipBetweenCustomersAndBankAccountsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_AspNetUsers_ApplicationUserId",
                table: "BankAccounts");

            migrationBuilder.DropIndex(
                name: "IX_BankAccounts_ApplicationUserId",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "BankAccounts");

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "BankAccounts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_CustomerId",
                table: "BankAccounts",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_Customers_CustomerId",
                table: "BankAccounts",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_Customers_CustomerId",
                table: "BankAccounts");

            migrationBuilder.DropIndex(
                name: "IX_BankAccounts_CustomerId",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "BankAccounts");

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "BankAccounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_ApplicationUserId",
                table: "BankAccounts",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_AspNetUsers_ApplicationUserId",
                table: "BankAccounts",
                column: "CustomerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
