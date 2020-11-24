using Microsoft.EntityFrameworkCore.Migrations;

namespace BankApp.Data.Migrations
{
    public partial class AddAssignedByAndExpelledByToMangerAtBranchHistoryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssignedById",
                table: "ManagerAtBranchHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ExpelledById",
                table: "ManagerAtBranchHistory",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ManagerAtBranchHistory_AssignedById",
                table: "ManagerAtBranchHistory",
                column: "AssignedById");

            migrationBuilder.CreateIndex(
                name: "IX_ManagerAtBranchHistory_ExpelledById",
                table: "ManagerAtBranchHistory",
                column: "ExpelledById");

            migrationBuilder.AddForeignKey(
                name: "FK_ManagerAtBranchHistory_AspNetUsers_AssignedById",
                table: "ManagerAtBranchHistory",
                column: "AssignedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ManagerAtBranchHistory_AspNetUsers_ExpelledById",
                table: "ManagerAtBranchHistory",
                column: "ExpelledById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ManagerAtBranchHistory_AspNetUsers_AssignedById",
                table: "ManagerAtBranchHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_ManagerAtBranchHistory_AspNetUsers_ExpelledById",
                table: "ManagerAtBranchHistory");

            migrationBuilder.DropIndex(
                name: "IX_ManagerAtBranchHistory_AssignedById",
                table: "ManagerAtBranchHistory");

            migrationBuilder.DropIndex(
                name: "IX_ManagerAtBranchHistory_ExpelledById",
                table: "ManagerAtBranchHistory");

            migrationBuilder.DropColumn(
                name: "AssignedById",
                table: "ManagerAtBranchHistory");

            migrationBuilder.DropColumn(
                name: "ExpelledById",
                table: "ManagerAtBranchHistory");
        }
    }
}
