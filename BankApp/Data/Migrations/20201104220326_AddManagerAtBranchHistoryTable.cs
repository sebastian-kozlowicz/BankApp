using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BankApp.Data.Migrations
{
    public partial class AddManagerAtBranchHistoryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WorkAtId",
                table: "Managers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ManagerAtBranchHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssignDate = table.Column<DateTime>(nullable: false),
                    BranchId = table.Column<int>(nullable: false),
                    ManagerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagerAtBranchHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManagerAtBranchHistory_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ManagerAtBranchHistory_Managers_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Managers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Managers_WorkAtId",
                table: "Managers",
                column: "WorkAtId");

            migrationBuilder.CreateIndex(
                name: "IX_ManagerAtBranchHistory_BranchId",
                table: "ManagerAtBranchHistory",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_ManagerAtBranchHistory_ManagerId",
                table: "ManagerAtBranchHistory",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Managers_Branches_WorkAtId",
                table: "Managers",
                column: "WorkAtId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Managers_Branches_WorkAtId",
                table: "Managers");

            migrationBuilder.DropTable(
                name: "ManagerAtBranchHistory");

            migrationBuilder.DropIndex(
                name: "IX_Managers_WorkAtId",
                table: "Managers");

            migrationBuilder.DropColumn(
                name: "WorkAtId",
                table: "Managers");
        }
    }
}
