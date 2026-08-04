using FoodHub.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodHub.Persistence.Configuration;

public class UserPresenceConfiguration : IEntityTypeConfiguration<UserPresence>
{
    public void Configure(EntityTypeBuilder<UserPresence> builder)
    {
        // The account id is the key: one row per user, so a heartbeat is an update, never an insert.
        builder.HasKey(x => x.UserId);

        // Matches the key length Identity uses for AspNetUsers.Id.
        builder.Property(x => x.UserId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(x => x.LastSeenUtc)
            .IsRequired();

        builder.Property(x => x.IsOnline)
            .IsRequired();

        builder.ToTable("UserPresence");
    }
}
