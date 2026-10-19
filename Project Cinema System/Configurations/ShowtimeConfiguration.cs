using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project_Cinema_System.Models;

namespace Project_Cinema_System.Configurations
{
    public class ShowtimeConfiguration : IEntityTypeConfiguration<Showtime>
    {
        public void Configure(EntityTypeBuilder<Showtime> builder)
        {
            builder.ToTable("Showtimes");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Price)
                .HasPrecision(10, 2)
                .IsRequired();

            builder.Property(s => s.TotalTickets)
                .IsRequired();

            builder.Property(s => s.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasOne(s => s.Movie)
                .WithMany(m => m.Showtimes)
                .HasForeignKey(s => s.MovieId);

            builder.HasOne(s => s.Cinema)
                .WithMany(c => c.Showtimes)
                .HasForeignKey(s => s.CinemaId);
        }
    }
}
