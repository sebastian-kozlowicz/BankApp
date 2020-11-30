using Microsoft.EntityFrameworkCore.Migrations;

namespace BankApp.Data.Migrations
{
    public partial class AddCreatedByIdColumnInBankAccountsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "BankAccounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_CreatedById",
                table: "BankAccounts",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_AspNetUsers_CreatedById",
                table: "BankAccounts",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_AspNetUsers_CreatedById",
                table: "BankAccounts");

            migrationBuilder.DropIndex(
                name: "IX_BankAccounts_CreatedById",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "BankAccounts");
        }
    }
}
