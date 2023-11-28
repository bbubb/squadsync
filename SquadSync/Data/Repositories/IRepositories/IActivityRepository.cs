using SquadSync.Data.Models;

namespace SquadSync.Data.Repositories.IRepositories
{
    public interface IActivityRepository
    {
        Task CreateActivityAsync(Activity activity);
        Task ArchiveActivityAsync(Activity activity);
        Task DeleteActivityAsync(Activity activity);
        Task<Activity> GetActivityByGuidAsync(Guid activityGuid);
        Task<ICollection<Activity>> GetAllActivitiesAsync();
        Task<ICollection<Activity>> GetActivitieByParticipants(ICollection<IRoleBearer> participants);
        Task<ICollection<Activity>> GetActivitiesByOrganizer(IRoleBearer organizer);
    }
}
