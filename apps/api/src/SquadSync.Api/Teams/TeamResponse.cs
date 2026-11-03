using System.ComponentModel.DataAnnotations;

namespace SquadSync.Api.Teams;

public sealed record TeamResponse(
    [property: Required] Guid Id,
    [property: Required] string Name);
