namespace Project_Cinema_System.ViewModels
{
    public class ShowtimeListItem
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public string MovieName { get; set; } = string.Empty;
        public string CinemaName { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public decimal Price { get; set; }
        public int TotalTickets { get; set; }
        public int AvailableTickets { get; set; }
        public bool IsSoldOut => AvailableTickets <= 0;
    }
}
