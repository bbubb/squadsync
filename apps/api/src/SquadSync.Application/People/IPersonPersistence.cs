using SquadSync.Domain;

namespace SquadSync.Application.People;

public interface IPersonPersistence
{
    Task AddAsync(Person person, CancellationToken cancellationToken = default);

    Task<Person?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
