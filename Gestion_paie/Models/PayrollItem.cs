using Gestion_paie.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionPaie.Models
{
    public enum PayrollItemType
    {
        EARNING,
        DEDUCTION
    }

    [Table("payroll_items")]
    public class PayrollItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PayrollId { get; set; }
        public Payroll Payroll { get; set; }

        [Required]
        [Column("item_type")]
        public PayrollItemType ItemType { get; set; } 

        [Required]
        [MaxLength(50)]
        public string Code { get; set; }  

        [MaxLength(200)]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal Amount { get; set; }

        public bool IsTaxable { get; set; } = true;

        public bool IsCnssSubject { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
