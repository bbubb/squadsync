using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SquadSync.Domain;

namespace SquadSync.Infrastructure.Persistence.Configurations;

internal sealed class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.HasKey(team => team.Id);
        builder.Property(team => team.Id)
            .HasField("<Id>k__BackingField")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .ValueGeneratedNever();
        builder.Property(team => team.Name)
            .HasField("<Name>k__BackingField")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .IsRequired();
    }
}
