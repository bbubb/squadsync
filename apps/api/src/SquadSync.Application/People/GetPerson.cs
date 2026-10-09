using SquadSync.Domain;

namespace SquadSync.Application.People;

public sealed class GetPerson(IPersonPersistence persistence)
{
    public Task<Person?> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
        => persistence.GetByIdAsync(id, cancellationToken);
}
