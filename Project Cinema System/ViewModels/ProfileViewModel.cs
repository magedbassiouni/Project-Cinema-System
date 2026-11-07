
using System.ComponentModel.DataAnnotations;

namespace Project_Cinema_System.ViewModels
{
    public class ProfileViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
