using System.ComponentModel.DataAnnotations;

namespace SquadSync.Api.People;

public sealed class CreatePersonRequest
{
    [Required]
    public string? FirstName { get; init; }

    [Required]
    public string? LastName { get; init; }
}
