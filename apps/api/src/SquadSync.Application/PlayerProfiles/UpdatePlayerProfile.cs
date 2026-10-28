using SquadSync.Domain;

namespace SquadSync.Application.PlayerProfiles;

public sealed class UpdatePlayerProfile(IPlayerProfilePersistence profiles)
{
    public async Task<PlayerProfileDetails?> ExecuteAsync(
        Guid personId,
        DominantFoot? dominantFoot,
        int? heightInches,
        decimal? weightPounds,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (personId == Guid.Empty)
            throw new PlayerProfileValidationException("personId", new ArgumentException("A non-empty ID is required.", "personId"));

        var profile = await profiles.GetForUpdateAsync(personId, cancellationToken);

        if (profile is null)
            return null;

        try
        {
            profile.Update(dominantFoot, heightInches, weightPounds);
        }
        catch (ArgumentException exception) when (
            exception.ParamName is "dominantFoot" or "heightInches" or "weightPounds")
        {
            throw new PlayerProfileValidationException(
                exception.ParamName, exception);
        }

        await profiles.UpdateAsync(profile, cancellationToken);
        return PlayerProfileDetails.From(profile);
    }
}
