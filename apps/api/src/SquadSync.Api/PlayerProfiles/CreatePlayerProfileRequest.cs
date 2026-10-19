using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SquadSync.Api.PlayerProfiles;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[JsonNumberHandling(JsonNumberHandling.Strict)]
public sealed class CreatePlayerProfileRequest
{
    [RegularExpression("^(Left|Right|Both)$", ErrorMessage = "Use Left, Right, Both or null.")]
    public string? DominantFoot { get; init; }
    public int? HeightInches { get; init; }
    public decimal? WeightPounds { get; init; }
}
