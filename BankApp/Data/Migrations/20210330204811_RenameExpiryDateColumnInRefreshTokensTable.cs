using Microsoft.EntityFrameworkCore.Migrations;

namespace BankApp.Data.Migrations
{
    public partial class RenameExpiryDateColumnInRefreshTokensTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpiryDate",
                table: "RefreshTokens",
                newName: "ExpirationDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpirationDate",
                table: "RefreshTokens",
                newName: "ExpiryDate");
        }
    }
}
