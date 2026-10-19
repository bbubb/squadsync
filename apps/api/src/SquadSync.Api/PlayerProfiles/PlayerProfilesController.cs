using Microsoft.AspNetCore.Mvc;
using SquadSync.Application.PlayerProfiles;
using SquadSync.Domain;

namespace SquadSync.Api.PlayerProfiles;

[ApiController]
[Route("api/people/{personId}/player-profile")]
public sealed class PlayerProfilesController(CreatePlayerProfile createProfile, GetPlayerProfile getProfile) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<PlayerProfileResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PlayerProfileResponse>> Create(Guid personId,
        CreatePlayerProfileRequest request, CancellationToken cancellationToken)
    {
        DominantFoot? foot = null;
        if (request.DominantFoot is not null)
        {
            if (!Enum.TryParse<DominantFoot>(request.DominantFoot, out var parsed)
                || !Enum.IsDefined(parsed) || parsed.ToString() != request.DominantFoot)
            {
                ModelState.AddModelError("dominantFoot", "Use Left, Right, Both or null.");
                return ValidationProblem(ModelState);
            }
            foot = parsed;
        }
        try
        {
            var profile = await createProfile.ExecuteAsync(personId, foot, request.HeightInches, request.WeightPounds, cancellationToken);
            return CreatedAtAction(nameof(Get), new { personId }, PlayerProfileResponse.From(profile));
        }
        catch (PlayerProfileValidationException exception)
        {
            ModelState.AddModelError(exception.Field, "A non-empty identifier or positive measurement is required.");
            return ValidationProblem(ModelState);
        }
        catch (PlayerProfilePersonNotFoundException)
        {
            return NotFound();
        }
        catch (PlayerProfileConflictException)
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "Player profile already exists.");
        }
    }

    [HttpGet]
    [ProducesResponseType<PlayerProfileResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PlayerProfileResponse>> Get(Guid personId, CancellationToken cancellationToken)
    {
        try
        {
            var profile = await getProfile.ExecuteAsync(personId, cancellationToken);
            return profile is null ? NotFound() : Ok(PlayerProfileResponse.From(profile));
        }
        catch (PlayerProfileValidationException exception)
        {
            ModelState.AddModelError(exception.Field, "A non-empty identifier is required.");
            return ValidationProblem(ModelState);
        }
    }
}
