using Microsoft.EntityFrameworkCore.Migrations;

namespace BankApp.Data.Migrations
{
    public partial class AddRelationshipBetweenBranchesAndEmployeesTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WorkAtId",
                table: "Employees",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_WorkAtId",
                table: "Employees",
                column: "WorkAtId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Branches_WorkAtId",
                table: "Employees",
                column: "WorkAtId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Branches_WorkAtId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_WorkAtId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "WorkAtId",
                table: "Employees");
        }
    }
}
