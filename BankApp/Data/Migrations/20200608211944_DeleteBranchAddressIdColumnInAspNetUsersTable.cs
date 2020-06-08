using Microsoft.EntityFrameworkCore.Migrations;

namespace BankApp.Data.Migrations
{
    public partial class DeleteBranchAddressIdColumnInAspNetUsersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_BranchAddresses_BranchAddressId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_BranchAddressId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BranchAddressId",
                table: "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BranchAddressId",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_BranchAddressId",
                table: "AspNetUsers",
                column: "BranchAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_BranchAddresses_BranchAddressId",
                table: "AspNetUsers",
                column: "BranchAddressId",
                principalTable: "BranchAddresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
