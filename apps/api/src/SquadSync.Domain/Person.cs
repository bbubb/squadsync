namespace SquadSync.Domain;

public sealed class Person
{
    public Person(Guid id, string firstName, string lastName)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Person ID cannot be empty.", nameof(id));
        }

        Id = id;
        FirstName = NormalizeRequiredName(firstName, nameof(firstName));
        LastName = NormalizeRequiredName(lastName, nameof(lastName));
    }

    public Guid Id { get; }

    public string FirstName { get; }

    public string LastName { get; }

    private static string NormalizeRequiredName(string name, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, parameterName);
        return name.Trim();
    }
}
