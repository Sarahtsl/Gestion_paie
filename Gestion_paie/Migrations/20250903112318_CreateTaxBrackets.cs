using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gestion_paie.Migrations
{
    /// <inheritdoc />
    public partial class CreateTaxBrackets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaxBrackets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MinAmount = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    MaxAmount = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    TaxRate = table.Column<decimal>(type: "decimal(5,4)", nullable: false),
                    FixedAmount = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxBrackets", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaxBrackets");
        }
    }
}
