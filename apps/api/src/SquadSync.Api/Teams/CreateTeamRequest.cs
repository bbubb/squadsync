using System.ComponentModel.DataAnnotations;

namespace SquadSync.Api.Teams;

public sealed class CreateTeamRequest
{
    [Required]
    public string? Name { get; init; }
}
