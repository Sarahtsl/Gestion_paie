using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gestion_paie.Migrations
{
    /// <inheritdoc />
    public partial class CreateCnssRatesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CnssRates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FamilyAllowanceRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AmoRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AccidentRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RetirementRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EmployerFamilyRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EmployerAmoRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EmployerAccidentRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EmployerRetirementRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CnssRates", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CnssRates");
        }
    }
}
