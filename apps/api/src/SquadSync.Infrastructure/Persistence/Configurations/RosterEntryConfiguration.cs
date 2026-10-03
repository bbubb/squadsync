using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SquadSync.Domain;

namespace SquadSync.Infrastructure.Persistence.Configurations;

internal sealed class RosterEntryConfiguration : IEntityTypeConfiguration<RosterEntry>
{
    public void Configure(EntityTypeBuilder<RosterEntry> builder)
    {
        builder.HasKey(entry => entry.Id);
        builder.Property(entry => entry.Id)
            .ValueGeneratedNever();
        builder.Property(entry => entry.TeamMembershipId)
            .IsRequired();
        builder.Property(entry => entry.JerseyNumber)
            .HasMaxLength(3);
        builder.Property(entry => entry.RosterStatus)
            .HasConversion<string>()
            .IsRequired();

        builder.HasOne<TeamMembership>()
            .WithOne()
            .HasForeignKey<RosterEntry>(entry => entry.TeamMembershipId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(entry => entry.TeamMembershipId)
            .IsUnique();
    }
}
