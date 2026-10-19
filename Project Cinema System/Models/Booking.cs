namespace Project_Cinema_System.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public ApplicationUser User { get; set; } = null!;

        public int ShowtimeId { get; set; }

        public Showtime Showtime { get; set; } = null!;

        public DateTime BookingDate { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
