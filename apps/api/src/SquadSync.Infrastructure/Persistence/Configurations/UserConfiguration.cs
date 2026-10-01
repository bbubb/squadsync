using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SquadSync.Domain;

namespace SquadSync.Infrastructure.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(user => user.Id);
        builder.Property(user => user.Id)
            .HasField("<Id>k__BackingField")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .ValueGeneratedNever();
        builder.Property(user => user.FirstName)
            .HasField("<FirstName>k__BackingField")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .IsRequired();
        builder.Property(user => user.LastName)
            .HasField("<LastName>k__BackingField")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .IsRequired();
    }
}
