using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gestion_paie.Models
{
    public class Company
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string RegistrationNumber { get; set; }

        [MaxLength(20)]
        public string CNSSNumber { get; set; }

        [MaxLength(20)]
        public string TaxNumber { get; set; }

        [MaxLength(50)]
        public string SectorType { get; set; } 

        public string Address { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }

        [MaxLength(100)]
        public string Email { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public ICollection<Employee> Employees { get; set; }
    }
}
