using System;
using System.ComponentModel.DataAnnotations;

namespace Gestion_paie.Models
{
    public class CnssRate
    {
        public int Id { get; set; }

        [Required]
        public DateTime EffectiveDate { get; set; }

        [Display(Name = "Taux Prestations Familiales")]
        public decimal? FamilyAllowanceRate { get; set; }

        [Display(Name = "Taux AMO")]
        public decimal? AmoRate { get; set; }

        [Display(Name = "Taux Accident de Travail / Maladies Professionnelles")]
        public decimal? AccidentRate { get; set; }

        [Display(Name = "Taux Retraite")]
        public decimal? RetirementRate { get; set; }

        [Display(Name = "Part Patronale - Prestations Familiales")]
        public decimal? EmployerFamilyRate { get; set; }

        [Display(Name = "Part Patronale - AMO")]
        public decimal? EmployerAmoRate { get; set; }

        [Display(Name = "Part Patronale - AT/MP")]
        public decimal? EmployerAccidentRate { get; set; }

        [Display(Name = "Part Patronale - Retraite")]
        public decimal? EmployerRetirementRate { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
