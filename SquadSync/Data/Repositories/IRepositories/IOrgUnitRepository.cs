using SquadSync.Data.Models;

namespace SquadSync.Data.Repositories.IRepositories
{
    public interface IOrgUnitRepository
    {
        Task ArchiveAsync(OrgUnit orgUnit);
        Task CreateAsync(OrgUnit orgUnit);
        Task DeleteAsync(OrgUnit orgUnit);
        Task<IEnumerable<OrgUnit>> GetAllAsync();
        Task<OrgUnit> GetByGuidAsync(Guid guid);
        Task UpdateAsync(OrgUnit orgUnit);
    }
}
