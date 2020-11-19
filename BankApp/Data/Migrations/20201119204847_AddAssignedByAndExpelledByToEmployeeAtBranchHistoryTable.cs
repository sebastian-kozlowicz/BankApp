using Microsoft.EntityFrameworkCore.Migrations;

namespace BankApp.Data.Migrations
{
    public partial class AddAssignedByAndExpelledByToEmployeeAtBranchHistoryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssignedById",
                table: "EmployeeAtBranchHistory",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ExpelledById",
                table: "EmployeeAtBranchHistory",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAtBranchHistory_AssignedById",
                table: "EmployeeAtBranchHistory",
                column: "AssignedById");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAtBranchHistory_ExpelledById",
                table: "EmployeeAtBranchHistory",
                column: "ExpelledById");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeAtBranchHistory_AspNetUsers_AssignedById",
                table: "EmployeeAtBranchHistory",
                column: "AssignedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeAtBranchHistory_AspNetUsers_ExpelledById",
                table: "EmployeeAtBranchHistory",
                column: "ExpelledById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeAtBranchHistory_AspNetUsers_AssignedById",
                table: "EmployeeAtBranchHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeAtBranchHistory_AspNetUsers_ExpelledById",
                table: "EmployeeAtBranchHistory");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeAtBranchHistory_AssignedById",
                table: "EmployeeAtBranchHistory");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeAtBranchHistory_ExpelledById",
                table: "EmployeeAtBranchHistory");

            migrationBuilder.DropColumn(
                name: "AssignedById",
                table: "EmployeeAtBranchHistory");

            migrationBuilder.DropColumn(
                name: "ExpelledById",
                table: "EmployeeAtBranchHistory");
        }
    }
}
