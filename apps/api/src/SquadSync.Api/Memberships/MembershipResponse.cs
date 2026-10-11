using SquadSync.Application.Memberships;

namespace SquadSync.Api.Memberships;

public sealed record MembershipPersonResponse(Guid Id, string FirstName, string LastName);

public sealed record MembershipResponse(Guid Id, Guid TeamId, MembershipPersonResponse Person, string TeamRole)
{
    public static MembershipResponse From(MembershipDetails membership)
        => new(membership.Id, membership.TeamId,
            new MembershipPersonResponse(membership.Person.Id, membership.Person.FirstName, membership.Person.LastName),
            membership.TeamRole.ToString());
}
