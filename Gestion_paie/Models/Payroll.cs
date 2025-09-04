using GestionPaie.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gestion_paie.Models
{
    public enum PayrollStatus { DRAFT, CALCULATED, VALIDATED, PAID }

    public class Payroll
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Employee))]
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        [Required]
        [ForeignKey(nameof(PayrollPeriod))]
        public int PeriodId { get; set; }
        public PayrollPeriod PayrollPeriod { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal BaseSalary { get; set; }

        public int WorkedDays { get; set; } = 0;
        public int WorkingDays { get; set; } = 0;

        [Column(TypeName = "decimal(5,2)")]
        public decimal OvertimeHours { get; set; } = 0;

        [Column(TypeName = "decimal(12,2)")]
        public decimal OvertimeAmount { get; set; } = 0;

        [Column(TypeName = "decimal(12,2)")]
        public decimal BonusAmount { get; set; } = 0;

        [Column(TypeName = "decimal(12,2)")]
        public decimal CommissionAmount { get; set; } = 0;

        [Column(TypeName = "decimal(12,2)")]
        public decimal BenefitsInKind { get; set; } = 0;

        [Column(TypeName = "decimal(12,2)")]
        public decimal CnssFamilyAllowance { get; set; } = 0;

        [Column(TypeName = "decimal(12,2)")]
        public decimal CnssAmo { get; set; } = 0;

        [Column(TypeName = "decimal(12,2)")]
        public decimal CnssAccident { get; set; } = 0;

        [Column(TypeName = "decimal(12,2)")]
        public decimal CnssRetirement { get; set; } = 0;

        [Column(TypeName = "decimal(12,2)")]
        public decimal EmployerFamilyAllowance { get; set; } = 0;

        [Column(TypeName = "decimal(12,2)")]
        public decimal EmployerAmo { get; set; } = 0;

        [Column(TypeName = "decimal(12,2)")]
        public decimal EmployerAccident { get; set; } = 0;

        [Column(TypeName = "decimal(12,2)")]
        public decimal EmployerRetirement { get; set; } = 0;

        [Column(TypeName = "decimal(12,2)")]
        public decimal GrossSalary { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal TaxableIncome { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal IncomeTax { get; set; } = 0;

        [Column(TypeName = "decimal(12,2)")]
        public decimal NetSalary { get; set; }

        public PayrollStatus Status { get; set; } = PayrollStatus.DRAFT;

        public ICollection<PayrollItem> PayrollItems { get; set; } = new List<PayrollItem>();

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
  
        public ICollection<BenefitInKind> Benefits { get; set; } = new List<BenefitInKind>();
    }
}
