using Microsoft.EntityFrameworkCore.Migrations;

namespace BankApp.Data.Migrations
{
    public partial class RenamePostCodeToPostalCodeColumnInAddressesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostCode",
                table: "Addresses");

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "Addresses",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Addresses");

            migrationBuilder.AddColumn<string>(
                name: "PostCode",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
