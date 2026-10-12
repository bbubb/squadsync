using SquadSync.Domain;

namespace SquadSync.Application.Memberships;

public sealed record MembershipPerson(Guid Id, string FirstName, string LastName);

public sealed record MembershipDetails(Guid Id, Guid TeamId, MembershipPerson Person, TeamRole TeamRole);
