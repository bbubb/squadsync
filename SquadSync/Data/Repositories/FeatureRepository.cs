using Microsoft.EntityFrameworkCore;
using SquadSync.Data.Models;
using SquadSync.Data.Repositories.IRepositories;
using SquadSync.Exceptions;

namespace SquadSync.Data.Repositories;

public class FeatureRepository : IFeatureRepository
{
    private readonly SQLDbContext _dbContext;
    private readonly ILogger<FeatureRepository> _logger;

    public FeatureRepository(
                   SQLDbContext dbContext,
                              ILogger<FeatureRepository> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task ArchiveFeatureAsync(Feature feature)
    {
        _logger.LogDebug("FeatureRepository: Start archiving feature with GUID: {Guid}", feature.FeatureGuid);

        if (feature == null)
        {
            _logger.LogWarning("FeatureRepository: Feature is null.");
            throw new CustomArgumentNullException("FeatureRepository", nameof(feature.FeatureGuid));
        }

        _dbContext.Features.Update(feature);
        await _dbContext.SaveChangesAsync();

        _logger.LogDebug("FeatureRepository: Finished archiving feature with GUID: {Guid}", feature.FeatureGuid);
    }

    public async Task CreateFeatureAsync(Feature feature)
    {
        _logger.LogDebug("FeatureRepository: Start creating feature with GUID: {Guid}", feature.FeatureGuid);

        if (feature == null)
        {
            _logger.LogWarning("FeatureRepository: Feature is null.");
            throw new CustomArgumentNullException("FeatureRepository", nameof(feature.FeatureGuid));
        }

        _dbContext.Features.Add(feature);
        await _dbContext.SaveChangesAsync();

        _logger.LogDebug("FeatureRepository: Finished creating feature with GUID: {Guid}", feature.FeatureGuid);
    }

    public async Task DeleteFeatureAsync(Feature feature)
    {
        _logger.LogDebug("FeatureRepository: Start deleting feature with GUID: {Guid}", feature.FeatureGuid);

        if (feature == null)
        {
            _logger.LogWarning("FeatureRepository: Feature is null.");
            throw new CustomArgumentNullException("FeatureRepository", nameof(feature.FeatureGuid));
        }

        _dbContext.Features.Remove(feature);
        await _dbContext.SaveChangesAsync();

        _logger.LogDebug("FeatureRepository: Finished deleting feature with GUID: {Guid}", feature.FeatureGuid);
    }

    public async Task<IEnumerable<Feature>> GetAllFeaturesAsync()
    {
        _logger.LogDebug("FeatureRepository: Start getting all features.");

        var features = await _dbContext.Features.ToListAsync();

        _logger.LogDebug("FeatureRepository: Finished getting all features.");

        return features;
    }

    public async Task<Feature> GetFeatureByGuidAsync(Guid featureGuid)
    {
        _logger.LogDebug("FeatureRepository: Start getting feature with GUID: {Guid}", featureGuid);

        var feature = await _dbContext.Features.FirstOrDefaultAsync(f => f.FeatureGuid == featureGuid);

        _logger.LogDebug("FeatureRepository: Finished getting feature with GUID: {Guid}", featureGuid);

        return feature;
    }

    public async Task ToggleFeatureStatusAsync(Guid featureGuid, bool isActiveFeature)
    {
        _logger.LogDebug("FeatureRepository: Start toggling feature status with GUID: {Guid}", featureGuid);

        var feature = await _dbContext.Features.FirstOrDefaultAsync(f => f.FeatureGuid == featureGuid);

        if (feature == null)
        {
            _logger.LogWarning("FeatureRepository: Feature is null.");
            throw new CustomArgumentNullException("FeatureRepository", nameof(feature.FeatureGuid));
        }

        feature.IsActiveFeature = isActiveFeature;
        _dbContext.Features.Update(feature);
        await _dbContext.SaveChangesAsync();

        _logger.LogDebug("FeatureRepository: Finished toggling feature status with GUID: {Guid}", featureGuid);
    }

    public async Task UpdateFeatureAsync(Feature feature)
    {
        _logger.LogDebug("FeatureRepository: Start updating feature with GUID: {Guid}", feature.FeatureGuid);

        if (feature == null)
        {
            _logger.LogWarning("FeatureRepository: Feature is null.");
            throw new CustomArgumentNullException("FeatureRepository", nameof(feature.FeatureGuid));
        }

        _dbContext.Features.Update(feature);
        await _dbContext.SaveChangesAsync();

        _logger.LogDebug("FeatureRepository: Finished updating feature with GUID: {Guid}", feature.FeatureGuid);
    }
}