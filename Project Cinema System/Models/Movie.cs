namespace Project_Cinema_System.Models
{
    public class Movie
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Status { get; set; } = string.Empty;

        public string? MainImg { get; set; }

        public int CategoryId { get; set; }

        public Category? Category { get; set; }


        public decimal Price { get; set; }

        public DateTime DateTime { get; set; }

        public int CinemaId { get; set; }

        public Cinema? Cinema { get; set; }

        public ICollection<MovieActor> MovieActors { get; set; }
            = new List<MovieActor>();

        public ICollection<MovieImage> MovieImages { get; set; }
            = new List<MovieImage>();

        public ICollection<Showtime> Showtimes { get; set; }
            = new List<Showtime>();
    }
}
