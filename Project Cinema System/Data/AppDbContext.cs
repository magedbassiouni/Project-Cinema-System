
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Project_Cinema_System.Configurations;
using Project_Cinema_System.Models;

namespace Project_Cinema_System.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(
            DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; } = null!;

        public DbSet<Cinema> Cinemas { get; set; } = null!;

        public DbSet<Movie> Movies { get; set; } = null!;

        public DbSet<Actor> Actors { get; set; } = null!;

        public DbSet<MovieActor> MovieActors { get; set; } = null!;

        public DbSet<MovieImage> MovieImages { get; set; } = null!;

        public DbSet<Showtime> Showtimes { get; set; } = null!;

        public DbSet<Booking> Bookings { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new CategoryConfiguration());
            builder.ApplyConfiguration(new CinemaConfiguration());
            builder.ApplyConfiguration(new ActorConfiguration());
            builder.ApplyConfiguration(new MovieActorConfiguration());
            builder.ApplyConfiguration(new MovieImageConfiguration());
            builder.ApplyConfiguration(new MovieConfiguration());
            builder.ApplyConfiguration(new ShowtimeConfiguration());
            builder.ApplyConfiguration(new BookingConfiguration());
        }
    }
}
