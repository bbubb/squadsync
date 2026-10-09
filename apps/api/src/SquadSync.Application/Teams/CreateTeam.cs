using SquadSync.Domain;

namespace SquadSync.Application.Teams;

public sealed class CreateTeam(ITeamPersistence persistence)
{
    public async Task<Team> ExecuteAsync(string name, CancellationToken cancellationToken = default)
    {
        Team team;
        try
        {
            team = new Team(Guid.NewGuid(), name);
        }
        catch (ArgumentException exception) when (exception.ParamName is "name")
        {
            // Translate only Domain input validation; persistence errors must propagate unchanged.
            throw new TeamValidationException(exception.ParamName, exception);
        }

        await persistence.AddAsync(team, cancellationToken);
        return team;
    }
}
