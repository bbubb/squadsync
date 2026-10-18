using SquadSync.Domain;

namespace SquadSync.Application.Rosters;

public sealed record RosterPerson(Guid Id, string FirstName, string LastName);

public sealed record RosterDetails(Guid Id, Guid TeamMembershipId, RosterPerson Person,
    string? JerseyNumber, RosterStatus RosterStatus);
