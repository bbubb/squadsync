using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SquadSync.Application.Rosters;

namespace SquadSync.Api.Rosters;

public sealed record RosterPersonResponse(
    [property: Required] Guid Id,
    [property: Required] string FirstName,
    [property: Required] string LastName);

public sealed record RosterEntryResponse(
    [property: Required] Guid Id,
    [property: Required] Guid TeamMembershipId,
    [property: Required] RosterPersonResponse Person,
    [property: JsonRequired] string? JerseyNumber,
    [property: Required] string RosterStatus)
{
    public static RosterEntryResponse From(RosterDetails entry)
        => new(entry.Id, entry.TeamMembershipId,
            new RosterPersonResponse(entry.Person.Id, entry.Person.FirstName, entry.Person.LastName),
            entry.JerseyNumber, entry.RosterStatus.ToString());
}
