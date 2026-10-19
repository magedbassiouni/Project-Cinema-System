using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project_Cinema_System.Models;

namespace Project_Cinema_System.Configurations
{
    public class CinemaConfiguration : IEntityTypeConfiguration<Cinema>
    {
            public void Configure(EntityTypeBuilder<Cinema> builder)
            {
                builder.ToTable("Cinemas");

                builder.HasKey(c => c.Id);

                builder.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                builder.Property(c => c.Description)
                    .HasMaxLength(500);

                builder.Property(c => c.Address)
                    .IsRequired()
                    .HasMaxLength(250);

                builder.Property(c => c.Img)
                    .HasMaxLength(500);

                builder.HasMany(c => c.Showtimes)
                    .WithOne(s => s.Cinema)
                    .HasForeignKey(s => s.CinemaId);

            }
        
    }
}
