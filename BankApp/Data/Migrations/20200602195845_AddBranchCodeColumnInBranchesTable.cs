using Microsoft.EntityFrameworkCore.Migrations;

namespace BankApp.Data.Migrations
{
    public partial class AddBranchCodeColumnInBranchesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BranchCode",
                table: "Branches",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BranchCode",
                table: "Branches");
        }
    }
}
