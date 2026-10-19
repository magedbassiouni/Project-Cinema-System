namespace Project_Cinema_System.Models
{
    public class Actor
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Img { get; set; }

        public string? Biography { get; set; }

        public ICollection<MovieActor> MovieActors { get; set; }
            = new List<MovieActor>();
    }
}
