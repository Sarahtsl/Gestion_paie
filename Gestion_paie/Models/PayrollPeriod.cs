using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gestion_paie.Models
{
    public enum PayrollPeriodStatus { DRAFT, PROCESSING, COMPLETED, LOCKED }

    public class PayrollPeriod
    {
        public int Id { get; set; }

        public int? CompanyId { get; set; }
        [ValidateNever] public Company? Company { get; set; }

        [Required] public int PeriodYear { get; set; }
        [Required, Range(1, 12)] public int PeriodMonth { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required] public PayrollPeriodStatus Status { get; set; } = PayrollPeriodStatus.DRAFT;

        public DateTime? ProcessedAt { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }
    }
}
