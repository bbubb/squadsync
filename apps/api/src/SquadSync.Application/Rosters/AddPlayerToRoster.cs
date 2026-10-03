using SquadSync.Domain;

namespace SquadSync.Application.Rosters;

public sealed class AddPlayerToRoster(IRosterPersistence persistence)
{
    public async Task<RosterEntry> ExecuteAsync(
        Guid rosterEntryId,
        Guid teamMembershipId,
        string? jerseyNumber,
        RosterStatus rosterStatus,
        CancellationToken cancellationToken = default)
    {
        var membership = await persistence.GetMembershipAsync(teamMembershipId, cancellationToken)
            ?? throw new RosterMembershipNotFoundException(teamMembershipId);

        if (membership.TeamRole != TeamRole.Player)
        {
            throw new RosterMembershipNotPlayerException(teamMembershipId, membership.TeamRole);
        }

        var entry = new RosterEntry(rosterEntryId, membership.Id, jerseyNumber, rosterStatus);
        await persistence.AddEntryAsync(entry, cancellationToken);
        return entry;
    }
}
