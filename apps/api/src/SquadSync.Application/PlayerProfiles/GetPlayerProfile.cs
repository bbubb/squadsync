namespace SquadSync.Application.PlayerProfiles;

public sealed class GetPlayerProfile(IPlayerProfilePersistence profiles)
{
    public Task<PlayerProfileDetails?> ExecuteAsync(Guid personId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (personId == Guid.Empty)
            throw new PlayerProfileValidationException("personId", new ArgumentException("A non-empty ID is required.", "personId"));
        return profiles.GetByPersonIdAsync(personId, cancellationToken);
    }
}
