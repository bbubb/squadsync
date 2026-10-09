namespace SquadSync.Domain;

public sealed class TeamMembership
{
    public TeamMembership(Guid id, Guid personId, Guid teamId, TeamRole teamRole)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Membership ID cannot be empty.", nameof(id));
        }

        if (personId == Guid.Empty)
        {
            throw new ArgumentException("Person ID cannot be empty.", nameof(personId));
        }

        if (teamId == Guid.Empty)
        {
            throw new ArgumentException("Team ID cannot be empty.", nameof(teamId));
        }

        if (!Enum.IsDefined(teamRole))
        {
            throw new ArgumentOutOfRangeException(nameof(teamRole), teamRole, "Team role must be a defined value.");
        }

        Id = id;
        PersonId = personId;
        TeamId = teamId;
        TeamRole = teamRole;
    }

    public Guid Id { get; }

    public Guid PersonId { get; }

    public Guid TeamId { get; }

    public TeamRole TeamRole { get; }
}
