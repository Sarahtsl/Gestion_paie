using System.ComponentModel.DataAnnotations;

namespace Gestion_paie.ViewModels
{
    public class EditUserViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public int RoleId { get; set; }
    }

}
