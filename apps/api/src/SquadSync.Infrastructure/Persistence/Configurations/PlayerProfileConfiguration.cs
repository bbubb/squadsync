using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SquadSync.Domain;

namespace SquadSync.Infrastructure.Persistence.Configurations;

internal sealed class PlayerProfileConfiguration : IEntityTypeConfiguration<PlayerProfile>
{
    public void Configure(EntityTypeBuilder<PlayerProfile> builder)
    {
        builder.HasKey(profile => profile.Id);
        builder.Property(profile => profile.Id)
            .ValueGeneratedNever();
        builder.Property(profile => profile.UserId)
            .IsRequired();
        builder.Property(profile => profile.DominantFoot)
            .HasConversion<string>();
        builder.Property(profile => profile.HeightInches)
            .HasColumnType("integer");
        // The Domain accepts decimal values without a fixed scale; preserve their precision.
        builder.Property(profile => profile.WeightPounds)
            .HasColumnType("numeric");

        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<PlayerProfile>(profile => profile.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(profile => profile.UserId)
            .IsUnique();
    }
}
