using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project_Cinema_System.Models;

namespace Project_Cinema_System.Configurations
{
    public class MovieConfiguration:IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            builder.ToTable("Movies");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.Description)
                .HasMaxLength(2000);

            builder.Property(m => m.Status)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(m => m.Price)
                .HasPrecision(10, 2);

            builder.Property(m => m.DateTime);

            builder.HasOne(m => m.Cinema)
                .WithMany()
                .HasForeignKey(m => m.CinemaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(m => m.MainImg)
                .HasMaxLength(500);

            builder.HasOne(m => m.Category)
                .WithMany(c => c.Movies)
                .HasForeignKey(m => m.CategoryId);
        }
    }
}
