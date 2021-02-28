using Microsoft.EntityFrameworkCore.Migrations;

namespace BankApp.Data.Migrations
{
    public partial class AddColumnsInPaymentCardsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountIdentificationNumberText",
                table: "PaymentCards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IssuingNetwork",
                table: "PaymentCards",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountIdentificationNumberText",
                table: "PaymentCards");

            migrationBuilder.DropColumn(
                name: "IssuingNetwork",
                table: "PaymentCards");
        }
    }
}
