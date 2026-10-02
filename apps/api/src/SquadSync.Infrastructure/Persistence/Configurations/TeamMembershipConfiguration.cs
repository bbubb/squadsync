using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SquadSync.Domain;

namespace SquadSync.Infrastructure.Persistence.Configurations;

internal sealed class TeamMembershipConfiguration : IEntityTypeConfiguration<TeamMembership>
{
    public void Configure(EntityTypeBuilder<TeamMembership> builder)
    {
        builder.HasKey(membership => membership.Id);
        builder.Property(membership => membership.Id)
            .ValueGeneratedNever();
        builder.Property(membership => membership.UserId)
            .IsRequired();
        builder.Property(membership => membership.TeamId)
            .IsRequired();
        builder.Property(membership => membership.TeamRole)
            .HasConversion<string>()
            .IsRequired();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(membership => membership.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Team>()
            .WithMany()
            .HasForeignKey(membership => membership.TeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(membership => new { membership.UserId, membership.TeamId })
            .IsUnique();
    }
}
