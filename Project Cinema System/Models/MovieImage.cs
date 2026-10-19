namespace Project_Cinema_System.Models
{
    public class MovieImage
    {
        public int Id { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public int MovieId { get; set; }

        public Movie Movie { get; set; } = null!;
    }
}
