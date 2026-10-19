using SquadSync.Domain;

namespace SquadSync.Application.PlayerProfiles;

public sealed record PlayerProfileDetails(Guid Id, Guid PersonId, DominantFoot? DominantFoot, int? HeightInches, decimal? WeightPounds)
{
    public static PlayerProfileDetails From(PlayerProfile profile)
        => new(profile.Id, profile.PersonId, profile.DominantFoot, profile.HeightInches, profile.WeightPounds);
}
