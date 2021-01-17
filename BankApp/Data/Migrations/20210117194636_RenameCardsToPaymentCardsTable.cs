using Microsoft.EntityFrameworkCore.Migrations;

namespace BankApp.Data.Migrations
{
    public partial class RenameCardsToPaymentCardsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.CreateTable(
                name: "PaymentCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MajorIndustryIdentifier = table.Column<byte>(type: "tinyint", nullable: false),
                    IssuerIdentificationNumber = table.Column<int>(type: "int", nullable: false),
                    AccountIdentificationNumber = table.Column<long>(type: "bigint", nullable: false),
                    CheckDigit = table.Column<byte>(type: "tinyint", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankAccountId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentCards_BankAccounts_BankAccountId",
                        column: x => x.BankAccountId,
                        principalTable: "BankAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentCards_BankAccountId",
                table: "PaymentCards",
                column: "BankAccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentCards");

            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountIdentificationNumber = table.Column<long>(type: "bigint", nullable: false),
                    BankAccountId = table.Column<int>(type: "int", nullable: false),
                    CheckDigit = table.Column<byte>(type: "tinyint", nullable: false),
                    IssuerIdentificationNumber = table.Column<int>(type: "int", nullable: false),
                    MajorIndustryIdentifier = table.Column<byte>(type: "tinyint", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cards_BankAccounts_BankAccountId",
                        column: x => x.BankAccountId,
                        principalTable: "BankAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cards_BankAccountId",
                table: "Cards",
                column: "BankAccountId");
        }
    }
}
