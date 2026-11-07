namespace Project_Cinema_System.ViewModels
{
    public class MovieDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CategoryName { get; set; }
        public string? CinemaName { get; set; }
        public List<string> ActorNames { get; set; } = new();
        public List<string> ImageUrls { get; set; } = new();
        public List<ShowtimeItem> Showtimes { get; set; } = new();
    }

    public class ShowtimeItem
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Price { get; set; }
        public string CinemaName { get; set; } = string.Empty;
    }
}