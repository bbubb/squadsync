using SquadSync.Application.People;
using SquadSync.Application.Teams;
using SquadSync.Domain;

namespace SquadSync.Application.Memberships;

public sealed class CreateMembership(
    IMembershipPersistence memberships, ITeamPersistence teams, IPersonPersistence people)
{
    public async Task<MembershipDetails> ExecuteAsync(
        Guid teamId, Guid personId, TeamRole teamRole, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        TeamMembership membership;
        try
        {
            membership = new TeamMembership(Guid.NewGuid(), personId, teamId, teamRole);
        }
        catch (ArgumentException exception) when (exception.ParamName is "personId" or "teamId" or "teamRole")
        {
            throw new MembershipValidationException(exception.ParamName, exception);
        }

        if (await teams.GetByIdAsync(teamId, cancellationToken) is null)
        {
            throw new MembershipReferenceNotFoundException();
        }
        var person = await people.GetByIdAsync(personId, cancellationToken)
            ?? throw new MembershipReferenceNotFoundException();

        await memberships.AddAsync(membership, cancellationToken);
        return new MembershipDetails(membership.Id, membership.TeamId,
            new MembershipPerson(person.Id, person.FirstName, person.LastName), membership.TeamRole);
    }
}
