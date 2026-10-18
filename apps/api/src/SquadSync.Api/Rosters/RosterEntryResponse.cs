using SquadSync.Application.Rosters;

namespace SquadSync.Api.Rosters;

public sealed record RosterPersonResponse(Guid Id, string FirstName, string LastName);

public sealed record RosterEntryResponse(Guid Id, Guid TeamMembershipId, RosterPersonResponse Person,
    string? JerseyNumber, string RosterStatus)
{
    public static RosterEntryResponse From(RosterDetails entry)
        => new(entry.Id, entry.TeamMembershipId,
            new RosterPersonResponse(entry.Person.Id, entry.Person.FirstName, entry.Person.LastName),
            entry.JerseyNumber, entry.RosterStatus.ToString());
}
