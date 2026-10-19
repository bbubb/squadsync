using SquadSync.Application.PlayerProfiles;

namespace SquadSync.Api.PlayerProfiles;

public sealed record PlayerProfileResponse(Guid Id, Guid PersonId, string? DominantFoot, int? HeightInches, decimal? WeightPounds)
{
    public static PlayerProfileResponse From(PlayerProfileDetails profile)
        => new(profile.Id, profile.PersonId, profile.DominantFoot?.ToString(), profile.HeightInches, profile.WeightPounds);
}
