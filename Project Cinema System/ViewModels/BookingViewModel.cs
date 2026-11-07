
using System.ComponentModel.DataAnnotations;

namespace Project_Cinema_System.ViewModels
{
    public class BookingViewModel
    {
        public int ShowtimeId { get; set; }

        [Required]
        [Range(1, 10, ErrorMessage = "You can book between 1 and 10 tickets.")]
        public int Quantity { get; set; } = 1;

        public string? PromotionCode { get; set; }

        public string MovieName { get; set; } = string.Empty;

        public string CinemaName { get; set; } = string.Empty;

        public DateTime ShowtimeDate { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal FinalAmount { get; set; }
    }
}
