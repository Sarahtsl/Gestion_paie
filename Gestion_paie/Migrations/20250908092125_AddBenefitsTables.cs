using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gestion_paie.Migrations
{
    /// <inheritdoc />
    public partial class AddBenefitsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BenefitTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BenefitTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BenefitsInKind",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    payroll_id = table.Column<int>(type: "int", nullable: false),
                    BenefitTypeId = table.Column<int>(type: "int", nullable: false),
                    description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    value = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    taxable_value = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BenefitsInKind", x => x.id);
                    table.ForeignKey(
                        name: "FK_BenefitsInKind_BenefitTypes_BenefitTypeId",
                        column: x => x.BenefitTypeId,
                        principalTable: "BenefitTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BenefitsInKind_Payrolls_payroll_id",
                        column: x => x.payroll_id,
                        principalTable: "Payrolls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BenefitsInKind_BenefitTypeId",
                table: "BenefitsInKind",
                column: "BenefitTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BenefitsInKind_payroll_id",
                table: "BenefitsInKind",
                column: "payroll_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BenefitsInKind");

            migrationBuilder.DropTable(
                name: "BenefitTypes");
        }
    }
}
