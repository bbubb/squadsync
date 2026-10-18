using Microsoft.AspNetCore.Mvc;
using SquadSync.Application.Memberships;
using SquadSync.Domain;

namespace SquadSync.Api.Memberships;

[ApiController]
[Route("api/teams/{teamId}/memberships")]
public sealed class MembershipsController(CreateMembership createMembership, GetMembership getMembership, GetTeamMemberships getTeamMemberships) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<MembershipResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<MembershipResponse>> Create(
        Guid teamId, CreateMembershipRequest request, CancellationToken cancellationToken)
    {
        // Enum.TryParse alone also accepts numeric and whitespace values.
        if (!Enum.TryParse<TeamRole>(request.TeamRole, out var role) || role.ToString() != request.TeamRole)
        {
            ModelState.AddModelError("teamRole", "A canonical team role name is required.");
            return ValidationProblem(ModelState);
        }
        try
        {
            var membership = await createMembership.ExecuteAsync(teamId, request.PersonId!.Value, role, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { teamId, membershipId = membership.Id }, MembershipResponse.From(membership));
        }
        catch (MembershipValidationException exception)
        {
            ModelState.AddModelError(exception.Field, "A non-empty identifier or valid team role is required.");
            return ValidationProblem(ModelState);
        }
        catch (MembershipReferenceNotFoundException)
        {
            return NotFound();
        }
        catch (MembershipConflictException)
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "Membership already exists.");
        }
    }

    [HttpGet("{membershipId}")]
    [ProducesResponseType<MembershipResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MembershipResponse>> GetById(
        Guid teamId, Guid membershipId, CancellationToken cancellationToken)
    {
        try
        {
            var membership = await getMembership.ExecuteAsync(teamId, membershipId, cancellationToken);
            return membership is null ? NotFound() : Ok(MembershipResponse.From(membership));
        }
        catch (MembershipValidationException exception)
        {
            ModelState.AddModelError(exception.Field, "A non-empty identifier is required.");
            return ValidationProblem(ModelState);
        }
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<MembershipResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<MembershipResponse>>> GetByTeamId(
        Guid teamId, CancellationToken cancellationToken)
    {
        try
        {
            var memberships = await getTeamMemberships.ExecuteAsync(teamId, cancellationToken);
            return Ok(memberships.Select(MembershipResponse.From).ToList());
        }
        catch (MembershipValidationException exception)
        {
            ModelState.AddModelError(exception.Field, "A non-empty identifier is required.");
            return ValidationProblem(ModelState);
        }
        catch (MembershipReferenceNotFoundException)
        {
            return NotFound();
        }
    }
}
