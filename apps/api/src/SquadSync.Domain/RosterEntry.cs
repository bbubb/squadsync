namespace SquadSync.Domain;

public sealed class RosterEntry
{
    public RosterEntry(Guid id, Guid teamMembershipId, string? jerseyNumber, RosterStatus rosterStatus)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Roster entry ID cannot be empty.", nameof(id));
        }

        if (teamMembershipId == Guid.Empty)
        {
            throw new ArgumentException("Team membership ID cannot be empty.", nameof(teamMembershipId));
        }

        if (!Enum.IsDefined(rosterStatus))
        {
            throw new ArgumentOutOfRangeException(nameof(rosterStatus), rosterStatus, "Roster status must be a defined value.");
        }

        if (jerseyNumber is not null)
        {
            if (jerseyNumber.Length is < 1 or > 3 || jerseyNumber.Any(character => character is < '0' or > '9'))
            {
                throw new ArgumentException("Jersey number must contain exactly 1–3 ASCII digits.", nameof(jerseyNumber));
            }
        }

        Id = id;
        TeamMembershipId = teamMembershipId;
        JerseyNumber = jerseyNumber;
        RosterStatus = rosterStatus;
    }

    public Guid Id { get; }

    public Guid TeamMembershipId { get; }

    public string? JerseyNumber { get; }

    public RosterStatus RosterStatus { get; }
}
