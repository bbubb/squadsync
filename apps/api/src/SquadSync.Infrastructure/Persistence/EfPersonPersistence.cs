using Microsoft.EntityFrameworkCore;
using SquadSync.Application.People;
using SquadSync.Domain;

namespace SquadSync.Infrastructure.Persistence;

public sealed class EfPersonPersistence(SquadSyncDbContext context) : IPersonPersistence
{
    public async Task AddAsync(Person person, CancellationToken cancellationToken = default)
    {
        context.People.Add(person);
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task<Person?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => context.People.AsNoTracking().SingleOrDefaultAsync(person => person.Id == id, cancellationToken);
}
