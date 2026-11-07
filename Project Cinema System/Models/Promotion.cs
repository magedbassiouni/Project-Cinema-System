
namespace Project_Cinema_System.Models
{
    public class Promotion
    {
        public int Id { get; set; }

        public string Code { get; set; } = string.Empty;

        public decimal DiscountPercentage { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int MaxUsage { get; set; }

        public int UsageCount { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<PromotionUsage> PromotionUsages { get; set; }
            = new List<PromotionUsage>();
    }
}
