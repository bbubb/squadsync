using System.ComponentModel.DataAnnotations;
using SquadSync.Application.Memberships;

namespace SquadSync.Api.Memberships;

public sealed record MembershipPersonResponse(
    [property: Required] Guid Id,
    [property: Required] string FirstName,
    [property: Required] string LastName);

public sealed record MembershipResponse(
    [property: Required] Guid Id,
    [property: Required] Guid TeamId,
    [property: Required] MembershipPersonResponse Person,
    [property: Required] string TeamRole)
{
    public static MembershipResponse From(MembershipDetails membership)
        => new(membership.Id, membership.TeamId,
            new MembershipPersonResponse(membership.Person.Id, membership.Person.FirstName, membership.Person.LastName),
            membership.TeamRole.ToString());
}
