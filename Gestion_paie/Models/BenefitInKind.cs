using Gestion_paie.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionPaie.Models
{
    public class BenefitInKind
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("payroll_id")]
        public int PayrollId { get; set; }
        public Payroll Payroll { get; set; }

        [Required]
        public int BenefitTypeId { get; set; }
        public BenefitType BenefitType { get; set; }

        [MaxLength(200)]
        [Column("description")]
        public string Description { get; set; } = "";

        [Required]
        [Column("value", TypeName = "decimal(12,2)")]
        public decimal Value { get; set; }

        [Required]
        [Column("taxable_value", TypeName = "decimal(12,2)")]
        public decimal TaxableValue { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
