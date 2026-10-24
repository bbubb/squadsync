using SquadSync.Domain;

namespace SquadSync.Application.PlayerProfiles;

public interface IPlayerProfilePersistence
{
    Task<PlayerProfileDetails?> GetByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default);
    
    Task AddAsync(PlayerProfile profile, CancellationToken cancellationToken = default);
    
    Task<PlayerProfile?> GetForUpdateAsync(Guid personId, CancellationToken cancellationToken = default);

    Task UpdateAsync(PlayerProfile profile, CancellationToken cancellationToken = default);
}
