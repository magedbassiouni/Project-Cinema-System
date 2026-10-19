namespace Project_Cinema_System.Models
{
    public class Showtime
    {
        public int Id { get; set; }

        public int MovieId { get; set; }

        public Movie Movie { get; set; } = null!;

        public int CinemaId { get; set; }

        public Cinema Cinema { get; set; } = null!;

        public DateTime DateTime { get; set; }

        public decimal Price { get; set; }

        public int TotalTickets { get; set; }

        public string Status { get; set; } = "Scheduled";

        public ICollection<Booking> Bookings { get; set; }
            = new List<Booking>();
    }
}
