using Microsoft.AspNetCore.Mvc;
using SquadSync.Application.Teams;

namespace SquadSync.Api.Teams;

[ApiController]
[Route("api/teams")]
public sealed class TeamsController(CreateTeam createTeam, GetTeam getTeam) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<TeamResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TeamResponse>> Create(
        CreateTeamRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var team = await createTeam.ExecuteAsync(request.Name!, cancellationToken);
            var response = new TeamResponse(team.Id, team.Name);
            return CreatedAtAction(nameof(GetById), new { id = team.Id }, response);
        }
        catch (TeamValidationException exception)
        {
            ModelState.AddModelError(exception.Field, "A non-blank name is required.");
            return ValidationProblem(ModelState);
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType<TeamResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TeamResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var team = await getTeam.ExecuteAsync(id, cancellationToken);
        return team is null ? NotFound() : Ok(new TeamResponse(team.Id, team.Name));
    }
}
