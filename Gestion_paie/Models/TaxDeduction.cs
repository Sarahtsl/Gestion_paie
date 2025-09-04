using System;
using System.ComponentModel.DataAnnotations;

namespace Gestion_paie.Models
{
    public class TaxDeduction
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(50)]
        public string DeductionType { get; set; } // personal, dependent, professional

        [Range(0, double.MaxValue)]
        public decimal? Amount { get; set; }

        [Range(0, 1, ErrorMessage = "Le pourcentage doit être compris entre 0 et 1.")]
        public decimal? Percentage { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? MaxAmount { get; set; }

        [Required]
        public DateTime EffectiveDate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
