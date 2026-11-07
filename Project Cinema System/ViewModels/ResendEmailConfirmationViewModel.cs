using System.ComponentModel.DataAnnotations;

namespace Project_Cinema_System.ViewModels
{
    public class ResendEmailConfirmationViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}