
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project_Cinema_System.Models;

namespace Project_Cinema_System.Configurations
{
    public class PromotionUsageConfiguration
        : IEntityTypeConfiguration<PromotionUsage>
    {
        public void Configure(EntityTypeBuilder<PromotionUsage> builder)
        {
            builder.ToTable("PromotionUsages");

            builder.HasKey(pu => pu.Id);

            builder.Property(pu => pu.DiscountAmount)
                .HasPrecision(10, 2)
                .IsRequired();

            builder.Property(pu => pu.UsedAt)
                .IsRequired();

            builder.Property(pu => pu.UserId)
                .IsRequired();

            builder.HasOne(pu => pu.Promotion)
                .WithMany(p => p.PromotionUsages)
                .HasForeignKey(pu => pu.PromotionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pu => pu.Booking)
                .WithMany()
                .HasForeignKey(pu => pu.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pu => pu.User)
                .WithMany()
                .HasForeignKey(pu => pu.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(pu => pu.BookingId)
                .IsUnique();
        }
    }
}
