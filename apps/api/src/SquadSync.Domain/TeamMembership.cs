namespace SquadSync.Domain;

public sealed class TeamMembership
{
    public TeamMembership(Guid id, Guid userId, Guid teamId, TeamRole teamRole)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Membership ID cannot be empty.", nameof(id));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
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
        UserId = userId;
        TeamId = teamId;
        TeamRole = teamRole;
    }

    public Guid Id { get; }

    public Guid UserId { get; }

    public Guid TeamId { get; }

    public TeamRole TeamRole { get; }
}
