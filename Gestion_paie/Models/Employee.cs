using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Gestion_paie.Models
{
    public class Employee
    {
        public int Id { get; set; }
        
        public int? UserId { get; set; }
        [ValidateNever]
        public User User { get; set; }
        
        public int? CompanyId { get; set; }
        [ValidateNever]
        public Company Company { get; set; } 

        [MaxLength(50)]
        public string? EmployeeNumber { get; set; }

        [MaxLength(20)]
        [Remote(action: "VerifyCNSS", controller: "RH", AdditionalFields = nameof(Id),
        ErrorMessage = "Ce numéro CNSS est déjà utilisé")]
        public string? CNSSNumber { get; set; }

        [MaxLength(20)]
        [Remote(action: "VerifyCIN", controller: "RH", AdditionalFields = nameof(Id),
        ErrorMessage = "Ce CIN est déjà utilisé")]
        public string? CIN { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        public DateTime? BirthDate { get; set; }

        [Required]
        public DateTime HireDate { get; set; }

        [MaxLength(20)]
        public string? ContractType { get; set; }

        [MaxLength(100)]
        public string? Position { get; set; }

        [MaxLength(100)]
        public string? Department { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal BaseSalary { get; set; }

        [MaxLength(30)]
        [Remote(action: "VerifyBankAccount", controller: "RH", AdditionalFields = nameof(Id),
                ErrorMessage = "Ce compte bancaire est déjà utilisé")]
        public string? BankAccount { get; set; }

        [MaxLength(10)]
        public string? BankCode { get; set; }

        [MaxLength(20)]
        public string? MaritalStatus { get; set; }

        public int DependentsCount { get; set; } = 0;

        [MaxLength(20)]
        public string Status { get; set; } = "ACTIVE"; 

        public DateTime? TerminationDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
