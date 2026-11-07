
using System.ComponentModel.DataAnnotations;

namespace Project_Cinema_System.ViewModels.Admin
{
    public class PromotionViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [Range(1, 100)]
        public decimal DiscountPercentage { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int MaxUsage { get; set; }

        public int UsageCount { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
