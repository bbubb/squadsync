using Microsoft.EntityFrameworkCore;
using SquadSync.Data.Models;
using SquadSync.Data.Repositories.IRepositories;
using SquadSync.Exceptions;

namespace SquadSync.Data.Repositories
{
    public class SiteRepository : ISiteRepository
    {
        readonly SQLDbContext _dbContext;
        readonly ILogger<SiteRepository> _logger;

        public SiteRepository(
            SQLDbContext dbContext,
            ILogger<SiteRepository> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task ArchiveSiteAsync(Site site)
        {
            _logger.LogDebug("SiteRepository: Start archiving site with GUID: {Guid}", site.SiteGuid);

            if (site == null)
            {
                _logger.LogWarning("SiteRepository: Site is null.");
                throw new CustomArgumentNullException("SiteRepository", nameof(site.SiteGuid));
            }

            _dbContext.Sites.Update(site);
            await _dbContext.SaveChangesAsync();

            _logger.LogDebug("SiteRepository: Finished archiving site with GUID: {Guid}", site.SiteGuid);
        }

        public async Task CreateSiteAsync(Site site)
        {
            _logger.LogDebug("SiteRepository: Start creating site with GUID: {Guid}", site.SiteGuid);

            if (site == null)
            {
                _logger.LogWarning("SiteRepository: Site is null.");
                throw new CustomArgumentNullException("SiteRepository", nameof(site.SiteGuid));
            }

            await _dbContext.Sites.AddAsync(site);
            await _dbContext.SaveChangesAsync();

            _logger.LogDebug("SiteRepository: Finished creating site with GUID: {Guid}", site.SiteGuid);
        }

        public async Task DeleteSiteAsync(Site site)
        {
            _logger.LogDebug("SiteRepository: Start deleting site with GUID: {Guid}", site.SiteGuid);

            if (site == null)
            {
                _logger.LogWarning("SiteRepository: Site is null.");
                throw new CustomArgumentNullException("SiteRepository", nameof(site.SiteGuid));
            }

            _dbContext.Sites.Remove(site);
            await _dbContext.SaveChangesAsync();

            _logger.LogDebug("SiteRepository: Finished deleting site with GUID: {Guid}", site.SiteGuid);
        }

        public async Task<IEnumerable<Site>> GetAllSitesAsync()
        {
            _logger.LogDebug("SiteRepository: Start getting all sites.");

            var sites = await _dbContext.Sites.ToListAsync();

            _logger.LogDebug("SiteRepository: Finished getting all sites.");

            return sites;
        }

        public async Task<Site> GetSiteByGuidAsync(Guid siteGuid)
        {
            _logger.LogDebug("SiteRepository: Start getting site with GUID: {Guid}", siteGuid);

            var site = await _dbContext.Sites.FirstOrDefaultAsync(s => s.SiteGuid == siteGuid);
            if (site == null)
            {
                _logger.LogWarning("SiteRepository: Site is null.");
                throw new EntityNotFoundException("SiteRepository", nameof(site.SiteGuid));
            }

            _logger.LogDebug("SiteRepository: Finished getting site with GUID: {Guid}", siteGuid);
            return site;
        }

        public async Task<IEnumerable<Site>> GetSitesByLocationGuidAsync(Guid locationGuid)
        {
            _logger.LogDebug("SiteRepository: Start getting sites with Location GUID: {Guid}", locationGuid);

            var sites = await _dbContext.Sites.Where(s => s.Location.LocationGuid == locationGuid).ToListAsync();

            if (sites == null)
            {
                _logger.LogWarning("SiteRepository: Sites is null.");
                throw new EntityNotFoundException("SiteRepository", nameof(sites));
            }

            _logger.LogDebug("SiteRepository: Finished getting sites with Location GUID: {Guid}", locationGuid);
            return sites;
        }

        public async Task UpdateSiteAsync(Site site)
        {
            _logger.LogDebug("SiteRepository: Start updating site with GUID: {Guid}", site.SiteGuid);

            if (site == null)
            {
                _logger.LogWarning("SiteRepository: Site is null.");
                throw new CustomArgumentNullException("SiteRepository", nameof(site.SiteGuid));
            }

            _dbContext.Sites.Update(site);
            await _dbContext.SaveChangesAsync();

            _logger.LogDebug("SiteRepository: Finished updating site with GUID: {Guid}", site.SiteGuid);
        }
    }
}
