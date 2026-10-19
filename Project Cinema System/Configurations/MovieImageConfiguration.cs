using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Project_Cinema_System.Models;

namespace Project_Cinema_System.Configurations
{
    public class MovieImageConfiguration : IEntityTypeConfiguration<MovieImage>
    {
        public void Configure(EntityTypeBuilder<MovieImage> builder)
        {
            builder.ToTable("MovieImage");

            builder.HasKey(mi => mi.Id);

            builder.Property(mi=>mi.ImageUrl)
                .IsRequired()
                .HasMaxLength(500);
            builder.HasOne(mi => mi.Movie)
                .WithMany(m => m.MovieImages)
                .HasForeignKey(mi => mi.MovieId);

        }
    }
}
