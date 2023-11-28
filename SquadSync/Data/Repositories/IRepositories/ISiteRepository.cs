using SquadSync.Data.Models;

namespace SquadSync.Data.Repositories.IRepositories
{
    public interface ISiteRepository
    {
        Task ArchiveSiteAsync(Site site);
        Task CreateSiteAsync(Site site);
        Task DeleteSiteAsync(Site site);
        Task<IEnumerable<Site>> GetAllSitesAsync();
        Task<Site> GetSiteByGuidAsync(Guid siteGuid);
        Task<IEnumerable<Site>> GetSitesByLocationGuidAsync(Guid locationGuid);
        Task UpdateSiteAsync(Site site);
    }
}
