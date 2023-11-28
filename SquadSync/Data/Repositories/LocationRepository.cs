using Microsoft.EntityFrameworkCore;
using SquadSync.Data.Models;
using SquadSync.Data.Repositories.IRepositories;
using SquadSync.Exceptions;

namespace SquadSync.Data.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly SQLDbContext _dbContext;
        private readonly ILogger<LocationRepository> _logger;

        public LocationRepository(
                       SQLDbContext dbContext,
                       ILogger<LocationRepository> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task ArchiveLocationAsync(Location location)
        {
            _logger.LogDebug("LocationRepository: Start archiving location with GUID: {Guid}", location.LocationGuid);

            if (location == null)
            {
                _logger.LogWarning("LocationRepository: Location is null.");
                throw new CustomArgumentNullException("LocationRepository", nameof(location.LocationGuid));
            }

            _dbContext.Locations.Update(location);
            await _dbContext.SaveChangesAsync();

            _logger.LogDebug("LocationRepository: Finished archiving location with GUID: {Guid}", location.LocationGuid);
        }

        public async Task CreateLocationAsync(Location location)
        {
            _logger.LogDebug("LocationRepository: Start creating location with GUID: {Guid}", location.LocationGuid);

            if (location == null)
            {
                _logger.LogWarning("LocationRepository: Location is null.");
                throw new CustomArgumentNullException("LocationRepository", nameof(location.LocationGuid));
            }

            await _dbContext.Locations.AddAsync(location);
            await _dbContext.SaveChangesAsync();

            _logger.LogDebug("LocationRepository: Finished creating location with GUID: {Guid}", location.LocationGuid);
        }

        public async Task DeleteLocationAsync(Location location)
        {
            _logger.LogDebug("LocationRepository: Start deleting location with GUID: {Guid}", location.LocationGuid);

            if (location == null)
            {
                _logger.LogWarning("LocationRepository: Location is null.");
                throw new CustomArgumentNullException("LocationRepository", nameof(location.LocationGuid));
            }

            _dbContext.Locations.Remove(location);
            await _dbContext.SaveChangesAsync();

            _logger.LogDebug("LocationRepository: Finished deleting location with GUID: {Guid}", location.LocationGuid);
        }

        public async Task<IEnumerable<Location>> GetAllLocationsAsync()
        {
            _logger.LogDebug("LocationRepository: Start getting all locations.");

            var locations = await _dbContext.Locations.ToListAsync();
            if(locations == null)
            {
                _logger.LogWarning("LocationRepository: Locations is null.");
                throw new EntityNotFoundException("LocationRepository", nameof(locations));
            }
            _logger.LogDebug("LocationRepository: Finished getting all locations.");
            return locations;
        }

        public async Task<Location> GetLocationByGuidAsync(Guid locationGuid)
        {
            _logger.LogDebug("LocationRepository: Start getting location with GUID: {Guid}", locationGuid);

            var location = await _dbContext.Locations.FirstOrDefaultAsync(l => l.LocationGuid == locationGuid);
            if (location == null)
            {
                _logger.LogWarning("LocationRepository: Location is null.");
                throw new EntityNotFoundException("LocationRepository", nameof(location.LocationGuid));
            }
            _logger.LogDebug("LocationRepository: Finished getting location with GUID: {Guid}", locationGuid);

            return location;
        }

        public async Task<Location> GetLocationBySiteGuid(Guid siteGuid)
        {
            _logger.LogDebug("LocationRepository: Start getting location with Site GUID: {Guid}", siteGuid);

            var location = await _dbContext.Locations.FirstOrDefaultAsync(l => l.Sites.Any(s => s.SiteGuid == siteGuid));
            if (location == null)
            {
                _logger.LogWarning("LocationRepository: Location is null.");
                throw new EntityNotFoundException("LocationRepository", nameof(location.Sites));
            }

            _logger.LogDebug("LocationRepository: Finished getting location with Site GUID: {Guid}", siteGuid);
            return location;
        }

        public async Task UpdateLocationAsync(Location location)
        {
            _logger.LogDebug("LocationRepository: Start updating location with GUID: {Guid}", location.LocationGuid);

            if (location == null)
            {
                _logger.LogWarning("LocationRepository: Location is null.");
                throw new CustomArgumentNullException("LocationRepository", nameof(location.LocationGuid));
            }

            _dbContext.Locations.Update(location);
            await _dbContext.SaveChangesAsync();

            _logger.LogDebug("LocationRepository: Finished updating location with GUID: {Guid}", location.LocationGuid);
        }
    }
}
