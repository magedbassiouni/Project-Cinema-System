using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project_Cinema_System.Models;

namespace Project_Cinema_System.Configurations
{
    public class ActorConfiguration : IEntityTypeConfiguration<Actor>
    {
        public void Configure(EntityTypeBuilder<Actor> builder)
        {
            builder.ToTable("Actors");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.Img)
                .HasMaxLength(500);

            builder.Property(a => a.Biography)
              .HasMaxLength(1000);

            builder.HasMany(a => a.MovieActors)
                .WithOne(a => a.Actor)
                .HasForeignKey(a => a.ActorId);

        }
    }
}
