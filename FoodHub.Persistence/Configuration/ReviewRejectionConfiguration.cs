using FoodHub.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodHub.Persistence.Configuration;

public class ReviewRejectionConfiguration : IEntityTypeConfiguration<ReviewRejection>
{
    public void Configure(EntityTypeBuilder<ReviewRejection> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.TargetName)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(r => r.Reason)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(r => r.TargetOwnerUserId)
            .IsRequired();

        builder.Property(r => r.RejectedByUserId)
            .IsRequired();

        // The review screen looks up "has this been rejected before?" by target, and the
        // future notification feature will look up "what was rejected for me?" by owner.
        builder.HasIndex(r => new
        {
            r.TargetType,
            r.TargetId
        });

        builder.HasIndex(r => r.TargetOwnerUserId);
    }
}
