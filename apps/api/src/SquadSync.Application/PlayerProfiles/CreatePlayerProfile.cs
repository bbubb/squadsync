using SquadSync.Application.People;
using SquadSync.Domain;

namespace SquadSync.Application.PlayerProfiles;

public sealed class CreatePlayerProfile(IPlayerProfilePersistence profiles, IPersonPersistence people)
{
    public async Task<PlayerProfileDetails> ExecuteAsync(Guid personId, DominantFoot? dominantFoot,
        int? heightInches, decimal? weightPounds, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        PlayerProfile profile;
        try
        {
            profile = new PlayerProfile(Guid.NewGuid(), personId, dominantFoot, heightInches, weightPounds);
        }
        catch (ArgumentException exception) when (exception.ParamName is "personId" or "dominantFoot" or "heightInches" or "weightPounds")
        {
            throw new PlayerProfileValidationException(exception.ParamName, exception);
        }

        if (await people.GetByIdAsync(personId, cancellationToken) is null)
            throw new PlayerProfilePersonNotFoundException();
        if (await profiles.GetByPersonIdAsync(personId, cancellationToken) is not null)
            throw new PlayerProfileConflictException();

        await profiles.AddAsync(profile, cancellationToken);
        return PlayerProfileDetails.From(profile);
    }
}
