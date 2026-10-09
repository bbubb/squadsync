using SquadSync.Domain;

namespace SquadSync.Application.People;

public sealed class CreatePerson(IPersonPersistence persistence)
{
    public async Task<Person> ExecuteAsync(
        string firstName, string lastName, CancellationToken cancellationToken = default)
    {
        Person person;
        try
        {
            person = new Person(Guid.NewGuid(), firstName, lastName);
        }
        catch (ArgumentException exception) when (exception.ParamName is "firstName" or "lastName")
        {
            // Translate only Domain input validation; persistence errors must propagate unchanged.
            throw new PersonValidationException(exception.ParamName, exception);
        }

        await persistence.AddAsync(person, cancellationToken);
        return person;
    }
}
