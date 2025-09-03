using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gestion_paie.Models
{
    public class TaxBracket
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime EffectiveDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(12,2)")]
        [Range(0, 9999999999.99, ErrorMessage = "MinAmount doit être positif et inférieur à 10 milliards.")]
        public decimal MinAmount { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        [Range(0, 9999999999.99, ErrorMessage = "MaxAmount doit être positif et inférieur à 10 milliards.")]
        public decimal? MaxAmount { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,4)")]
        [Range(0, 100, ErrorMessage = "TaxRate doit être entre 0 et 100.")]
        public decimal TaxRate { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        [Range(0, 9999999999.99, ErrorMessage = "FixedAmount doit être positif.")]
        public decimal FixedAmount { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
