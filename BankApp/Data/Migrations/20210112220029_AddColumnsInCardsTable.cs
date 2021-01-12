using Microsoft.EntityFrameworkCore.Migrations;

namespace BankApp.Data.Migrations
{
    public partial class AddColumnsInCardsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AccountIdentificationNumber",
                table: "Cards",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<byte>(
                name: "CheckDigit",
                table: "Cards",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "IssuerIdentificationNumber",
                table: "Cards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte>(
                name: "MajorIndustryIdentifier",
                table: "Cards",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountIdentificationNumber",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "CheckDigit",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "IssuerIdentificationNumber",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "MajorIndustryIdentifier",
                table: "Cards");
        }
    }
}
