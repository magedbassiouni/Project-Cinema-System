using System.ComponentModel.DataAnnotations.Schema;

namespace Project_Cinema_System.Models
{
    public class Cart
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public ApplicationUser User { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }

    public class CartItem
    {
        public int Id { get; set; }

        public int CartId { get; set; }

        public Cart Cart { get; set; } = null!;

        public int ShowtimeId { get; set; }

        public Showtime Showtime { get; set; } = null!;

        public int Quantity { get; set; }
    }

    public class Payment
    {
        public int Id { get; set; }

        public int BookingId { get; set; }

        public Booking Booking { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        // Cash / Online
        public string Method { get; set; } = string.Empty;

        // Pending / Paid / Failed / Refunded
        public string Status { get; set; } = "Pending";

        public string TransactionReference { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? PaidAt { get; set; }
    }
}