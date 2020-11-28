using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BankApp.Data.Migrations
{
    public partial class RenameEmployeesToTellersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeAtBranchHistory");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.CreateTable(
                name: "Tellers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    WorkAtId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tellers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tellers_AspNetUsers_Id",
                        column: x => x.Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tellers_Branches_WorkAtId",
                        column: x => x.WorkAtId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TellerAtBranchHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssignDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpelDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    TellerId = table.Column<int>(type: "int", nullable: false),
                    AssignedById = table.Column<int>(type: "int", nullable: false),
                    ExpelledById = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TellerAtBranchHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TellerAtBranchHistory_AspNetUsers_AssignedById",
                        column: x => x.AssignedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TellerAtBranchHistory_AspNetUsers_ExpelledById",
                        column: x => x.ExpelledById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TellerAtBranchHistory_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TellerAtBranchHistory_Tellers_TellerId",
                        column: x => x.TellerId,
                        principalTable: "Tellers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TellerAtBranchHistory_AssignedById",
                table: "TellerAtBranchHistory",
                column: "AssignedById");

            migrationBuilder.CreateIndex(
                name: "IX_TellerAtBranchHistory_BranchId",
                table: "TellerAtBranchHistory",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_TellerAtBranchHistory_ExpelledById",
                table: "TellerAtBranchHistory",
                column: "ExpelledById");

            migrationBuilder.CreateIndex(
                name: "IX_TellerAtBranchHistory_TellerId",
                table: "TellerAtBranchHistory",
                column: "TellerId");

            migrationBuilder.CreateIndex(
                name: "IX_Tellers_WorkAtId",
                table: "Tellers",
                column: "WorkAtId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TellerAtBranchHistory");

            migrationBuilder.DropTable(
                name: "Tellers");

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    WorkAtId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_AspNetUsers_Id",
                        column: x => x.Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employees_Branches_WorkAtId",
                        column: x => x.WorkAtId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeAtBranchHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssignDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AssignedById = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    ExpelDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpelledById = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeAtBranchHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeAtBranchHistory_AspNetUsers_AssignedById",
                        column: x => x.AssignedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeAtBranchHistory_AspNetUsers_ExpelledById",
                        column: x => x.ExpelledById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeAtBranchHistory_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeAtBranchHistory_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAtBranchHistory_AssignedById",
                table: "EmployeeAtBranchHistory",
                column: "AssignedById");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAtBranchHistory_BranchId",
                table: "EmployeeAtBranchHistory",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAtBranchHistory_EmployeeId",
                table: "EmployeeAtBranchHistory",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeAtBranchHistory_ExpelledById",
                table: "EmployeeAtBranchHistory",
                column: "ExpelledById");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_WorkAtId",
                table: "Employees",
                column: "WorkAtId");
        }
    }
}
