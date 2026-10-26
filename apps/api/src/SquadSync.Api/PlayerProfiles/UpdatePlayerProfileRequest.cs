using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SquadSync.Api.PlayerProfiles;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[JsonNumberHandling(JsonNumberHandling.Strict)]
public sealed class UpdatePlayerProfileRequest
{
    [JsonRequired]
    [RegularExpression("^(Left|Right|Both)$")]
    public string? DominantFoot { get; init; }

    [JsonRequired]
    public int? HeightInches { get; init; }

    [JsonRequired]
    public decimal? WeightPounds { get; init; }
}
