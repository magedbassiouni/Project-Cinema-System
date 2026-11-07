
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project_Cinema_System.Models;

namespace Project_Cinema_System.Configurations
{
    public class PromotionConfiguration
        : IEntityTypeConfiguration<Promotion>
    {
        public void Configure(EntityTypeBuilder<Promotion> builder)
        {
            builder.ToTable("Promotions");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(p => p.Code)
                .IsUnique();

            builder.Property(p => p.DiscountPercentage)
                .HasPrecision(5, 2)
                .IsRequired();

            builder.Property(p => p.StartDate)
                .IsRequired();

            builder.Property(p => p.EndDate)
                .IsRequired();

            builder.Property(p => p.MaxUsage)
                .IsRequired();

            builder.Property(p => p.UsageCount)
                .HasDefaultValue(0)
                .IsRequired();

            builder.Property(p => p.IsActive)
                .HasDefaultValue(true)
                .IsRequired();
        }
    }
}
