using SquadSync.Application.Memberships;
using SquadSync.Domain;

namespace SquadSync.Application.Rosters;

public sealed class CreateRosterEntry(AddPlayerToRoster addPlayer, IMembershipPersistence memberships)
{
    public async Task<RosterDetails> ExecuteAsync(Guid teamId, Guid teamMembershipId, string? jerseyNumber,
        RosterStatus rosterStatus, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (teamId == Guid.Empty)
            throw new RosterValidationException("teamId", new ArgumentException("A non-empty ID is required.", "teamId"));
        // Validate intrinsic values before any persistence access; Domain owns their rules.
        try
        {
            _ = new RosterEntry(Guid.NewGuid(), teamMembershipId, jerseyNumber, rosterStatus);
        }
        catch (ArgumentException exception) when (exception.ParamName is "teamMembershipId" or "jerseyNumber" or "rosterStatus")
        {
            throw new RosterValidationException(exception.ParamName, exception);
        }

        var membership = await memberships.GetByIdAsync(teamId, teamMembershipId, cancellationToken)
            ?? throw new RosterMembershipNotFoundException(teamMembershipId);
        // The existing membership projection supplies Person data; AddPlayerToRoster
        // independently enforces team ownership and Player eligibility before writing.
        var entry = await addPlayer.ExecuteForTeamAsync(teamId, teamMembershipId, jerseyNumber, rosterStatus, cancellationToken);
        return new RosterDetails(entry.Id, entry.TeamMembershipId,
            new RosterPerson(membership.Person.Id, membership.Person.FirstName, membership.Person.LastName),
            entry.JerseyNumber, entry.RosterStatus);
    }
}
