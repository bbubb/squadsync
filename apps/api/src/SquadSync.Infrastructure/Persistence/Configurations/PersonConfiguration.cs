using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SquadSync.Domain;

namespace SquadSync.Infrastructure.Persistence.Configurations;

internal sealed class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.HasKey(person => person.Id);
        builder.Property(person => person.Id)
            .ValueGeneratedNever();
        builder.Property(person => person.FirstName)
            .IsRequired();
        builder.Property(person => person.LastName)
            .IsRequired();
    }
}
