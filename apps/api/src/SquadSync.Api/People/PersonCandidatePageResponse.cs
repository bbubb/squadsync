using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SquadSync.Application.People;

namespace SquadSync.Api.People;

public sealed record PersonCandidateResponse(
    [property: Required] Guid PersonId,
    [property: Required] string FirstName,
    [property: Required] string LastName);

public sealed record PersonCandidatePageResponse(
    [property: Required] IReadOnlyList<PersonCandidateResponse> Items,
    [property: JsonRequired] string? NextCursor)
{
    public static PersonCandidatePageResponse From(PersonCandidatePage page)
        => new(page.Items.Select(item => new PersonCandidateResponse(item.PersonId, item.FirstName, item.LastName)).ToArray(), page.NextCursor);
}
