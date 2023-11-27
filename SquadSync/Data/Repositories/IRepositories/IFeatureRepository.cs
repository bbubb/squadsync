using SquadSync.Data.Models;

namespace SquadSync.Data.Repositories.IRepositories;

public interface IFeatureRepository
{
    Task CreateFeatureAsync(Feature feature);
    Task ArchiveFeatureAsync(Feature feature);
    Task DeleteFeatureAsync(Feature feature);
    Task<Feature> GetFeatureByGuidAsync(Guid guid);
    Task<ICollection<Feature>> GetAllFeaturesAsync();
    Task ToggleFeatureStatusAsync(Guid featureGuid, bool isActiveFeature);
    Task UpdateFeatureAsync(Feature feature);
}