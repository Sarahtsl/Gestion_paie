using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gestion_paie.Migrations
{
    /// <inheritdoc />
    public partial class AddPayrollTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "PayrollPeriods",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateTable(
                name: "Payrolls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    PeriodId = table.Column<int>(type: "int", nullable: false),
                    BaseSalary = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    WorkedDays = table.Column<int>(type: "int", nullable: false),
                    WorkingDays = table.Column<int>(type: "int", nullable: false),
                    OvertimeHours = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    OvertimeAmount = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    BonusAmount = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    CommissionAmount = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    BenefitsInKind = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    CnssFamilyAllowance = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    CnssAmo = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    CnssAccident = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    CnssRetirement = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    EmployerFamilyAllowance = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    EmployerAmo = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    EmployerAccident = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    EmployerRetirement = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    GrossSalary = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    TaxableIncome = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    IncomeTax = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    NetSalary = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payrolls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payrolls_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payrolls_PayrollPeriods_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "PayrollPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payrolls_EmployeeId_PeriodId",
                table: "Payrolls",
                columns: new[] { "EmployeeId", "PeriodId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payrolls_PeriodId",
                table: "Payrolls",
                column: "PeriodId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payrolls");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "PayrollPeriods",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");
        }
    }
}
