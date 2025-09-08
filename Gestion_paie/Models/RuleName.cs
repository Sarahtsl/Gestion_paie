using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gestion_paie.Models
{
    // Table de noms de règles (équivalent de rule_name)
    public class RuleName
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = null!;
    }

    public enum RuleType { Threshold, Comparison, Trend, Outlier }
    public enum Severity { LOW, MEDIUM, HIGH, CRITICAL }

    public class AnomalyRule
    {
        public int Id { get; set; }

        // FK vers RuleName
        [Required]
        public int RuleNameId { get; set; }
        public RuleName? RuleName { get; set; }

        // rule_type VARCHAR(50)
        [Required]
        public RuleType RuleType { get; set; }

        public string? Description { get; set; }

        // CONDITIONS → string pour SQL Server
        [Column(TypeName = "nvarchar(max)")]
        public string? Conditions { get; set; }   // JSON sérialisé en texte

        // severity (LOW, MEDIUM, HIGH, CRITICAL)
        [Required]
        public Severity Severity { get; set; }

        public bool IsActive { get; set; } = true;

        // default GETDATE() sera configuré en Fluent API
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }
    }
}
