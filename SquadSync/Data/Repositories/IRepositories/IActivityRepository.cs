using SquadSync.Data.Models;

namespace SquadSync.Data.Repositories.IRepositories
{
    public interface IActivityRepository
    {
        Task CreateActivityAsync(Activity activity);
        Task ArchiveActivityAsync(Activity activity);
        Task DeleteActivityAsync(Activity activity);
        Task<Activity> GetActivityByGuidAsync(Guid activityGuid);
        Task<IEnumerable<Activity>> GetAllActivitiesAsync();
        Task<IEnumerable<Activity>> GetActivitieByParticipants(IEnumerable<IRoleBearer> participants);
        Task<IEnumerable<Activity>> GetActivitiesByOrganizer(IRoleBearer organizer);
    }
}
