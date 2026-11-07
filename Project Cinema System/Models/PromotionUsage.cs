
namespace Project_Cinema_System.Models
{
    public class PromotionUsage
    {
        public int Id { get; set; }

        public int PromotionId { get; set; }

        public Promotion Promotion { get; set; } = null!;

        public int BookingId { get; set; }

        public Booking Booking { get; set; } = null!;

        public string UserId { get; set; } = string.Empty;

        public ApplicationUser User { get; set; } = null!;

        public decimal DiscountAmount { get; set; }

        public DateTime UsedAt { get; set; }
    }
}
