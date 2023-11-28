using SquadSync.Data.Models;

namespace SquadSync.Data.Repositories.IRepositories
{
    public interface ILocationRepository
    {
        public Task ArchiveLocationAsync(Location location);
        public Task CreateLocationAsync(Location location);
        public Task DeleteLocationAsync(Location location);
        public Task<IEnumerable<Location>> GetAllLocationsAsync();
        public Task<Location> GetLocationByGuidAsync(Guid locationGuid);
        public Task<Location> GetLocationBySiteGuid(Guid siteGuid);
        public Task UpdateLocationAsync(Location location);
    }
}
