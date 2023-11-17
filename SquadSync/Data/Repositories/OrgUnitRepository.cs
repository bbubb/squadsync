using Microsoft.EntityFrameworkCore;
using SquadSync.Data.Models;
using SquadSync.Data.Repositories.IRepositories;
using SquadSync.Exceptions;

namespace SquadSync.Data.Repositories
{
    public class OrgUnitRepository : IOrgUnitRepository
    {
        private readonly SQLDbContext _dbContext;
        private readonly ILogger<OrgUnitRepository> _logger;

        public OrgUnitRepository(SQLDbContext dbContext, ILogger<OrgUnitRepository> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task ArchiveAsync(OrgUnit orgUnit)
        {
            _logger.LogDebug("OrgUnitRepository: Start archiving org unit with GUID: {Guid}", orgUnit.Guid);

            if (orgUnit == null)
            {
                _logger.LogWarning("OrgUnitRepository: Org unit is null.");
                throw new CustomArgumentNullException("OrgUnitRepository", nameof(orgUnit.Guid));
            }

            _dbContext.OrgUnits.Update(orgUnit);
            await _dbContext.SaveChangesAsync();

            _logger.LogDebug("OrgUnitRepository: Finished archiving org unit with GUID: {Guid}", orgUnit.Guid);
        }

        public async Task CreateAsync(OrgUnit orgUnit)
        { 
            _logger.LogDebug("OrgUnitRepository: Start creating a new org unit with name: {OrgUnitName}", orgUnit.OrgUnitName);

            if (orgUnit == null)
            {
                _logger.LogWarning("OrgUnitRepository: Org unit is null.");
                throw new CustomArgumentNullException("OrgUnitRepository", nameof(orgUnit.Guid));
            }

            // Should we check if the org unit already exists? // name is unique
            _dbContext.OrgUnits.Add(orgUnit);
            await _dbContext.SaveChangesAsync();

            _logger.LogDebug("OrgUnitRepository: Finished creating a new org unit with name: {OrgUnitName}", orgUnit.OrgUnitName);
        }

        public async Task DeleteAsync(OrgUnit orgUnit)
        {
            _logger.LogDebug("OrgUnitRepository: Start deleting org unit with GUID: {Guid}", orgUnit.Guid);

            if (orgUnit == null)
            {
                _logger.LogWarning("OrgUnitRepository: Org unit is null.");
                throw new CustomArgumentNullException("OrgUnitRepository", nameof(orgUnit.Guid));
            }

            _dbContext.OrgUnits.Remove(orgUnit);
            await _dbContext.SaveChangesAsync();

            _logger.LogDebug("OrgUnitRepository: Finished deleting org unit with GUID: {Guid}", orgUnit.Guid);
        }

        public async Task<IEnumerable<OrgUnit>> GetAllAsync()
        {
            _logger.LogDebug("OrgUnitRepository: Start getting all org units.");
            return await _dbContext.OrgUnits.ToListAsync();
        }

        public async Task<OrgUnit> GetByGuidAsync(Guid guid)
        {
            _logger.LogDebug("OrgUnitRepository: Start getting org unit with GUID: {Guid}", guid);

            if (guid == null)
            {
                _logger.LogWarning("OrgUnitRepository: GUID is null.");
                throw new CustomArgumentNullException("OrgUnitRepository", nameof(guid));
            }

            var orgUnit = await _dbContext.OrgUnits.FirstOrDefaultAsync(x => x.Guid == guid);
            if (orgUnit == null) 
            {
                _logger.LogWarning("OrgUnitRepository: Org unit with GUID: {Guid} was not found.", guid);
                throw new EntityNotFoundException("OrgUnitRepository", nameof(guid), guid);
            }

            _logger.LogDebug("OrgUnitRepository: Finished getting org unit with GUID: {Guid}", guid);
            return orgUnit;
            
        }

        public async Task UpdateAsync(OrgUnit orgUnit)
        {
        _logger.LogDebug("OrgUnitRepository: Start updating org unit with GUID: {Guid}", orgUnit.Guid);

            if (orgUnit == null)
        {
                _logger.LogWarning("OrgUnitRepository: Org unit is null.");
                throw new CustomArgumentNullException("OrgUnitRepository", nameof(orgUnit.Guid));
            }

            _dbContext.OrgUnits.Update(orgUnit);
            await _dbContext.SaveChangesAsync();

            _logger.LogDebug("OrgUnitRepository: Finished updating org unit with GUID: {Guid}", orgUnit.Guid);
        }
    }
}
