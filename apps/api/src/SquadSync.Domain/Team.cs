namespace SquadSync.Domain;

public sealed class Team
{
    public Team(Guid id, string name)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Team ID cannot be empty.", nameof(id));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Id = id;
        Name = name.Trim();
    }

    public Guid Id { get; }

    public string Name { get; }
}
