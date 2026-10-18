using Microsoft.AspNetCore.Mvc;
using SquadSync.Application.Rosters;
using SquadSync.Domain;

namespace SquadSync.Api.Rosters;

[ApiController]
[Route("api/teams/{teamId}/roster")]
public sealed class RostersController(CreateRosterEntry createRosterEntry, GetRosterEntry getRosterEntry) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<RosterEntryResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RosterEntryResponse>> Create(
        Guid teamId, CreateRosterEntryRequest request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<RosterStatus>(request.RosterStatus, out var status) || status.ToString() != request.RosterStatus)
        {
            ModelState.AddModelError("rosterStatus", "A canonical roster status name is required.");
            return ValidationProblem(ModelState);
        }
        try
        {
            var entry = await createRosterEntry.ExecuteAsync(teamId, request.TeamMembershipId!.Value,
                request.JerseyNumber, status, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { teamId, rosterEntryId = entry.Id }, RosterEntryResponse.From(entry));
        }
        catch (RosterValidationException exception)
        {
            ModelState.AddModelError(exception.Field, "A non-empty identifier, valid jersey number or roster status is required.");
            return ValidationProblem(ModelState);
        }
        catch (RosterMembershipNotFoundException)
        {
            return NotFound();
        }
        catch (RosterMembershipNotPlayerException)
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "Only Player memberships may join the roster.");
        }
        catch (RosterEntryConflictException)
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "Roster entry already exists.");
        }
    }

    [HttpGet("{rosterEntryId}")]
    [ProducesResponseType<RosterEntryResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RosterEntryResponse>> GetById(
        Guid teamId, Guid rosterEntryId, CancellationToken cancellationToken)
    {
        try
        {
            var entry = await getRosterEntry.ExecuteAsync(teamId, rosterEntryId, cancellationToken);
            return entry is null ? NotFound() : Ok(RosterEntryResponse.From(entry));
        }
        catch (RosterValidationException exception)
        {
            ModelState.AddModelError(exception.Field, "A non-empty identifier is required.");
            return ValidationProblem(ModelState);
        }
    }
}
