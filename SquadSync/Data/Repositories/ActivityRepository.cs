using Microsoft.EntityFrameworkCore;
using SquadSync.Data.Models;
using SquadSync.Data.Repositories.IRepositories;
using SquadSync.Exceptions;

namespace SquadSync.Data.Repositories
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly SQLDbContext _dbContext;
        private readonly ILogger<ActivityRepository> _logger;

        public ActivityRepository(
            SQLDbContext dbContext,
            ILogger<ActivityRepository> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task ArchiveActivityAsync(Activity activity)
        {
            _logger.LogDebug("ActivityRepository: Start archiving activity with GUID: {Guid}", activity.ActivityGuid);

            if (activity == null)
            {
                _logger.LogWarning("ActivityRepository: Activity is null.");
                throw new CustomArgumentNullException("ActivityRepository", nameof(activity.ActivityGuid));
            }

            _dbContext.Activities.Update(activity);
            await _dbContext.SaveChangesAsync();

            _logger.LogDebug("ActivityRepository: Finished archiving activity with GUID: {Guid}", activity.ActivityGuid);
        }

        public async Task CreateActivityAsync(Activity activity)
        {
            _logger.LogDebug("ActivityRepository: Start creating activity with GUID: {Guid}", activity.ActivityGuid);

            if (activity == null)
            {
                _logger.LogWarning("ActivityRepository: Activity is null.");
                throw new CustomArgumentNullException("ActivityRepository", nameof(activity.ActivityGuid));
            }

            await _dbContext.Activities.AddAsync(activity);
            await _dbContext.SaveChangesAsync();

            _logger.LogDebug("ActivityRepository: Finished creating activity with GUID: {Guid}", activity.ActivityGuid);
        }

        public async Task DeleteActivityAsync(Activity activity)
        {
            _logger.LogDebug("ActivityRepository: Start deleting activity with GUID: {Guid}", activity.ActivityGuid);

            if (activity == null)
            {
                _logger.LogWarning("ActivityRepository: Activity is null.");
                throw new CustomArgumentNullException("ActivityRepository", nameof(activity.ActivityGuid));
            }

            _dbContext.Activities.Remove(activity);
            await _dbContext.SaveChangesAsync();

            _logger.LogDebug("ActivityRepository: Finished deleting activity with GUID: {Guid}", activity.ActivityGuid);
        }

        public async Task<ICollection<Activity>> GetAllActivitiesAsync()
        {
            _logger.LogDebug("ActivityRepository: Start getting all activities.");

            var activities = await _dbContext.Activities.ToListAsync();

            _logger.LogDebug("ActivityRepository: Finished getting all activities.");

            return activities;
        }

        public async Task<Activity> GetActivityByGuidAsync(Guid activityGuid)
        {
            _logger.LogDebug("ActivityRepository: Start getting activity with GUID: {Guid}", activityGuid);

            if (activityGuid == null)
            {
                _logger.LogWarning("ActivityRepository: Activity GUID is null.");
                throw new CustomArgumentNullException("ActivityRepository", nameof(activityGuid));
            }

            var activity = await _dbContext.Activities.FirstOrDefaultAsync(a => a.ActivityGuid == activityGuid);
            if(activity == null)
            {
                _logger.LogWarning("ActivityRepository: Activity with GUID: {Guid} does not exist.", activityGuid);
                throw new EntityNotFoundException("ActivityRepository", nameof(activityGuid));
            }

            _logger.LogDebug("ActivityRepository: Finished getting activity with GUID: {Guid}", activityGuid);
            return activity;
        }

        public async Task<ICollection<Activity>> GetActivitiesByOrganizer(IRoleBearer organizer)
        {
            _logger.LogDebug("ActivityRepository: Start getting activities by organizer with GUID: {Guid}", organizer.RoleBearerGuid);

            if (organizer == null)
            {
                _logger.LogWarning("ActivityRepository: Organizer is null.");
                throw new CustomArgumentNullException("ActivityRepository", nameof(organizer.RoleBearerGuid));
            }

            var activities = await _dbContext.Activities.Where(a => a.Organizer.RoleBearerGuid == organizer.RoleBearerGuid).ToListAsync();

            _logger.LogDebug("ActivityRepository: Finished getting activities by organizer with GUID: {Guid}", organizer.RoleBearerGuid);
            return activities;
        }

        // Get activities by all participants
        public async Task<ICollection<Activity>> GetActivitieByParticipants(ICollection<IRoleBearer> participants)
        {
            _logger.LogDebug("ActivityRepository: Start getting activities by participants.");

            if (participants == null)
            {
                _logger.LogWarning("ActivityRepository: Participants is null.");
                throw new CustomArgumentNullException("ActivityRepository", nameof(participants));
            }

            var activities = await _dbContext.Activities.Where(a => a.Participants == participants).ToListAsync();

            _logger.LogDebug("ActivityRepository: Finished getting activities by participants.");
            return activities;
        }

        public async Task UpdateActivityAsync(Activity activity)
        {
            _logger.LogDebug("ActivityRepository: Start updating activity with GUID: {Guid}", activity.ActivityGuid);

            if (activity == null)
            {
                _logger.LogWarning("ActivityRepository: Activity is null.");
                throw new CustomArgumentNullException("ActivityRepository", nameof(activity.ActivityGuid));
            }

            _dbContext.Activities.Update(activity);
            await _dbContext.SaveChangesAsync();

            _logger.LogDebug("ActivityRepository: Finished updating activity with GUID: {Guid}", activity.ActivityGuid);
        }
    }
}
