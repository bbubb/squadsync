using SquadSync.Enums;
using SquadSync.Data.Models;

namespace SquadSync.Data.Models
{
    public class Activity
    {
        public Guid ActivityGuid { get; set; }
        public long ActivityId { get; set; }
        public string ActivityName { get; set; }
        public string? ActivityDescription { get; set; }
        public DateTime ActivityStart { get; set; }
        public DateTime ActivityEnd { get; set; }
        public string Location { get; set; } // Address, GPS, name of place, etc. // Will probably need to be an object
        public string Site { get; set; } // Ex: Field 13B // Will probably need to be an object
        public ActivityTypeEnum ActivityType { get; set; }
        public ActivityStatusEnum ActivityStatus { get; set; }
        public IRoleBearer Organizer { get; set; }
        public ICollection<IRoleBearer> Participants { get; set; }
        public ICollection<RoleRequest> RoleRequests { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? ArchivedOn { get; set; }
    }
}
