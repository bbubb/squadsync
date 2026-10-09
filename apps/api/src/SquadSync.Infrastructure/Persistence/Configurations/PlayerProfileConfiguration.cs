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
        builder.Property(profile => profile.PersonId)
            .IsRequired();
        builder.Property(profile => profile.DominantFoot)
            .HasConversion<string>();
        builder.Property(profile => profile.HeightInches)
            .HasColumnType("integer");
        // The Domain accepts decimal values without a fixed scale; preserve their precision.
        builder.Property(profile => profile.WeightPounds)
            .HasColumnType("numeric");

        builder.HasOne<Person>()
            .WithOne()
            .HasForeignKey<PlayerProfile>(profile => profile.PersonId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(profile => profile.PersonId)
            .IsUnique();
    }
}
