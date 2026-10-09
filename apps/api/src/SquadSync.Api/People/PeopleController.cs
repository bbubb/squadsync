using Microsoft.AspNetCore.Mvc;
using SquadSync.Application.People;

namespace SquadSync.Api.People;

[ApiController]
[Route("api/people")]
public sealed class PeopleController(CreatePerson createPerson, GetPerson getPerson) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<PersonResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PersonResponse>> Create(
        CreatePersonRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var person = await createPerson.ExecuteAsync(request.FirstName!, request.LastName!, cancellationToken);
            var response = new PersonResponse(person.Id, person.FirstName, person.LastName);
            return CreatedAtAction(nameof(GetById), new { id = person.Id }, response);
        }
        catch (PersonValidationException exception)
        {
            ModelState.AddModelError(exception.Field, "A non-blank name is required.");
            return ValidationProblem(ModelState);
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType<PersonResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PersonResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var person = await getPerson.ExecuteAsync(id, cancellationToken);
        return person is null ? NotFound() : Ok(new PersonResponse(person.Id, person.FirstName, person.LastName));
    }
}
