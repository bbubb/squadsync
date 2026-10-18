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
        return await ExecuteCoreAsync(rosterEntryId, teamMembershipId, jerseyNumber, rosterStatus, null, cancellationToken);
    }

    public Task<RosterEntry> ExecuteForTeamAsync(
        Guid teamId, Guid teamMembershipId, string? jerseyNumber, RosterStatus rosterStatus,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (teamId == Guid.Empty)
            throw new RosterValidationException("teamId", new ArgumentException("A non-empty ID is required.", "teamId"));
        return ExecuteCoreAsync(Guid.NewGuid(), teamMembershipId, jerseyNumber, rosterStatus, teamId, cancellationToken);
    }

    private async Task<RosterEntry> ExecuteCoreAsync(
        Guid rosterEntryId, Guid teamMembershipId, string? jerseyNumber, RosterStatus rosterStatus,
        Guid? teamId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var membership = await persistence.GetMembershipAsync(teamMembershipId, cancellationToken)
            ?? throw new RosterMembershipNotFoundException(teamMembershipId);

        if (teamId.HasValue && membership.TeamId != teamId.Value)
            throw new RosterMembershipNotFoundException(teamMembershipId);

        if (membership.TeamRole != TeamRole.Player)
        {
            throw new RosterMembershipNotPlayerException(teamMembershipId, membership.TeamRole);
        }

        var entry = new RosterEntry(rosterEntryId, membership.Id, jerseyNumber, rosterStatus);
        await persistence.AddEntryAsync(entry, cancellationToken);
        return entry;
    }
}
